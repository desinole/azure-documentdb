using System.Net.Security;
using Bogus;
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
    // Target collection to create the index on
    { "createIndexes", "words" },
    { "indexes", new BsonArray
        {
            new BsonDocument
            {
                // Unique name to identify this index
                { "name", "vectorIndex" },
                // "cosmosSearch" marks the "embedding" field as a vector index (not a standard B-tree)
                { "key", new BsonDocument("embedding", "cosmosSearch") },
                { "cosmosSearchOptions", new BsonDocument
                    {
                        // "vector-hnsw" = Hierarchical Navigable Small World graph — an in-memory
                        // ANN algorithm with high recall. Use "vector-diskann" for SSD-based search at larger scale.
                        { "kind", "vector-hnsw" },
                        // Must match the output size of the embedding model (text-embedding-3-small = 1536)
                        { "dimensions", dimensions },
                        // Similarity metric: COS = cosine similarity. Alternatives: IP (inner product), L2 (Euclidean)
                        { "similarity", "COS" },
                        // Max bi-directional links per node in the graph. Higher = better recall, more memory. Default: 16
                        { "m", 16 },
                        // Neighbors evaluated during index construction. Higher = better quality index, slower build. Default: 64
                        { "efConstruction", 64 }
                    }
                }
            }
        }
    }
};

db.RunCommand<BsonDocument>(createIndex, ReadPreference.Primary);
Console.WriteLine("HNSW vector index created.\n");

// --- Search timing stats ---
var searchCount = 0;
var totalSearchMs = 0.0;

// --- Menu: Insert, Search, or Quit ---
while (true)
{
    Console.WriteLine("What would you like to do?");
    Console.WriteLine("  1) Insert records");
    Console.WriteLine("  2) Search records");
    Console.WriteLine("  3) Quit");
    Console.Write("Enter choice (1, 2, or 3): ");
    var choice = Console.ReadLine()?.Trim();

    if (choice == "1")
    {
        InsertRecords();
    }
    else if (choice == "2")
    {
        SearchRecords();
    }
    else if (choice == "3")
    {
        Console.WriteLine("Goodbye!");
        return;
    }
    else
    {
        Console.WriteLine("Invalid choice. Try again.\n");
    }
}

