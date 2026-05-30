---
marp: true
theme: default
paginate: true
backgroundColor: #fff
backgroundImage: url('https://marp.app/assets/hero-background.svg')
header: 'Azure DocumentDB - Open Source Distributed Database'
footer: '© 2026'
---

# **Azure DocumentDB**
## Azure DocumentDB is back: Open Source and AI-ready

### Santosh Hari

<!--
Presenter Notes:
- Welcome the audience and introduce yourself briefly
- Set context: this talk covers what Azure DocumentDB is, why it exists, and how to get started
- Mention that there will be live demos — encourage questions throughout
-->

---

# **Follow Along 📲**

![w:280](img/talk-qr.png)

**All content from this talk (slides, demo code, and setup instructions) lives in this GitHub repo:**

### [github.com/desinole/azure-documentdb](https://github.com/desinole/azure-documentdb)

<!--
Presenter Notes:
- Scan the QR code now if you'd like to follow along with the slides and code as we go
- Everything in this talk (the slides, all demo projects, and setup steps) is in this one repo
- I'll show this QR code again at the end, so no need to rush
-->

---

# **Speaker Introduction**

<!-- Add your information here -->

- **Name:** Santosh Hari
- **Role:** Azure EngOps
- **Connect:** 
  - LinkedIn: santoshhari
  - BlueSky: @santoshhari.dev
  - GitHub: desinole

<!--
Presenter Notes:
- Quick personal intro — keep it brief, audience is here for the content
- Mention relevant experience with Azure, databases, or open source if applicable
- Encourage attendees to connect afterwards for follow-up questions
-->

---

# **Agenda**

1. 🏗️ What is DocumentDB & why it matters
2. 🐳 **Demo:** Local setup, CRUD & querying
3. 🤖 Vector search & AI fundamentals
4. 🔍 **Demo:** Vector search with HNSW & DiskANN

<!--
Presenter Notes:
- Walk through the agenda quickly — give audience a roadmap of what to expect
- Highlight that there are multiple live demos throughout the talk
- Mention the talk will go from concepts to hands-on in about 45 minutes
- Note: there are TWO vector search demos — one with HNSW (in-memory) and one with DiskANN (SSD-based)
- Vector search section is especially relevant for anyone building AI/ML applications
-->

---

# **Introduction to Azure DocumentDB**

## What is Azure DocumentDB?

- **Open Source** distributed document database (released 2025) 
- ❌ 2017, ❌ AWS
- **MongoDB-compatible API** running on **PostgreSQL**
- Built for **cloud-native** applications
- Designed for **horizontal scalability**
- **ACID transactions** at document and collection level
- **Global distribution** capabilities

<!--
Presenter Notes:
- Emphasize that this is a NEW open source project released in 2025, not a legacy product from 2017 or an AWS service
- Key differentiator: MongoDB API compatibility running on PostgreSQL foundation
- This means familiar MongoDB syntax and tooling, but PostgreSQL reliability underneath
- Highlight the "best of both worlds" approach
- Use case: Teams who want MongoDB-style development but need PostgreSQL enterprise features
- Point out ACID guarantees - full transactional support unlike eventual consistency models
- Global distribution through Azure's infrastructure - multi-region deployment made simple
-->

---

# **Introduction to Azure DocumentDB**

## Why Document Databases?

- **Flexible Schema** - evolve without downtime
- **Natural data representation** - JSON documents
- **Developer friendly** - maps to application objects
- **Fast reads and writes** - optimized for documents
- **Horizontal scalability** - grow with your data

<!--
Presenter Notes:
- Schema flexibility: No need to run ALTER TABLE migrations; just add new fields
- Example: Add a "preferences" object to user documents without touching existing records
- JSON is native to modern apps - no ORM impedance mismatch
- Direct mapping to JavaScript/Python/Java objects - what you code is what you store
- Performance: Documents stored together, reducing JOIN operations
- Scalability story: Add more nodes to scale out, not just scale up
- Contrast with relational: No rigid table structures, foreign keys, or complex schema migrations
- Real-world scenario: E-commerce product catalogs with varying attributes per category
-->

---

# **Architecture Overview**

## High-Level Design

