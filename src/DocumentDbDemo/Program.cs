using System.Net.Security;
using MongoDB.Bson;
using MongoDB.Driver;

// Connect to DocumentDB gateway
var connectionString = Environment.GetEnvironmentVariable("DOCUMENTDB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Set the DOCUMENTDB_CONNECTION_STRING environment variable.");
var settings = MongoClientSettings.FromConnectionString(connectionString);
settings.SslSettings = new SslSettings
{
    ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true
};
var client = new MongoClient(settings);

// Create database and collection
var db = client.GetDatabase("sampledb");
var collection = db.GetCollection<BsonDocument>("products");

// Insert documents
var products = new[]
{
    new BsonDocument { { "_id", "prod-001" }, { "name", "Laptop" }, { "price", 1299.99 }, { "category", "electronics" } },
    new BsonDocument { { "_id", "prod-002" }, { "name", "Headphones" }, { "price", 79.99 }, { "category", "electronics" } },
    new BsonDocument { { "_id", "prod-003" }, { "name", "Notebook" }, { "price", 4.99 }, { "category", "office" } }
};

try
{
    collection.InsertMany(products, new InsertManyOptions { IsOrdered = false });
    Console.WriteLine($"Inserted {products.Length} documents into sampledb.products");
}
catch (MongoBulkWriteException e) when (e.WriteErrors.All(err => err.Category == ServerErrorCategory.DuplicateKey))
{
    var inserted = products.Length - e.WriteErrors.Count;
    Console.WriteLine($"Inserted {inserted} new documents (skipped {e.WriteErrors.Count} duplicates)");
}

// Query: find electronics over $100
var filter = Builders<BsonDocument>.Filter.And(
    Builders<BsonDocument>.Filter.Regex("category", new BsonRegularExpression("^electronics$", "i")),
    Builders<BsonDocument>.Filter.Gt("price", 100)
);

var results = collection.Find(filter).ToList();
Console.WriteLine($"\nElectronics over $100:");
foreach (var doc in results)
{
    Console.WriteLine($"  {doc["name"]} - ${doc["price"]}");
}

// Aggregation: average price by category
var pipeline = new[]
{
    new BsonDocument("$group", new BsonDocument
    {
        { "_id", new BsonDocument("$toLower", "$category") },
        { "avgPrice", new BsonDocument("$avg", "$price") }
    })
};

var aggResults = collection.Aggregate<BsonDocument>(pipeline).ToList();
Console.WriteLine($"\nAverage price by category:");
foreach (var doc in aggResults)
{
    Console.WriteLine($"  {doc["_id"]}: ${doc["avgPrice"]:F2}");
}