// --- Insert path: Generate 200 big-box-store product words with Bogus ---
void InsertRecords()
{
    var departments = new[]
    {
        "Electronics", "Furniture", "Clothing", "Garden", "Tools",
        "Grocery", "Toys", "Automotive", "Sports", "Kitchen",
        "Bathroom", "Bedding", "Lighting", "Appliances", "Pet Supplies",
        "Office", "Baby", "Pharmacy", "Seasonal", "Hardware"
    };

    var products = new Dictionary<string, string[]>
    {
        ["Electronics"] = ["television", "laptop", "tablet", "headphones", "speaker", "camera", "monitor", "keyboard", "mouse", "charger", "power strip", "surge protector", "flash drive", "memory card", "smart watch", "streaming device", "drone", "projector", "webcam", "microphone"],
        ["Furniture"] = ["sofa", "recliner", "bookshelf", "desk", "dining table", "office chair", "nightstand", "dresser", "futon", "coffee table", "bar stool", "filing cabinet", "tv stand", "shoe rack", "storage bench", "folding table", "bean bag", "step stool", "coat rack", "end table"],
        ["Clothing"] = ["t-shirt", "jeans", "jacket", "hoodie", "sneakers", "boots", "sandals", "socks", "underwear", "polo shirt", "dress shirt", "cargo pants", "rain coat", "winter gloves", "baseball cap", "belt", "scarf", "pajamas", "swim trunks", "work vest"],
        ["Garden"] = ["lawn mower", "garden hose", "fertilizer", "potting soil", "rake", "shovel", "wheelbarrow", "pruning shears", "weed killer", "sprinkler", "flower pot", "mulch", "bird feeder", "outdoor planter", "garden gloves", "leaf blower", "hedge trimmer", "seed starter", "compost bin", "patio umbrella"],
        ["Tools"] = ["drill", "hammer", "screwdriver set", "wrench", "tape measure", "level", "saw", "pliers", "socket set", "utility knife", "work light", "tool box", "sanding block", "clamp", "stud finder", "Allen wrench set", "wire stripper", "bolt cutter", "pry bar", "chisel set"],
        ["Grocery"] = ["bread", "milk", "eggs", "cereal", "pasta", "rice", "canned soup", "peanut butter", "cooking oil", "sugar", "flour", "coffee", "bottled water", "snack chips", "frozen pizza", "salad mix", "yogurt", "cheese", "lunch meat", "granola bar"],
        ["Toys"] = ["building blocks", "action figure", "board game", "puzzle", "stuffed animal", "toy truck", "play dough", "coloring book", "dollhouse", "remote control car", "jump rope", "water gun", "kite", "bubble wand", "train set", "foam ball", "toy robot", "card game", "sidewalk chalk", "sticker book"],
        ["Automotive"] = ["motor oil", "car battery", "wiper blades", "tire gauge", "jumper cables", "air freshener", "floor mats", "car wash soap", "brake fluid", "headlight bulb", "fuse kit", "steering wheel cover", "trunk organizer", "ice scraper", "seat cover", "dash cam", "phone mount", "tire inflator", "touch up paint", "fuel additive"],
        ["Sports"] = ["basketball", "yoga mat", "dumbbell", "resistance band", "tennis racket", "bicycle helmet", "water bottle", "running shoes", "fitness tracker", "jump rope", "camping tent", "sleeping bag", "fishing rod", "cooler", "backpack", "soccer ball", "golf ball", "swim goggles", "knee brace", "protein powder"],
        ["Kitchen"] = ["blender", "toaster", "coffee maker", "frying pan", "cutting board", "knife set", "mixing bowl", "measuring cup", "can opener", "food storage container", "dish rack", "oven mitt", "paper towel holder", "trash bags", "dish soap", "sponge", "baking sheet", "slow cooker", "air fryer", "spatula"],
        ["Bathroom"] = ["bath towel", "shower curtain", "soap dispenser", "toothbrush holder", "toilet brush", "bath mat", "shampoo", "body wash", "hand soap", "tissue box", "bathroom scale", "vanity mirror", "shower head", "plunger", "towel rack", "laundry hamper", "first aid kit", "cotton swabs", "lotion", "razor"],
        ["Bedding"] = ["pillow", "comforter", "bed sheet set", "mattress pad", "duvet cover", "throw blanket", "mattress topper", "pillow case", "weighted blanket", "bed frame", "quilt", "sleeping pillow", "body pillow", "electric blanket", "bed skirt", "foam mattress", "crib sheet", "blanket", "linen set", "cushion"],
        ["Lighting"] = ["table lamp", "floor lamp", "ceiling fan", "light bulb", "string lights", "desk lamp", "night light", "flashlight", "lantern", "under cabinet light", "dimmer switch", "flood light", "motion sensor light", "chandelier", "pendant light", "recessed light", "track lighting", "work lamp", "solar light", "candle"],
        ["Appliances"] = ["refrigerator", "washing machine", "dryer", "dishwasher", "microwave", "vacuum cleaner", "iron", "space heater", "dehumidifier", "humidifier", "window fan", "air conditioner", "chest freezer", "garbage disposal", "water filter", "electric kettle", "steam mop", "robot vacuum", "pressure washer", "air purifier"],
        ["Pet Supplies"] = ["dog food", "cat food", "pet bed", "leash", "collar", "chew toy", "cat litter", "food bowl", "fish tank", "bird cage", "hamster wheel", "pet carrier", "flea treatment", "grooming brush", "puppy pad", "aquarium filter", "dog treat", "scratching post", "pet shampoo", "water fountain"],
        ["Office"] = ["printer paper", "ink cartridge", "stapler", "tape dispenser", "binder", "sticky notes", "pen set", "pencil sharpener", "paper clip", "envelope", "label maker", "white board", "desk organizer", "file folder", "calculator", "laminator", "paper shredder", "desk calendar", "notebook", "highlighter"],
        ["Baby"] = ["diapers", "baby wipes", "baby bottle", "pacifier", "baby formula", "car seat", "stroller", "high chair", "baby monitor", "teething ring", "baby blanket", "sippy cup", "bib", "changing pad", "diaper bag", "baby gate", "crib mobile", "baby lotion", "baby shampoo", "rattle"],
        ["Pharmacy"] = ["pain reliever", "bandage", "cold medicine", "allergy pills", "vitamins", "hand sanitizer", "thermometer", "cough drops", "antacid", "eye drops", "nasal spray", "heating pad", "ice pack", "sunscreen", "insect repellent", "lip balm", "cotton balls", "rubbing alcohol", "hydrogen peroxide", "face mask"],
        ["Seasonal"] = ["patio chair", "grill", "pool float", "snow shovel", "space heater", "christmas lights", "wreath", "outdoor rug", "fire pit", "picnic table", "lawn chair", "beach towel", "ice melt", "window insulation", "portable fan", "porch swing", "tiki torch", "hammock", "window screen", "weather stripping"],
        ["Hardware"] = ["paint", "paint brush", "wall anchor", "wood screw", "nail", "sandpaper", "caulk", "duct tape", "electrical tape", "zip tie", "padlock", "door knob", "hinge", "shelf bracket", "picture hook", "wood glue", "epoxy", "extension cord", "smoke detector", "carbon monoxide detector"]
    };

    var faker = new Faker();
    const int totalRecords = 200;
    const int batchSize = 50;

    Console.WriteLine($"\nGenerating {totalRecords} big-box-store product words...");

    var words = Enumerable.Range(0, totalRecords).Select(_ =>
    {
        var dept = faker.PickRandom(departments);
        return faker.PickRandom(products[dept]);
    }).ToList();

    // Deduplicate while preserving count by appending a variant when needed
    var seen = new Dictionary<string, int>();
    for (var i = 0; i < words.Count; i++)
    {
        var w = words[i];
        if (seen.ContainsKey(w))
        {
            seen[w]++;
            words[i] = $"{w} {seen[w]}";
        }
        else
        {
            seen[w] = 0;
        }
    }

    var wordCount = words.Count;
    Console.WriteLine($"Inserting {wordCount} words in batches of {batchSize}...\n");

    for (var i = 0; i < words.Count; i += batchSize)
    {
        var batch = words.Skip(i).Take(batchSize).ToList();
        var docs = new List<BsonDocument>();

        foreach (var word in batch)
        {
            var embedding = GetEmbedding(word);
            docs.Add(new BsonDocument
            {
                { "word", word },
                { "embedding", embedding }
            });
        }

        collection.InsertMany(docs);
        Console.WriteLine($"  Inserted batch {i / batchSize + 1} ({Math.Min(i + batchSize, words.Count)}/{wordCount})");
    }

    Console.WriteLine($"\nInserted {wordCount} words total.");
}