![DocumentDB Architecture](img/documentdb.gif)

```

```
<!--
Presenter Notes:
- Gateway acts as a protocol translation layer between MongoDB clients and a PostgreSQL backend. 
- Gateway interprets MongoDB wire protocol, maps commands to PostgreSQL operations
- Gateway manages session handling, transactions, cursor-based paging, and TLS termination.
-->
---

# **Architecture Overview**

## Core Components

![w:560](img/core-components.svg)

<!--
Presenter Notes:
- Gateway: the adapter between MongoDB clients and PostgreSQL. Translates wire protocol, handles auth, TLS, and connection pooling.
- Extension: runs the MongoDB query language, CRUD, and aggregation inside PostgreSQL. This is where MongoDB semantics meet PostgreSQL storage.
- Core: the foundation. BSON encode/decode and document storage, shared by both layers above.
- Walk the diagram top to bottom: a request flows client to gateway to extension to core to PostgreSQL.
-->

---

# **Why Azure DocumentDB Exists**

## The Core Problems It Solves

### Licensing & Open Source

In 2018, MongoDB changed from AGPL to SSPL:
- Not OSI-approved
- Unacceptable for enterprises, governments, Linux distributions
- Creates legal/compliance risk for vendors and cloud providers

Teams built large applications around MongoDB's Query language, Drivers and Data model

**Result:** Organizations want MongoDB's API—not its licensing terms.

<!--
Presenter Notes:
- MongoDB's 2018 license change (AGPL to SSPL) created major issues for enterprises
- SSPL is not recognized as "open source" by OSI - this matters for procurement and compliance
- Many governments and regulated industries cannot use SSPL-licensed software
- Linux distributions (Red Hat, Debian, Ubuntu) removed MongoDB from their repos
- Legal departments flag SSPL as high-risk for SaaS companies
- The core insight: teams love MongoDB's developer experience but want freedom from MongoDB Inc.
-->

---

# **Why Azure DocumentDB Exists**

## The Core Problems It Solves

### Avoiding Vendor Lock-In

**Rewriting is expensive and risky.**

Azure DocumentDB offers:
- ✅ Zero or near-zero code changes
- ✅ Same MongoDB drivers
- ✅ Same queries
- ✅ Different backend (PostgreSQL)
- ✅ **Exit optionality**

<!--
Presenter Notes:
- "How many of you have built something on MongoDB? Now imagine your CTO says 'we need to switch databases.' That's the nightmare scenario — and it happens more than you'd think."
- "Once you've invested in MongoDB's query language, drivers, and data model, you're locked in. Rewriting thousands of queries isn't just expensive — it's risky. Every rewritten query is a potential bug."
- "What DocumentDB says is: keep your code, keep your drivers, keep your queries — just swap out the engine underneath. Zero or near-zero code changes."
- "Think of it like switching from one airline to another but keeping your frequent flyer miles. Same experience, different provider."
- "The 'exit optionality' point is huge for enterprise architecture reviews. When leadership asks 'what's our exit strategy?' — you have an answer."
- "This isn't just theoretical. Teams running on MongoDB Atlas who face pricing increases or compliance issues now have a real alternative without a rewrite."
-->

---

# **Why Azure DocumentDB Exists**

## The Core Problems It Solves

### Leveraging Mature Relational Databases

PostgreSQL already provides:
- ACID transactions
- Strong consistency
- Battle-tested durability
- Rich indexing
- Decades of operational tooling

**Azure DocumentDB = MongoDB ergonomics + PostgreSQL reliability**

<!--
Presenter Notes:
- Key message: why build a new storage engine when PostgreSQL already solved the hard problems?
- ACID transactions: guaranteed consistency — unlike eventual consistency in many NoSQL systems
- Strong consistency: reads always reflect the latest writes — critical for finance, healthcare, inventory
- Battle-tested: PostgreSQL has 30+ years of production hardening across every industry
- Rich indexing: B-tree, hash, GIN, GiST, BRIN — far beyond what most document DBs offer
- Operational tooling: pg_dump, pg_restore, replication, monitoring — your ops team already knows this
- The pitch: don't choose between developer experience and reliability — DocumentDB gives you both
-->

---

