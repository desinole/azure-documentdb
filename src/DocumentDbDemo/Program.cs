using MongoDB.Bson;
using MongoDB.Driver;

// Connect to DocumentDB gateway
var client = new MongoClient("mongodb://admin:YourPassword123!@localhost:10260");

// Create database and collection
var db = client.GetDatabase("demo_db");
var collection = db.GetCollection<BsonDocument>("products");

// Insert documents
var products = new[]
{
    new BsonDocument { { "name", "Laptop" }, { "price", 1299.99 }, { "category", "electronics" } },
    new BsonDocument { { "name", "Headphones" }, { "price", 79.99 }, { "category", "electronics" } },
    new BsonDocument { { "name", "Notebook" }, { "price", 4.99 }, { "category", "office" } }
};

collection.InsertMany(products);
Console.WriteLine($"Inserted {products.Length} documents into demo_db.products");

// Query: find electronics over $100
var filter = Builders<BsonDocument>.Filter.And(
    Builders<BsonDocument>.Filter.Eq("category", "electronics"),
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
        { "_id", "$category" },
        { "avgPrice", new BsonDocument("$avg", "$price") }
    })
};

var aggResults = collection.Aggregate<BsonDocument>(pipeline).ToList();
Console.WriteLine($"\nAverage price by category:");
foreach (var doc in aggResults)
{
    Console.WriteLine($"  {doc["_id"]}: ${doc["avgPrice"]:F2}");
}