// --- Search path: Prompt user for search term and perform vector search ---
void SearchRecords()
{
    Console.Write("\nEnter a search term: ");
    var searchQuery = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(searchQuery))
    {
        Console.WriteLine("No search term provided. Exiting.");
        return;
    }

    Console.WriteLine($"Searching for words similar to: \"{searchQuery}\"");

    var swEmbed = System.Diagnostics.Stopwatch.StartNew();
    var queryVector = GetEmbedding(searchQuery);
    swEmbed.Stop();

    var searchPipeline = new[]
    {
        // Stage 1: $search — performs the vector similarity search using the cosmosSearch index
        new BsonDocument("$search", new BsonDocument("cosmosSearch",
            new BsonDocument
            {
                // "path" — the document field that contains the stored vector embeddings
                { "path", "embedding" },
                // "vector" — the query embedding to compare against stored vectors
                { "vector", queryVector },
                // "k" — number of nearest neighbors to return (top-k results)
                { "k", 10 }
            })),
        // Stage 2: $project — controls which fields appear in the output
        new BsonDocument("$project", new BsonDocument
        {
            // Include the "word" field (1 = include, 0 = exclude)
            { "word", 1 },
            // "searchScore" is a built-in meta field that holds the cosine similarity score (0–1)
            { "score", new BsonDocument("$meta", "searchScore") }
        }),
        // Stage 3: $sort — order results by similarity score descending (most similar first)
        new BsonDocument("$sort", new BsonDocument("score", -1))
    };

    var swSearch = System.Diagnostics.Stopwatch.StartNew();
    var results = collection.Aggregate<BsonDocument>(searchPipeline).ToList();
    swSearch.Stop();

    Console.WriteLine("\nResults (sorted by similarity, descending):");
    Console.WriteLine($"  {"Rank",-6} {"Word",-20} {"Score"}");
    Console.WriteLine($"  {new string('-', 40)}");
    var rank = 1;
    foreach (var doc in results)
    {
        Console.WriteLine($"  {rank++,-6} {doc["word"],-20} {doc["score"]:F6}");
    }

    searchCount++;
    totalSearchMs += swSearch.Elapsed.TotalMilliseconds;
    var avgMs = totalSearchMs / searchCount;

    Console.WriteLine();
    Console.WriteLine($"  ⏱  Embedding: {swEmbed.Elapsed.TotalMilliseconds:F1}ms | " +
                      $"Search: {swSearch.Elapsed.TotalMilliseconds:F1}ms | " +
                      $"Total: {swEmbed.Elapsed.TotalMilliseconds + swSearch.Elapsed.TotalMilliseconds:F1}ms");
    Console.WriteLine($"  📊 Session: {searchCount} searches | " +
                      $"Avg search: {avgMs:F1}ms\n");
}