# **Demo 1: Setting Up DocumentDB Locally 🐳**

## Running DocumentDB in a Container

```bash
   # Pull the latest DocumentDB Docker image
   docker pull ghcr.io/documentdb/documentdb/documentdb-local:latest

   # Tag the image for convenience
   docker tag ghcr.io/documentdb/documentdb/documentdb-local:latest documentdb

   # Run the container with your chosen username and password
   docker run -dt -p 10260:10260 -p 5432:5432 --name documentdb-container documentdb --username admin --password DocDBPass123!
```

- Port **10260**: MongoDB-compatible gateway
- Port **5432**: PostgreSQL backend
- Ready in seconds!

<!--
Presenter Notes:
- Show the Docker pull and run commands live
- Highlight the two exposed ports: 10260 for MongoDB wire protocol, 5432 for PostgreSQL
- Emphasize how quick and easy local setup is — no complex installation
- Mention that the same container image works on any Docker-compatible environment
-->

---

# **Demo 2a: .NET Client App — Connect 🟣**

## Connecting to DocumentDB

```csharp
using MongoDB.Driver;
using MongoDB.Bson;

// Connect to DocumentDB gateway
var connectionString = Environment.GetEnvironmentVariable("DOCUMENTDB_CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "Set the DOCUMENTDB_CONNECTION_STRING environment variable.");
var settings = MongoClientSettings.FromConnectionString(connectionString);
settings.SslSettings = new SslSettings
{
    ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true
};
var client = new MongoClient(settings);

var db = client.GetDatabase("sampledb");
var collection = db.GetCollection<BsonDocument>("products");
```

**Standard MongoDB.Driver NuGet — zero DocumentDB-specific code!** ✨

<!--
Presenter Notes:
- Show the .NET console app running live against the local container
- Connection string comes from an environment variable — keeps credentials out of code
- SslSettings callback accepts the self-signed cert from the local container
- Emphasize: this is the standard MongoDB.Driver NuGet package — same code works against MongoDB
- No special SDK or driver needed — existing MongoDB drivers just work
- Database and collection objects are lightweight handles — no network call until you actually read or write
-->

---

# **Demo 2b: .NET Client App — Insert 🟣**

## Inserting Data

```csharp
var products = new[] {
    new BsonDocument { {"_id","prod-001"}, {"name","Laptop"},
                       {"price",1299.99}, {"category","electronics"} },
    new BsonDocument { {"_id","prod-002"}, {"name","Headphones"},
                       {"price",79.99}, {"category","electronics"} },
    new BsonDocument { {"_id","prod-003"}, {"name","Notebook"},
                       {"price",4.99}, {"category","office"} }
};

collection.InsertMany(products, new InsertManyOptions { IsOrdered = false });
```

- Database and collection are **created implicitly** on first insert
- `IsOrdered = false` — continues inserting even if one document fails (e.g., duplicate `_id`)

<!--
Presenter Notes:
- Walk through the InsertMany call — each BsonDocument is a flexible JSON-like object
- Explicit _id fields make inserts idempotent — re-running the demo won't create duplicates
- IsOrdered = false is a best practice for bulk inserts: skip duplicates, insert the rest
- The actual demo code also catches MongoBulkWriteException to report how many were inserted vs skipped
- Database and collection are created automatically on first insert — no need to pre-create them
- This is identical to how you'd insert into MongoDB — the code is fully portable
-->

---

# **Demo 3a: mongosh — Connect & Query 🔍**

## Connect to the Gateway

```bash
 docker exec -it documentdb-container mongosh "mongodb://admin:DocDBPass123!@localhost:10260/?tls=true&tlsAllowInvalidCertificates=true"
```

## Find Documents

```javascript
use sampledb

// Find electronics over $100
db.products.find({ category: "electronics", price: { $gt: 100 } })
```

**Standard MongoDB shell — no special syntax!** ✨

<!--
Presenter Notes:
- Show mongosh connecting to port 10260 — the MongoDB-compatible gateway
- The connection string uses TLS with tlsAllowInvalidCertificates for the self-signed cert
- Run the find query live — familiar MongoDB syntax, nothing DocumentDB-specific
- Emphasize: this is the same mongosh you'd use with any MongoDB instance
- The gateway translates these commands to PostgreSQL operations behind the scenes
-->

