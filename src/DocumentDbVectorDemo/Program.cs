using MongoDB.Bson;
using MongoDB.Driver;

// Connect to DocumentDB gateway
var client = new MongoClient("mongodb://admin:YourPassword123!@localhost:10260");
var db = client.GetDatabase("vector_db");
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
                        { "dimensions", 3 },
                        { "similarity", "COS" },
                        { "maxDegree", 32 },
                        { "lBuild", 64 }
                    }
                }
            }
        }
    }
};

db.RunCommand<BsonDocument>(createIndex);
Console.WriteLine("DiskANN index created.\n");

// --- Step 2: Insert documents with vector embeddings ---
Console.WriteLine("Inserting documents with embeddings...");

var documents = new[]
{
    new BsonDocument
    {
        { "name", "Wireless Headphones" },
        { "category", "electronics" },
        { "price", 79.99 },
        { "embedding", new BsonArray { 0.52, 0.20, 0.23 } }
    },
    new BsonDocument
    {
        { "name", "Bluetooth Speaker" },
        { "category", "electronics" },
        { "price", 49.99 },
        { "embedding", new BsonArray { 0.55, 0.89, 0.44 } }
    },
    new BsonDocument
    {
        { "name", "Notebook" },
        { "category", "office" },
        { "price", 4.99 },
        { "embedding", new BsonArray { 0.13, 0.92, 0.85 } }
    },
    new BsonDocument
    {
        { "name", "Gaming Laptop" },
        { "category", "electronics" },
        { "price", 1299.99 },
        { "embedding", new BsonArray { 0.91, 0.76, 0.83 } }
    }
};

collection.InsertMany(documents);
Console.WriteLine($"Inserted {documents.Length} documents.\n");

// --- Step 3: Vector similarity search ---
Console.WriteLine("Performing vector similarity search...");

var queryVector = new BsonArray { 0.52, 0.28, 0.12 };

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
Console.WriteLine("\nFiltered vector search (electronics only)...");

var filteredPipeline = new[]
{
    new BsonDocument("$search", new BsonDocument("cosmosSearch",
        new BsonDocument
        {
            { "path", "embedding" },
            { "vector", queryVector },
            { "k", 3 },
            { "filter", new BsonDocument("category", new BsonDocument("$eq", "electronics")) }
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
