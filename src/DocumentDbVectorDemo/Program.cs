using System.Net.Security;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenAI.Embeddings;

// --- OpenAI Embedding Setup ---
var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set the OPENAI_API_KEY environment variable.");
var embeddingClient = new EmbeddingClient("text-embedding-3-small", openAiKey);
const int dimensions = 1536;

BsonArray GetEmbedding(string text)
{
    var result = embeddingClient.GenerateEmbedding(text);
    return new BsonArray(result.Value.ToFloats().ToArray().Select(f => (double)f));
}

// --- Connect to DocumentDB ---
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
var collection = db.GetCollection<BsonDocument>("words");

// --- Step 1: Create an HNSW vector index ---
Console.WriteLine("Creating HNSW vector index on 'words' collection...");

var createIndex = new BsonDocument
{
    { "createIndexes", "words" },
    { "indexes", new BsonArray
        {
            new BsonDocument
            {
                { "name", "vectorIndex" },
                { "key", new BsonDocument("embedding", "cosmosSearch") },
                { "cosmosSearchOptions", new BsonDocument
                    {
                        { "kind", "vector-hnsw" },
                        { "dimensions", dimensions },
                        { "similarity", "COS" },
                        { "m", 16 },
                        { "efConstruction", 64 }
                    }
                }
            }
        }
    }
};

db.RunCommand<BsonDocument>(createIndex, ReadPreference.Primary);
Console.WriteLine("HNSW vector index created.\n");

// --- Step 2: Prompt user for words, generate embeddings, and insert ---
Console.WriteLine("Enter words to store (type 'exit' or 'quit' to finish):");

var wordCount = 0;
while (true)
{
    Console.Write($"  Word {wordCount + 1}: ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input))
        continue;
    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
        input.Equals("quit", StringComparison.OrdinalIgnoreCase))
        break;

    Console.WriteLine($"    Generating embedding for: \"{input}\"");
    var embedding = GetEmbedding(input);
    collection.InsertOne(new BsonDocument
    {
        { "word", input },
        { "embedding", embedding }
    });
    wordCount++;
    Console.WriteLine($"    Stored \"{input}\".");
}

Console.WriteLine($"\nInserted {wordCount} words total.\n");

// --- Step 3: Prompt user for search term and perform vector search ---
Console.Write("Enter a search term: ");
var searchQuery = Console.ReadLine()?.Trim();
if (string.IsNullOrEmpty(searchQuery))
{
    Console.WriteLine("No search term provided. Exiting.");
    return;
}

Console.WriteLine($"Searching for words similar to: \"{searchQuery}\"");
var queryVector = GetEmbedding(searchQuery);

var searchPipeline = new[]
{
    new BsonDocument("$search", new BsonDocument("cosmosSearch",
        new BsonDocument
        {
            { "path", "embedding" },
            { "vector", queryVector },
            { "k", wordCount > 0 ? wordCount : 10 }
        })),
    new BsonDocument("$project", new BsonDocument
    {
        { "word", 1 },
        { "score", new BsonDocument("$meta", "searchScore") }
    }),
    new BsonDocument("$sort", new BsonDocument("score", -1))
};

var results = collection.Aggregate<BsonDocument>(searchPipeline).ToList();

Console.WriteLine("\nResults (sorted by similarity, descending):");
Console.WriteLine($"  {"Rank",-6} {"Word",-20} {"Score"}");
Console.WriteLine($"  {new string('-', 40)}");
var rank = 1;
foreach (var doc in results)
{
    Console.WriteLine($"  {rank++,-6} {doc["word"],-20} {doc["score"]:F6}");
}