---

# **Demo 3a: mongosh — Aggregation 🔍**

## Aggregation Pipeline

```javascript
// Group by category and calculate average price
db.products.aggregate([
  { $group: { _id: { $toLower: "$category" }, avgPrice: { $avg: "$price" } } }
])
```

- Full **aggregation pipeline** support — `$group`, `$match`, `$project`, `$sort`, and more
- Same syntax you'd write against MongoDB Atlas or Community Edition

<!--
Presenter Notes:
- Walk through the aggregation pipeline stages — $group with $toLower and $avg
- Highlight that the aggregation framework is one of MongoDB's most powerful features — and it works here
- Mention other supported stages: $match, $project, $sort, $unwind, $lookup, etc.
- The gateway translates the entire pipeline into PostgreSQL execution plans
- This is where DocumentDB's PostgreSQL foundation shines — complex aggregations leverage PostgreSQL's mature query optimizer
-->

---

# **Demo 3b: PostgreSQL Queries via psql 🔍**

## Connect to the Backend

```bash
docker exec -it documentdb-container psql -U admin -d postgres -p 9712
```

## Run Queries

```sql
-- Same data, queried with SQL
SELECT document FROM documentdb_api.collection('sampledb', 'products') WHERE document @@ '{"category": {"$regex": "electronics", "$options": "i"}}';
```

**Same data, different query language!** ✨

<!--
Presenter Notes:
- Show psql connecting to port 9712 - the PostgreSQL backend
- Use docker exec to connect directly inside the container
- The same documents inserted via MongoDB API are queryable with SQL
- Key takeaway: the data is stored once but accessible through both interfaces
- MongoDB queries go through the gateway which translates to PostgreSQL operations
- PostgreSQL queries hit the backend directly — useful for DBAs and reporting
- This dual-interface approach gives teams flexibility: app developers use MongoDB, DBAs use SQL
-->

---

# **Why Vector & AI Matters 🤖**

## From Keywords to Meaning

- **LLMs and RAG** need a place to store and search **embeddings**
- Traditional queries match **exact keywords** — AI needs **semantic similarity**
- Embeddings turn text, images, and audio into **vectors of meaning**
- Every AI app must answer one question: *"What's most similar to this?"*

**Your operational data and your AI data don't have to live in two systems.**

<!--
Presenter Notes:
- This is the pivot slide — we move from "DocumentDB is a great Mongo workload database" to "DocumentDB is where your AI data lives too"
- RAG (Retrieval-Augmented Generation) is the dominant pattern for grounding LLMs in your own data
- The core mechanic: convert content into embeddings (vectors), then find the nearest vectors to a query
- Keyword search finds "laptop"; vector search finds "portable computer", "notebook PC", "MacBook" — meaning, not just text
- The big idea for the rest of the talk: you already store the documents here — store and search the vectors here too
- No separate vector database, no sync pipeline, no second system to operate — that's the selling point we'll prove
-->

---

# **Vector Index Algorithms 101 📐**

## How Do You Search Millions of Vectors Quickly?

Finding the **exact** nearest vector in millions of records is too slow. These algorithms trade a tiny bit of accuracy for massive speed gains — called **Approximate Nearest Neighbor (ANN)** search.

### **IVF** — Inverted File Index
Divides vectors into **clusters** (like zip codes). At query time, only searches the closest clusters instead of everything. Fast to build, but accuracy depends on hitting the right cluster.

<!--
Presenter Notes:
- Start with WHY: brute-force search is O(n) — checking every vector is impractical at scale
- ANN algorithms give you 95-99% accuracy at 100-1000x the speed
- IVF analogy: imagine sorting mail by zip code, then only checking the relevant zip codes
- IVF weakness: if your query is near a cluster boundary, you might miss nearby vectors in adjacent clusters
- IVF is the simplest ANN approach — good baseline, but largely superseded by HNSW and DiskANN
-->

---

# **Vector Index Algorithms 101 📐**

