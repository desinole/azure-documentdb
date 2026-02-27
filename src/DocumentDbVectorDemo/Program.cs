using System.Net.Security;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenAI.Embeddings;

// --- OpenAI Embedding Setup ---
// Set your OpenAI API key as an environment variable: OPENAI_API_KEY
var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set the OPENAI_API_KEY environment variable.");
var embeddingClient = new EmbeddingClient("text-embedding-3-small", openAiKey);
const int dimensions = 1536;

// Helper: generate an embedding vector from text using OpenAI
BsonArray GetEmbedding(string text)
{
    var result = embeddingClient.GenerateEmbedding(text);
    return new BsonArray(result.Value.ToFloats().ToArray().Select(f => (double)f));
}

// Connect to DocumentDB gateway
var connectionString = Environment.GetEnvironmentVariable("DOCUMENTDB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Set the DOCUMENTDB_CONNECTION_STRING environment variable.");
var settings = MongoClientSettings.FromConnectionString(connectionString);
settings.SslSettings = new SslSettings
{
    ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true
};
settings.ServerSelectionTimeout = TimeSpan.FromMinutes(5);
var client = new MongoClient(settings);
var db = client.GetDatabase("sampledb");
var collection = db.GetCollection<BsonDocument>("products");

// --- Step 1: Create a DiskANN vector index ---
Console.WriteLine("Creating DiskANN vector index...");

var createIndex = new BsonDocument
{
    { "createIndexes", "products" },
    { "indexes", new BsonArray
        {
            new BsonDocument
            {
                { "name", "vectorIndex" },
                { "key", new BsonDocument("embedding", "cosmosSearch") },
                { "cosmosSearchOptions", new BsonDocument
                    {
                        { "kind", "vector-diskann" },
                        { "dimensions", dimensions },
                        { "similarity", "COS" },
                        { "maxDegree", 32 },
                        { "lBuild", 64 }
                    }
                }
            }
        }
    }
};

db.RunCommand<BsonDocument>(createIndex, ReadPreference.Primary);
Console.WriteLine("DiskANN index created.");

// Create a standard index on category for filtered vector search
collection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(
    Builders<BsonDocument>.IndexKeys.Ascending("category"),
    new CreateIndexOptions { Name = "category_1" }));
Console.WriteLine("Category filter index created.\n");

// --- Step 2: Insert documents with OpenAI-generated embeddings ---
Console.WriteLine("Generating embeddings via OpenAI and inserting documents...");

var products = new[]
{
    ("vec-001", "Wireless Headphones", "electronics", 79.99),
    ("vec-002", "Bluetooth Speaker", "electronics", 49.99),
    ("vec-003", "Notebook", "office", 4.99),
    ("vec-004", "Gaming Laptop", "electronics", 1299.99)
};

var documents = new List<BsonDocument>();
foreach (var (id, name, category, price) in products)
{
    Console.WriteLine($"  Generating embedding for: {name}");
    var embedding = GetEmbedding($"{name} - {category}");
    documents.Add(new BsonDocument
    {
        { "_id", id },
        { "name", name },
        { "category", category },
        { "price", price },
        { "embedding", embedding }
    });
}

collection.InsertMany(documents);
Console.WriteLine($"Inserted {documents.Count} documents.\n");

// --- Step 3: Vector similarity search ---
// Generate an embedding for the search query using the same model
var searchQuery = "premium audio equipment";
Console.WriteLine($"Searching for: \"{searchQuery}\"");
var queryVector = GetEmbedding(searchQuery);

var searchPipeline = new[]
{
    new BsonDocument("$search", new BsonDocument("cosmosSearch",
        new BsonDocument
        {
            { "path", "embedding" },
            { "vector", queryVector },
            { "k", 3 }
        })),
    new BsonDocument("$project", new BsonDocument
    {
        { "name", 1 },
        { "category", 1 },
        { "price", 1 },
        { "similarityScore", new BsonDocument("$meta", "searchScore") }
    })
};

var results = collection.Aggregate<BsonDocument>(searchPipeline).ToList();

Console.WriteLine("Top 3 similar products:");
foreach (var doc in results)
{
    Console.WriteLine($"  {doc["name"],-25} | {doc["category"],-12} | ${doc["price"],-8} | score: {doc["similarityScore"]:F4}");
}

// --- Step 4: Filtered vector search ---
var filteredQuery = "portable music device";
Console.WriteLine($"\nFiltered search (electronics only): \"{filteredQuery}\"");
var filteredQueryVector = GetEmbedding(filteredQuery);

var filteredPipeline = new[]
{
    new BsonDocument("$search", new BsonDocument("cosmosSearch",
        new BsonDocument
        {
            { "path", "embedding" },
            { "vector", filteredQueryVector },
            { "k", 3 },
            { "filter", new BsonDocument("category", new BsonDocument("$regex", new BsonRegularExpression("^electronics$", "i"))) }
        })),
    new BsonDocument("$project", new BsonDocument
    {
        { "name", 1 },
        { "category", 1 },
        { "price", 1 },
        { "similarityScore", new BsonDocument("$meta", "searchScore") }
    })
};

var filteredResults = collection.Aggregate<BsonDocument>(filteredPipeline).ToList();

Console.WriteLine("Similar electronics:");
foreach (var doc in filteredResults)
{
    Console.WriteLine($"  {doc["name"],-25} | ${doc["price"],-8} | score: {doc["similarityScore"]:F4}");
}