### **HNSW** — Hierarchical Navigable Small World
Builds a **multi-layer graph** of connections between vectors (think express lanes on a highway). Top layers have long-distance links for fast traversal; bottom layers have fine-grained links for precision. High accuracy, but keeps **everything in memory**.

### **DiskANN** — Disk-based ANN *(used by DocumentDB)*
Similar graph structure to HNSW, but stores the index **on SSD instead of RAM**. Uses product quantization to keep a compressed version in memory for fast navigation, then fetches full vectors from disk. **Scales to billions of vectors** at a fraction of the memory cost.

<!--
Presenter Notes:
- HNSW analogy: like an airport hub system — fly to a hub (top layer), then to regional (mid layer), then to local (bottom layer)
- HNSW strength: excellent recall and speed. Weakness: entire index must fit in RAM
- DiskANN is Microsoft Research's answer to HNSW's memory problem
- DiskANN keeps a small compressed index in RAM for routing, full vectors live on SSD
- At query time: navigate the compressed graph in memory, then do a single SSD read for the final candidates
- Result: comparable recall to HNSW at 10-100x lower memory cost
- DocumentDB supports both HNSW and DiskANN — choose based on your scale and budget
-->

---
# **Vector Search with DiskANN 🧠**

## What is DiskANN?

- **Disk-based Approximate Nearest Neighbor** search algorithm
- Developed by **Microsoft Research**
- Graph-structured index for **scalable vector search**
- Handles **billions of vectors** without requiring all data in memory
- Open source: [github.com/microsoft/DiskANN](https://github.com/microsoft/DiskANN)

### Key Innovation

Traditional vector indexes (HNSW, IVF) require data in RAM.
**DiskANN stores the index on SSD** — enabling massive scale at lower cost.

<!--
Presenter Notes:
- DiskANN stands for Disk-based Approximate Nearest Neighbor
- Born from Microsoft Research, now powers vector search across Azure services
- The core innovation: a graph-based index that works efficiently from SSD storage
- Traditional approaches like HNSW keep everything in memory — expensive at scale
- DiskANN achieves comparable recall and latency while using a fraction of the memory
- Published at NeurIPS 2019 - one of the top ML/AI conferences
- Open source on GitHub: github.com/microsoft/DiskANN
- Now rewritten in Rust for performance and safety
-->

---

# **Vector Search with DiskANN **

## Why DiskANN in DocumentDB?

### Integrated Vector Search — Not a Bolt-On

- Vectors stored **alongside your documents** — no separate vector DB
- **No data sync pipelines** to build and maintain
- Query vectors and documents in the **same query**
- Native support for **filtered vector search** (geo, text, numeric)
- Supports up to **16,000 dimensions** with product quantization

<!--
Presenter Notes:
- The killer feature: vectors and documents in ONE database — no separate Pinecone/Weaviate/Qdrant
- Data consistency: when you update a document, the vector index updates too
- No ETL pipelines: no need to sync data between your app DB and a vector DB
- Filtered search: combine vector similarity with geo, text, or numeric filters in one query
-->

---

# **Vector Search with DiskANN **

### Use Cases

- 🔍 **Semantic search** over product catalogs
- 🤖 **RAG** (Retrieval-Augmented Generation) for AI apps
- 🎯 **Recommendation engines** with contextual filtering
- 📍 **Location-aware similarity** (vector + geospatial)

<!--
Presenter Notes:
- Example: "Find similar products within 50 miles that are in stock"
- 16K dimensions supports modern embedding models (OpenAI, Cohere, etc.)
- RAG pattern: store documents + embeddings together, retrieve context for LLM prompts
- This is the convergence story: your operational DB IS your vector DB
-->

---

# **Azure DocumentDB vs Competitors**

| Feature | **DocumentDB** | **Pinecone** | **Weaviate** | **Qdrant** | **Milvus** | **pgvector** |
|---------|---------------|-------------|-------------|-----------|-----------|-------------|
| **Index** | DiskANN | Proprietary | HNSW | HNSW | Multiple | HNSW/IVF |
| **Self-Host** | ✅ | ❌ | ✅ | ✅ | ✅ | ✅ |
| **Scale** | 500K+ vectors | Large | Medium | Medium | Large | Small |
| **Memory** | Low (SSD) | Managed | High | High | High | High |
| **Filtered Search** | ✅ Native | ✅ | ✅ | ✅ | ✅ | ❌ Limited |
| **DB Integration** | DocumentDB | Vector-only | Vector-only | Vector-only | Vector-only | PostgreSQL extension |

<!--
Presenter Notes:
- DocumentDB's key differentiator: DiskANN index + integrated document database + MongoDB API
- Pinecone: fully managed but proprietary, no self-hosting, vendor lock-in, vectors only
- Weaviate: good open source option but requires separate infrastructure — another service to manage
- Qdrant: performant Rust-based engine, but standalone — not integrated with your document data
- Milvus: powerful but complex distributed system, steep operational learning curve
- pgvector: closest competitor on PostgreSQL, but lacks DiskANN's scale — limited to HNSW and IVF
- Only DocumentDB gives you: document store + vector search + MongoDB API + SQL access in one system
- No data duplication, no sync pipelines, no extra infrastructure
- DiskANN's SSD-based design means you don't need expensive high-memory instances
-->

---

# **Demo 4a: Vector Search with HNSW — Index 🔍**

## Creating an HNSW Vector Index

```csharp
var createIndex = new BsonDocument {
    { "createIndexes", "words" },
    { "indexes", new BsonArray {
        new BsonDocument {
            { "name", "vectorIndex" },
            { "key", new BsonDocument("embedding", "cosmosSearch") },
            { "cosmosSearchOptions", new BsonDocument {
                { "kind", "vector-hnsw" },     // Builds a fast in-memory graph for quick similarity lookups
                { "dimensions", 1536 },         // Vector length; must match your embedding model's output
                { "similarity", "COS" },        // Ranks matches by the angle between vectors, best for text
                { "m", 16 },                    // Neighbors each item links to; higher means more accurate but more memory
                { "efConstruction", 64 }        // Effort spent while building; higher means better quality but slower
            }}
        }
    }}
};
db.RunCommand<BsonDocument>(createIndex);
```

<!--
Presenter Notes:
- This demo uses the DocumentDbVectorDemo project — HNSW index on 1000 Bogus-generated big-box-store product words
- HNSW = Hierarchical Navigable Small World — a multi-layer graph that lives entirely in memory
- "kind": "vector-hnsw" is the key switch — compare with "vector-diskann" in the next demo
- dimensions: 1536 matches the OpenAI text-embedding-3-small model
- similarity: COS (cosine) is best for normalized text embeddings; IP for dot product, L2 for Euclidean
- m: controls graph connectivity — higher means better recall but more memory. 16 is a good default
- efConstruction: how many candidates are evaluated when building the index — higher means better quality, slower build
- The demo generates 1000 product words using Bogus (faker library) across 20 departments
-->

---

# **Demo 4b: Vector Search with HNSW — Search 🔍**

## Running a Similarity Search

```csharp
var searchPipeline = new[] {
    // $search — vector similarity search via the cosmosSearch index
    new BsonDocument("$search", new BsonDocument("cosmosSearch",
        new BsonDocument {
            { "path", "embedding" },    // The document field where each item's vector is stored
            { "vector", queryVector },  // Your search text as a vector, compared against the stored ones
            { "k", 10 }                // How many of the closest matches to return
        })),
    // $project — select which fields to include in results
    new BsonDocument("$project", new BsonDocument {
        { "word", 1 },                                          // Include the matched word in the results
        { "score", new BsonDocument("$meta", "searchScore") }   // How close the match is, from 0 (unrelated) to 1 (identical)
    }),
    // $sort — most similar results first
    new BsonDocument("$sort", new BsonDocument("score", -1))
};
var results = collection.Aggregate<BsonDocument>(searchPipeline).ToList();
```

<!--
Presenter Notes:
- The search pipeline is a standard MongoDB aggregation — three stages: $search, $project, $sort
- $search with cosmosSearch triggers the vector index — this is where the ANN magic happens
- "path" points to the field with stored embeddings, "vector" is the query embedding, "k" is top-k
- $project selects output fields: "word" for the text, "searchScore" meta for the similarity score
- $sort orders by score descending — most similar first
- Live demo: type a search term like "camping gear" and see semantically similar products ranked by score
- The SAME search pipeline works for both HNSW and DiskANN — only the index creation differs
-->

---

# **Demo 5a: Vector Search with DiskANN — Index 🔍**

## Creating a DiskANN Vector Index

```csharp
var createIndex = new BsonDocument {
    { "createIndexes", "products" },
    { "indexes", new BsonArray {
        new BsonDocument {
            { "name", "vectorIndex" },
            { "key", new BsonDocument("embedding", "cosmosSearch") },
            { "cosmosSearchOptions", new BsonDocument {
                { "kind", "vector-diskann" },   // Stores the index on SSD so it scales to billions of vectors
                { "dimensions", 1536 },          // Vector length; must match your embedding model's output
                { "similarity", "COS" },         // Ranks matches by the angle between vectors, best for text
                { "maxDegree", 32 },             // Neighbors each item links to; higher means more accurate but more disk
                { "lBuild", 64 },                // Effort spent while building; higher means better quality but slower
                { "lSearch", 40 }                // Candidates checked per query; higher means more accurate but slower
            }}
        }
    }}
};
db.RunCommand<BsonDocument>(createIndex);
```

<!--
Presenter Notes:
- This demo uses the DocumentDbDiskANNDemo project — DiskANN index on 1000 Bogus-generated product words
- Key difference from HNSW: "kind" is "vector-diskann" — the index lives on SSD, not in RAM
- Same dimensions, same similarity metric, same data — only the index algorithm changes
- DiskANN-specific parameters:
  - maxDegree (32): max neighbors per node in the graph — higher means better recall, more disk I/O
  - lBuild (64): candidate list during construction — higher means better index quality, slower build
  - lSearch (40): candidate list during search — higher means better recall, slower queries
- Compare with HNSW: m=16 and efConstruction=64 serve similar roles but for an in-memory graph
- DiskANN's advantage: handles billion-scale datasets where HNSW would run out of memory
- The collection name is "products" to keep data separate from the HNSW demo
-->

---

# **Demo 5b: Vector Search with DiskANN — Search 🔍**

## Running a Similarity Search

```csharp
var searchPipeline = new[] {
    new BsonDocument("$search", new BsonDocument("cosmosSearch",
        new BsonDocument {
            { "path", "embedding" },    // The document field where each item's vector is stored
            { "vector", queryVector },  // Your search text as a vector, compared against the stored ones
            { "k", 10 }                // How many of the closest matches to return
        })),
    new BsonDocument("$project", new BsonDocument {
        { "word", 1 },
        { "score", new BsonDocument("$meta", "searchScore") }
    }),
    new BsonDocument("$sort", new BsonDocument("score", -1))
};
var results = collection.Aggregate<BsonDocument>(searchPipeline).ToList();
```

**Same search pipeline as HNSW — swap the index, keep your queries!** ✨

<!--
Presenter Notes:
- The search pipeline is IDENTICAL to the HNSW demo — this is a key takeaway
- You choose your index algorithm at creation time, but your application code doesn't change
- This means you can benchmark HNSW vs DiskANN on the same data with zero code changes
- Live demo: search for the same term as the HNSW demo — compare results and scores
- In production, pick HNSW for small-to-medium datasets that fit in memory (fastest queries)
- Pick DiskANN when your dataset grows beyond available RAM or you need to optimize cost
- Both demos use Bogus to generate 1000 big-box-store product words across 20 departments
-->

---

# **Distributed Architecture 🌍**

## How DocumentDB Scales Out

Built on **Citus** (PostgreSQL's distributed extension):

```
         MongoDB Clients
              │
       ┌──────┴──────┐
       │   Gateway   │   ← MongoDB wire protocol
       └──────┬──────┘
       ┌──────┴──────┐
       │ Coordinator │   ← Routes queries, manages metadata
       └──┬───┬───┬──┘
          │   │   │
     ┌────┘   │   └────┐
     ▼        ▼        ▼
  Worker 1  Worker 2  Worker N   ← Each holds a subset of shards
```

<!--
Presenter Notes:
- DocumentDB's distributed layer is the pg_documentdb_distributed extension, built on Citus
- Citus is a proven PostgreSQL extension for horizontal sharding — used in production by thousands of companies
- Coordinator node handles routing and metadata; worker nodes store the actual document shards
- The gateway sits in front of the coordinator and handles MongoDB wire protocol translation
- Queries arrive as MongoDB commands, get translated to SQL, then the coordinator fans them out to workers
- Each worker is a standard PostgreSQL instance running the pg_documentdb extension
-->

---

# **Geo-Replication & Sharding 🌍**

## Key Distribution Concepts

- **Shard colocation** — related collections stay on the same node, reducing cross-node queries
- **Reference tables** — metadata (collections, indexes, roles) replicated to all nodes
- **Rebalancer** — redistributes shards when nodes are added or removed

<!--
Presenter Notes:
- Shard colocation ensures related data stays together — e.g., a user's documents and their indexes on the same worker
- Reference tables replicate metadata to every node so each worker can resolve collection names and indexes locally
- The rebalancer moves shards when you add or remove workers — keeps data evenly distributed
- These are all features of the open source pg_documentdb_distributed extension built on Citus
-->

---

# **Open Source vs Azure Managed 🌍**

## Choosing Your Deployment Model

| | **Open Source** | **Azure Managed** |
|---|---|---|
| **Horizontal scaling** | ✅ Multi-node Citus cluster | ✅ Fully managed |
| **Shard rebalancing** | ✅ Manual / automated | ✅ Automatic |
| **Multi-region geo-replication** | ❌ | ✅ Built-in |
| **Automatic failover** | ❌ | ✅ Built-in |

- Open source gives you **scale-out** within a datacenter
- Azure adds **global distribution** and **disaster recovery**

<!--
Presenter Notes:
- Important distinction: the open source version gives you horizontal scaling within a datacenter
- Full multi-region geo-replication with automatic failover is a premium feature of the managed Azure DocumentDB service
- Think of it as: open source gives you scale-out, Azure adds global distribution and disaster recovery
- For most dev/test and single-region production workloads, the open source version is more than sufficient
- You can start with open source locally, then migrate to Azure managed when you need multi-region
- The code and queries are the same — only the infrastructure changes
-->

---

# **Resources**

## Learn More

- 📚 **DocumentDB Docs:** [learn.microsoft.com/en-us/azure/documentdb/](https://learn.microsoft.com/en-us/azure/documentdb/)
- 💻 **DocumentDB GitHub:** [github.com/microsoft/documentdb](https://github.com/microsoft/documentdb)
- 🧠 **DiskANN GitHub:** [github.com/microsoft/DiskANN](https://github.com/microsoft/DiskANN)
- 🔍 **Vector Search Docs:** [DocumentDB Vector Search](https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/vcore/vector-search)
- 🐳 **Docker Image:** [mcr.microsoft.com/documentdb/documentdb](https://mcr.microsoft.com/documentdb/documentdb)
- 🎯 **Demo Code:** [src/ in this repo](https://github.com/desinole/azure-documentdb/tree/main/src/)

<!--
Presenter Notes:
- Pause here and let audience take a photo or note down the links
- Highlight the GitHub repo — all demo code from this talk is available there
- Mention the Docker image is the fastest way to get started locally
- Vector search docs are especially useful for anyone building AI/RAG applications
-->

---

# **Thank You! 🙏**

- **Name:** Santosh Hari
- **Role:** Azure EngOps
- **Connect:** 
  - LinkedIn: santoshhari
  - BlueSky: @santoshhari.dev
  - GitHub: desinole

<!--
Presenter Notes:
- Thank the audience for their time
- Open the floor for Q&A
- Remind them to connect on LinkedIn or BlueSky for follow-up questions
- If time permits, offer to show any demo again or dive deeper into a topic
-->

---

# **📲 Get the Talk Content**

![w:280](img/talk-qr.png)

**Everything from this talk (slides, demo code, and resources) is in this GitHub repo:**

### [github.com/desinole/azure-documentdb](https://github.com/desinole/azure-documentdb)

<!--
Presenter Notes:
- Final reminder: scan the QR code to grab all the slides, demo projects, and resources
- This is the same repo from the opening "Follow Along" slide; everything is there to try at home
-->


