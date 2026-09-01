---
marp: true
theme: default
paginate: true
backgroundColor: #fff
backgroundImage: url('https://marp.app/assets/hero-background.svg')
header: 'Azure DocumentDB: open source distributed database'
footer: '© 2026'
---

# **Azure DocumentDB**
## Azure DocumentDB is back: open source and AI-ready

### Santosh Hari

<!--
Presenter Notes:
- Welcome the audience and introduce yourself briefly
- Set context: this talk covers what Azure DocumentDB is, why it exists, and how to get started
- Mention the live demos and invite questions throughout
-->

---

# **Follow along 📲**

![w:280](img/talk-qr.png)

**All content from this talk (slides, demo code, and setup instructions) lives in this GitHub repo:**

### [github.com/desinole/azure-documentdb](https://github.com/desinole/azure-documentdb)

<!--
Presenter Notes:
- Scan the QR code now if you'd like to follow along with the slides and code as we go
- The repo contains the slides, demo projects, and setup steps
- The QR code appears again at the end
-->

---

# **Speaker introduction**

<!-- Add your information here -->

- **Name:** Santosh Hari
- **Role:** Azure EngOps
- **Connect:** 
  - LinkedIn: santoshhari
  - BlueSky: @santoshhari.dev
  - GitHub: desinole

<!--
Presenter Notes:
- Keep the personal intro brief; the audience is here for the technical content
- Mention relevant experience with Azure, databases, or open source if applicable
- Encourage attendees to connect afterwards for follow-up questions
-->

---

# **Agenda**

1. 🏗️ What is DocumentDB & why it matters
2. 🐳 **Demo:** Local setup, CRUD & querying
3. 🤖 Vector search & AI fundamentals
4. 🔍 **Demo:** HNSW in open source and DiskANN in Azure

<!--
Presenter Notes:
- Give the audience a quick map of the talk
- Point out the live demos
- Mention the talk will go from concepts to hands-on in about 45 minutes
- There are 2 vector search demos: HNSW in the open-source build and DiskANN in Azure DocumentDB
- The vector section is for people building AI and ML applications
-->

---

# **Introduction to Azure DocumentDB**

## What is Azure DocumentDB?

- **Open source** distributed document database (released 2025)
- ❌ 2017, ❌ AWS
- **MongoDB-compatible API** running on **PostgreSQL**
- Built for **cloud-native** applications
- Designed to **scale horizontally**
- <a href="https://desinole.github.io/azure-documentdb/glossary.html#acid" target="_blank"><strong>ACID transactions</strong></a> at document and collection level
- **Global distribution** capabilities

<!--
Presenter Notes:
- State the date plainly: Microsoft released this open source project in 2025
- MongoDB API compatibility runs on a PostgreSQL foundation
- Developers keep familiar MongoDB syntax and tools while PostgreSQL handles storage
- Use case: Teams who want MongoDB-style development but need PostgreSQL enterprise features
- Explain how ACID transactions keep related writes consistent
- Azure's managed service adds multi-region distribution
-->

---

# **Introduction to Azure DocumentDB**

## Why document databases?

- **Flexible schema** - add fields as the application changes
- **Natural data representation** - JSON documents
- **Developer-friendly model** - maps to application objects
- **Fast entity reads** - related data stays in one document
- **Horizontal scale** - spread data across nodes

<!--
Presenter Notes:
- Schema flexibility lets an application write new fields without an `ALTER TABLE` migration
- Example: Add a "preferences" object to user documents without touching existing records
- JSON maps directly to objects used by modern applications
- Direct mapping to JavaScript/Python/Java objects - what you code is what you store
- Performance: Documents stored together, reducing JOIN operations
- Add nodes when the dataset outgrows one machine
- Relational systems use tables and foreign keys; document systems group an entity in one document
- Example: An e-commerce catalog can store different attributes for laptops, shoes, and groceries
-->

---

# **Document vs relational: how data lives**

| Aspect | **MongoDB / DocumentDB** | **SQL Server** |
|---|---|---|
| **Stored as** | One <a href="https://desinole.github.io/azure-documentdb/glossary.html#bson" target="_blank"><strong>BSON</strong></a> document (nested JSON) | Rows in normalized tables |
| **Related data** | Embedded in the document | Split across tables via foreign keys |
| **Read back** | One read fetches the whole entity | `JOIN` tables to reassemble it |
| **Query with** | Find + <a href="https://desinole.github.io/azure-documentdb/glossary.html#aggregation-pipeline" target="_blank"><strong>aggregation</strong></a> | SQL `SELECT … JOIN … GROUP BY` |
| **Schema change** | Write new fields | `ALTER TABLE` migration |

**Same order:** one document vs `Orders` + `OrderItems` rows joined every read.

<!--
Presenter Notes:
- Give the room a concrete model; most people know SQL Server better than document databases
- MongoDB can keep an entity in one document; SQL Server often splits it across normalized tables
- In SQL Server, an order may span `Orders`, `OrderItems`, and `Customers`; a query joins those rows for each read
- In a document model, the order and its line items can live in one BSON document
- SQL Server rows follow a table schema defined by DDL; BSON documents can carry different fields
- A document read can fetch one entity in one lookup; a relational read may join several tables
- Normalization avoids duplication and suits cross-entity analytics; documents may duplicate data to speed up entity reads
- Adding a document field is a write. Adding a relational column usually requires a migration
- DocumentDB puts the MongoDB document model on a PostgreSQL engine
- Note: this compares the document model to the relational model generally; SQL Server does have a native JSON type and columnstore, but the row/table model is still its default storage unit
-->

---

# **Architecture overview**

## High-level design

![DocumentDB Architecture](img/documentdb.gif)

```

```
<!--
Presenter Notes:
- The gateway translates MongoDB wire-protocol requests into PostgreSQL operations
- It handles sessions, transactions, cursor paging, and TLS termination
-->
---

# **Architecture overview**

## Core components

![w:560](img/core-components.svg)

<!--
Presenter Notes:
- Gateway: translates the wire protocol and handles auth, TLS, and connection pooling
- Extension: runs MongoDB queries, CRUD, and aggregation inside PostgreSQL
- Core: handles BSON encoding, decoding, and document storage
- Walk the diagram top to bottom: a request flows client to gateway to extension to core to PostgreSQL.
-->

---

# **Why Azure DocumentDB exists**

## The problems it solves

### Licensing and open source

In 2018, MongoDB changed from AGPL to SSPL:
- Not OSI-approved
- Unacceptable for enterprises, governments, Linux distributions
- Creates legal/compliance risk for vendors and cloud providers

Teams built large applications around MongoDB's query language, drivers, and data model.

**Result:** Organizations want MongoDB's API under a permissive license.

<!--
Presenter Notes:
- MongoDB changed from AGPL to SSPL in 2018
- SSPL is not recognized as "open source" by OSI - this matters for procurement and compliance
- Many governments and regulated industries cannot use SSPL-licensed software
- Linux distributions (Red Hat, Debian, Ubuntu) removed MongoDB from their repos
- Legal departments flag SSPL as high-risk for SaaS companies
- Teams want the MongoDB development model without depending on MongoDB Inc.
-->

---

# **Why Azure DocumentDB exists**

## The problems it solves

### Avoiding vendor lock-in

**Rewriting is expensive and risky.**

Azure DocumentDB offers:
- ✅ Existing MongoDB drivers
- ✅ Familiar query syntax
- ✅ PostgreSQL backend
- ✅ **Another migration target**

<!--
Presenter Notes:
- Ask how much application code depends on MongoDB queries, drivers, and document shapes
- Rewriting thousands of queries costs time and creates another place for bugs
- DocumentDB keeps the familiar driver and query model on a PostgreSQL backend
- The enterprise architecture question is direct: "What is our exit plan?"
- Existing MongoDB workloads now have another migration target
-->

---

# **Why Azure DocumentDB exists**

## The problems it solves

### PostgreSQL foundation

PostgreSQL already provides:
- ACID transactions
- Strong consistency
- 30+ years of production use
- Rich indexing
- Decades of operational tooling

**Azure DocumentDB = MongoDB ergonomics + PostgreSQL reliability**

<!--
Presenter Notes:
- PostgreSQL already handles storage, transactions, recovery, and indexing
- ACID transactions keep multi-step writes consistent
- Strong consistency matters for finance, healthcare, and inventory
- PostgreSQL has more than 30 years of production use
- Its index types include B-tree, hash, GIN, GiST, and BRIN
- Existing tools include `pg_dump`, `pg_restore`, replication, and monitoring
- DocumentDB pairs the MongoDB development model with PostgreSQL storage
-->

---

# **Demo 1: Set up DocumentDB locally 🐳**

## Run DocumentDB in a container

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
- Runs in the background

<!--
Presenter Notes:
- Read the commands top to bottom: pull downloads the image, tag gives it a shorter local name, and run starts the container
- `-d` runs the container in the background, while `-t` allocates a terminal so the container starts as expected
- Each `-p host:container` argument forwards a port on the laptop to a service listening inside the container
- `--name documentdb-container` gives later `docker exec` commands a stable container name
- `--username` and `--password` create the local credentials used by mongosh and the application samples
- One-sentence example: "`-p 10260:10260` makes the MongoDB-compatible gateway available at localhost port 10260."
-->

---

# **Demo 2a: Connect from .NET 🟣**

## Connect to DocumentDB

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

**Uses the standard MongoDB.Driver NuGet package.** ✨

<!--
Presenter Notes:
- `GetEnvironmentVariable` reads the endpoint and credentials without storing secrets in source control
- `FromConnectionString` parses the host, port, authentication, and TLS options into driver settings
- The certificate callback accepts the self-signed certificate used by the local container; do not use this bypass for production endpoints
- `MongoClient` owns the connection pool; create one client and reuse it across operations
- `GetDatabase` and `GetCollection` return lightweight handles; the driver does not contact the server until an operation runs
- `BsonDocument` keeps the sample schema flexible and maps to the driver's document representation
- One-sentence example: "This code creates a reusable MongoDB client, then points it at the `sampledb.products` collection."
-->

---

# **Demo 2b: Insert from .NET 🟣**

## Insert data

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
- `IsOrdered = false` continues after one document fails (for example, a duplicate `_id`)

<!--
Presenter Notes:
- Each `BsonDocument` is one product, and its fields can differ from fields in other documents
- `_id` is the unique document key; predictable values expose duplicate records when the demo is rerun
- `InsertMany` sends all products in one bulk request
- `IsOrdered = false` tells the server to continue processing later documents after an individual write fails
- The full demo catches `MongoBulkWriteException`, which is how it reports inserted records separately from duplicate `_id` failures
- The first successful write creates the database and collection
- One-sentence example: "If `prod-001` already exists, the unordered batch can still insert `prod-002` and `prod-003`."
-->

---

# **Demo 3a: Connect and query with mongosh 🔍**

## Connect to the gateway

```bash
 docker exec -it documentdb-container mongosh "mongodb://admin:DocDBPass123!@localhost:10260/?tls=true&tlsAllowInvalidCertificates=true"
```

## Find documents

```javascript
use sampledb

// Find electronics over $100
db.products.find({ category: "electronics", price: { $gt: 100 } })
```

**Uses the standard MongoDB shell and query syntax.** ✨

<!--
Presenter Notes:
- `docker exec -it` opens an interactive shell command inside the running `documentdb-container`
- Port 10260 is the MongoDB-compatible gateway
- `tls=true` encrypts the connection, and `tlsAllowInvalidCertificates=true` accepts the local self-signed certificate only for this demo
- `use sampledb` selects the database before the query runs
- The filter combines an exact category match with `$gt`, meaning the price must be greater than 100
- The gateway receives normal MongoDB wire-protocol commands and translates them for the PostgreSQL-backed engine
- One-sentence example: "The laptop qualifies at $1,299.99; the $79.99 headphones fall below the price filter."
-->

---

# **Demo 3a: Aggregate with mongosh 🔍**

## Aggregation pipeline

```javascript
// Group by category and calculate average price
db.products.aggregate([
  { $group: { _id: { $toLower: "$category" }, avgPrice: { $avg: "$price" } } }
])
```

- <a href="https://desinole.github.io/azure-documentdb/glossary.html#aggregation-pipeline" target="_blank"><strong>Aggregation pipeline</strong></a> stages include `$group`, `$match`, `$project`, and `$sort`
- Same syntax you'd write against MongoDB Atlas or Community Edition

<!--
Presenter Notes:
- `aggregate` receives an ordered array of stages, with each stage consuming the output of the previous one
- `$group` creates one result document per category value
- Here, `_id` names each category group, and `$toLower` merges casing variations
- `$avg` reads every `price` in a group and writes the calculated value to `avgPrice`
- Stages such as `$match`, `$project`, and `$sort` can filter, reshape, and order the grouped results
- One-sentence example: "Laptop and Headphones are grouped under `electronics`, producing their average price as one result."
-->

---

# **Demo 3b: Query PostgreSQL with psql 🔍**

## Connect to the backend

```bash
docker exec -it documentdb-container psql -U admin -d postgres -p 9712
```

## Run a query

```sql
-- Same data, queried with SQL
SELECT document FROM documentdb_api.collection('sampledb', 'products') WHERE document @@ '{"category": {"$regex": "electronics", "$options": "i"}}';
```

**Same data, different query language!** ✨

<!--
Presenter Notes:
- `docker exec -it` runs the PostgreSQL client inside the existing container
- `-U admin` selects the database role, `-d postgres` selects the database, and `-p 9712` selects the internal PostgreSQL port
- `documentdb_api.collection('sampledb', 'products')` exposes the document collection as rows containing BSON documents
- The `@@` operator applies the MongoDB-style predicate on the right to each document on the left
- `$regex` searches the category field, while `$options: "i"` makes that comparison case-insensitive
- The data is stored once; MongoDB clients use the gateway while advanced PostgreSQL users can query the extension API directly
- One-sentence example: "This SQL statement returns products whose category matches `electronics`, regardless of capitalization."
-->

---

# **Why vector search matters 🤖**

## Search by meaning

- <a href="https://desinole.github.io/azure-documentdb/glossary.html#llm" target="_blank"><strong>LLMs</strong></a> and <a href="https://desinole.github.io/azure-documentdb/glossary.html#rag" target="_blank"><strong>RAG</strong></a> need a place to store and search <a href="https://desinole.github.io/azure-documentdb/glossary.html#embedding" target="_blank"><strong>embeddings</strong></a>
- Keyword queries match exact text; <a href="https://desinole.github.io/azure-documentdb/glossary.html#semantic-search" target="_blank"><strong>semantic search</strong></a> matches meaning
- Embeddings turn text, images, and audio into <a href="https://desinole.github.io/azure-documentdb/glossary.html#vector" target="_blank"><strong>vectors of meaning</strong></a>
- Vector search answers one question: *"What's most similar to this?"*

**Operational data and embeddings can live in one system.**

<!--
Presenter Notes:
- RAG grounds an LLM with data retrieved from your own collection
- Convert content into embeddings, then find the vectors nearest to the query vector
- A keyword search for "laptop" matches that word; vector search can also find "portable computer" and "notebook PC"
- DocumentDB stores the source document and its embedding together
- One database handles document queries and vector retrieval
-->

---

# **Key terms in 60 seconds 📖**

## The AI vocabulary for this talk

- <a href="https://desinole.github.io/azure-documentdb/glossary.html#embedding" target="_blank"><strong>Embedding:</strong></a> a list of numbers that captures the meaning of text, an image, or audio
- <a href="https://desinole.github.io/azure-documentdb/glossary.html#vector" target="_blank"><strong>Vector:</strong></a> that list of numbers; each number is a coordinate in high-dimensional space
- <a href="https://desinole.github.io/azure-documentdb/glossary.html#vector-search" target="_blank"><strong>Vector search:</strong></a> finds items whose vectors sit closest to the query
- <a href="https://desinole.github.io/azure-documentdb/glossary.html#semantic-search" target="_blank"><strong>Semantic search:</strong></a> matches meaning, so "couch" can find "sofa"
- <a href="https://desinole.github.io/azure-documentdb/glossary.html#vector-database" target="_blank"><strong>Vector database:</strong></a> stores embeddings and searches them at scale
- <a href="https://desinole.github.io/azure-documentdb/glossary.html#rag" target="_blank"><strong>RAG:</strong></a> gives an <a href="https://desinole.github.io/azure-documentdb/glossary.html#llm" target="_blank"><strong>LLM</strong></a> documents retrieved from your own data
- <a href="https://desinole.github.io/azure-documentdb/glossary.html#ann" target="_blank"><strong>ANN:</strong></a> approximate nearest neighbor search trades some recall for speed

### 👀 Today's demos

✅ **Embeddings + vector database + vector search:** open-source DocumentDB uses <a href="https://desinole.github.io/azure-documentdb/glossary.html#hnsw" target="_blank"><strong>HNSW</strong></a> (Demo 4); Azure DocumentDB adds <a href="https://desinole.github.io/azure-documentdb/glossary.html#diskann" target="_blank"><strong>DiskANN</strong></a> (Demo 5)

📌 **RAG & LLMs:** the demos cover retrieval; generation is outside this talk

<!--
Presenter Notes:
- Give everyone the vocabulary needed for the next 2 demos
- Keep this to 60 seconds and point to the glossary for details
- The sequence is text, embedding, vector search, retrieved document, then optional LLM generation
- The demos cover storage and retrieval
- Demo 4 uses HNSW in open-source DocumentDB. Demo 5 covers DiskANN in Azure DocumentDB
- ANN is the category that includes IVF, HNSW, and DiskANN
- If time is short, read the terms used in the demos and move on
- All terms link to the online glossary (glossary.html) for anyone following along on the repo
-->

---

# **Vector index algorithms 101 📐**

## Search millions of vectors quickly

An exact search checks every vector. <a href="https://desinole.github.io/azure-documentdb/glossary.html#ann" target="_blank"><strong>Approximate nearest neighbor (ANN)</strong></a> search checks a smaller candidate set and returns results faster.

### <a href="https://desinole.github.io/azure-documentdb/glossary.html#ivf" target="_blank"><strong>IVF:</strong></a> inverted file index
Divides vectors into **clusters** (like zip codes). A query checks the nearest clusters. Build time is short, but accuracy depends on choosing the right clusters.

<!--
Presenter Notes:
- A brute-force search is O(n) because it checks every vector
- ANN reduces the number of comparisons
- IVF works like mail sorted by zip code; the search checks nearby groups
- IVF weakness: if your query is near a cluster boundary, you might miss nearby vectors in adjacent clusters
- IVF is a useful baseline and is fast to build
-->

---

# **Vector index algorithms 101 📐**

### <a href="https://desinole.github.io/azure-documentdb/glossary.html#hnsw" target="_blank"><strong>HNSW:</strong></a> hierarchical navigable small world
Builds a **multi-layer graph** of connections between vectors (think express lanes on a highway). Top layers have long-distance links for fast traversal; bottom layers have fine-grained links for precision. The full graph stays **in memory**.

### <a href="https://desinole.github.io/azure-documentdb/glossary.html#diskann" target="_blank"><strong>DiskANN:</strong></a> disk-based ANN *(Azure DocumentDB only)*
DiskANN stores most of its graph **on SSD**. <a href="https://desinole.github.io/azure-documentdb/glossary.html#product-quantization" target="_blank">Product quantization</a> keeps a compressed routing structure in memory, then the search reads full vectors from disk. **It can handle billions of vectors.**

<!--
Presenter Notes:
- HNSW works like airline routing: hub, regional airport, then local airport
- HNSW strength: excellent recall and speed. Weakness: entire index must fit in RAM
- DiskANN is Microsoft Research's answer to HNSW's memory problem
- DiskANN keeps a small compressed index in RAM for routing, full vectors live on SSD
- At query time: navigate the compressed graph in memory, then do a single SSD read for the final candidates
- DiskANN uses less memory because most of the index stays on SSD
- Choose the index based on dataset size, memory, and latency targets
-->

---
# **Vector search with HNSW 🧠**

## HNSW in open-source DocumentDB

- Included in the **open-source DocumentDB** build
- Works with the local `documentdb-local` container
- Keeps its graph **in memory**
- Suits datasets whose index fits in available RAM
- Tune recall and build cost with `m` and `efConstruction`

### When to use it

Use HNSW when query latency matters and the vector index fits in memory.

<!--
Presenter Notes:
- HNSW is the vector index used by the open-source demo in this talk
- Each vector is a node connected to nearby vectors
- The graph stays in memory, which gives fast traversal
- `m` controls the number of graph connections per vector
- `efConstruction` controls how much work is spent building the graph
- Example: "A product catalog with an index that fits in RAM is a good HNSW workload."
-->

---
# **Vector search with DiskANN: Azure only 🧠**

## What is DiskANN?

- **Disk-based Approximate Nearest Neighbor** search algorithm
- Developed by **Microsoft Research**
- Graph index for <a href="https://desinole.github.io/azure-documentdb/glossary.html#vector-search" target="_blank"><strong>large vector collections</strong></a>
- Handles **billions of vectors** while keeping most data on SSD
- The DiskANN algorithm is open source: [github.com/microsoft/DiskANN](https://github.com/microsoft/DiskANN)

### DocumentDB availability

**DocumentDB's DiskANN index is Azure-only today.**

Open-source DocumentDB and `documentdb-local` provide HNSW and IVF.

<!--
Presenter Notes:
- DiskANN stands for Disk-based Approximate Nearest Neighbor
- Microsoft Research created DiskANN
- The DiskANN algorithm has an open-source repository
- Its DocumentDB index implementation is currently available through Azure DocumentDB
- Open-source DocumentDB provides HNSW and IVF; Azure DocumentDB adds DiskANN
- Its graph index reads candidates from SSD
- HNSW keeps its graph in memory
- DiskANN achieves comparable recall and latency while using a fraction of the memory
- Published at NeurIPS 2019 - one of the top ML/AI conferences
-->

---

# **Vector search with DiskANN: Azure only**

## Why DiskANN in DocumentDB?

### Vectors and documents together

- Store vectors **alongside your documents**
- Keep document and vector updates in one write path
- Query vectors and documents in the **same query**
- Native support for **filtered vector search** (geo, text, numeric)
- Supports up to <a href="https://desinole.github.io/azure-documentdb/glossary.html#dimensions" target="_blank"><strong>16,000 dimensions</strong></a> with product quantization

<!--
Presenter Notes:
- Documents and vectors live in one database
- Data consistency: when you update a document, the vector index updates too
- One write path removes the need for a separate vector sync job
- Filtered search: combine vector similarity with geo, text, or numeric filters in one query
-->

---

# **Vector search with DiskANN: Azure only**

### Use cases

- 🔍 **Semantic search** over product catalogs
- 🤖 **RAG** (Retrieval-Augmented Generation) for AI apps

- 🎯 **Recommendation engines** with contextual filtering
- 📍 **Location-aware similarity** (vector + geospatial)

<!--
Presenter Notes:
- Example: "Find similar products within 50 miles that are in stock"
- 16K dimensions supports modern embedding models (OpenAI, Cohere, etc.)
- RAG pattern: store documents + embeddings together, retrieve context for LLM prompts
- The same database stores application records and embeddings
-->

---

# **Azure DocumentDB compared with vector databases**

| Feature | **DocumentDB** | **Pinecone** | **Weaviate** | **Qdrant** | **Milvus** | **pgvector** |
|---------|---------------|-------------|-------------|-----------|-----------|-------------|
| **Index** | HNSW (open source) + DiskANN (Azure) | Closed source | HNSW | HNSW | Multiple | HNSW/IVF |
| **Self-Host** | ✅ | ❌ | ✅ | ✅ | ✅ | ✅ |
| **Scale** | 500K+ vectors | Large | Medium | Medium | Large | Small |
| **Memory** | Low (SSD) | Managed | High | High | High | High |
| **Filtered Search** | ✅ Native | ✅ | ✅ | ✅ | ✅ | ❌ Limited |
| **Data storage** | Documents + vectors | Vectors | Vectors | Vectors | Vectors | PostgreSQL tables |

<!--
Presenter Notes:
- Open-source DocumentDB supports HNSW; Azure DocumentDB adds DiskANN
- Pinecone is managed, closed source, and focused on vector workloads
- Weaviate runs as a separate service
- Qdrant is a standalone Rust service
- Milvus is a distributed vector database with more moving parts to operate
- pgvector adds HNSW and IVF to PostgreSQL
- DocumentDB keeps documents and vectors in one database
- One write path keeps the document and embedding together
- DiskANN's SSD-based design lowers memory requirements
-->

---

# **Demo 4a: Create an HNSW index 🔍**

## Create an HNSW vector index

```csharp
var createIndex = new BsonDocument {
    { "createIndexes", "words" },
    { "indexes", new BsonArray {
        new BsonDocument {
            { "name", "vectorIndex" },
            { "key", new BsonDocument("embedding", "cosmosSearch") },
            { "cosmosSearchOptions", new BsonDocument {
                { "kind", "vector-hnsw" },     // Builds an in-memory graph for similarity lookups
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
- `createIndexes` names the collection that will receive the index, and the `indexes` array contains the index definitions to create
- `name` is the identifier used to inspect or remove the index later
- The `embedding: "cosmosSearch"` key marks the embedding field as a vector index
- `kind: "vector-hnsw"` selects the in-memory HNSW graph supported by the open-source project
- `dimensions: 1536` must exactly match the number of values produced by `text-embedding-3-small`
- `similarity: "COS"` ranks vectors by cosine similarity. `IP` uses inner product, and `L2` uses Euclidean distance
- `m: 16` allows each graph node to keep up to 16 nearby connections; increasing it can improve recall but uses more memory
- `efConstruction: 64` controls how many candidates are considered while building the graph; increasing it improves index quality but takes longer
- `RunCommand` sends the completed BSON command to the database for execution
- One-sentence example: "A 1,536-number product embedding is added to an HNSW graph with up to 16 neighbor links."
-->

---

# **Demo 4b: Search an HNSW index 🔍**

## Run a similarity search

```csharp
var searchPipeline = new[] {
    // $search: vector similarity search through the cosmosSearch index
    new BsonDocument("$search", new BsonDocument("cosmosSearch",
        new BsonDocument {
            { "path", "embedding" },    // Field that stores each item's vector
            { "vector", queryVector },  // Search text converted to a vector
            { "k", 10 }                // Number of nearest matches to return
        })),
    // $project: select fields for the result
    new BsonDocument("$project", new BsonDocument {
        { "word", 1 },                                          // Include the matched word in the results
        { "score", new BsonDocument("$meta", "searchScore") }   // Similarity score for the match
    }),
    // $sort: highest score first
    new BsonDocument("$sort", new BsonDocument("score", -1))
};
var results = collection.Aggregate<BsonDocument>(searchPipeline).ToList();
```

<!--
Presenter Notes:
- The pipeline runs vector retrieval first, then shapes the output, and finally orders the displayed results
- `$search` with `cosmosSearch` tells DocumentDB to use the vector index
- `path: "embedding"` identifies the field containing each stored vector
- `vector: queryVector` supplies the embedding generated from the audience's search text
- `k: 10` limits retrieval to the ten nearest candidates
- `$project` returns the human-readable `word` and exposes the computed similarity as `searchScore`
- `$sort` places the highest score first, and `Aggregate(...).ToList()` executes the pipeline and materializes the results
- One-sentence example: "Searching for `camping gear` converts that phrase to a vector and returns the ten closest product words."
-->

---

# **Demo 5a: Create a DiskANN index in Azure 🔍**

## Create a DiskANN vector index

```csharp
var createIndex = new BsonDocument {
    { "createIndexes", "products" },
    { "indexes", new BsonArray {
        new BsonDocument {
            { "name", "vectorIndex" },
            { "key", new BsonDocument("embedding", "cosmosSearch") },
            { "cosmosSearchOptions", new BsonDocument {
                { "kind", "vector-diskann" },   // Stores most of the index on SSD
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
- Scope note: DocumentDB's DiskANN index is available through the managed Azure DocumentDB service
- `createIndexes` targets the `products` collection, and `name` gives the index a stable identifier
- `embedding: "cosmosSearch"` marks the field as vector-searchable
- `kind: "vector-diskann"` asks the managed service to build its SSD-oriented DiskANN index
- `dimensions: 1536` must match every stored vector and the query vector
- `similarity: "COS"` uses cosine similarity to compare the direction of two embedding vectors
- `maxDegree: 32` limits graph connections per node; a larger value can improve recall while increasing storage and traversal work
- `lBuild: 64` controls candidate exploration during construction; a larger value builds a stronger graph more slowly
- `lSearch: 40` controls candidates examined per query; a larger value can improve recall at the cost of latency
- One-sentence example: "With `lSearch` set to 40, the query explores more candidates than a setting of 20 before choosing its nearest matches."
-->

---

# **Demo 5b: Search a DiskANN index in Azure 🔍**

## Run a similarity search

```csharp
var searchPipeline = new[] {
    new BsonDocument("$search", new BsonDocument("cosmosSearch",
        new BsonDocument {
            { "path", "embedding" },    // Field that stores each item's vector
            { "vector", queryVector },  // Search text converted to a vector
            { "k", 10 }                // Number of nearest matches to return
        })),
    new BsonDocument("$project", new BsonDocument {
        { "word", 1 },
        { "score", new BsonDocument("$meta", "searchScore") }
    }),
    new BsonDocument("$sort", new BsonDocument("score", -1))
};
var results = collection.Aggregate<BsonDocument>(searchPipeline).ToList();
```

**The HNSW and DiskANN demos use the same search pipeline.** ✨

<!--
Presenter Notes:
- Scope note: run this comparison against the managed Azure DocumentDB service
- The query shape matches the HNSW example because both indexes accept the same path, query vector, and top-k request
- `path` selects the stored embedding, `vector` supplies the query embedding, and `k` requests ten candidates
- `$project` returns the product word and score, leaving out the large embedding array
- `$sort` makes the comparison easy to read by displaying the strongest match first
- Reusing one query shape keeps the demo focused on index tradeoffs
- One-sentence example: "Run `camping gear` against both indexes and compare the returned words and search time."
-->

---

# **Distributed architecture 🌍**

## How DocumentDB scales out

Built on PostgreSQL's distributed extension:

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
- Citus is a PostgreSQL extension for horizontal sharding
- Coordinator node handles routing and metadata; worker nodes store the actual document shards
- The gateway sits in front of the coordinator and handles MongoDB wire protocol translation
- Queries arrive as MongoDB commands, get translated to SQL, then the coordinator fans them out to workers
- Each worker is a standard PostgreSQL instance running the pg_documentdb extension
-->

---

# **<a href="https://desinole.github.io/azure-documentdb/glossary.html#geo-replication" target="_blank">Geo-replication</a> and <a href="https://desinole.github.io/azure-documentdb/glossary.html#sharding" target="_blank">sharding</a> 🌍**

## Distribution concepts

- **Shard colocation:** related collections stay on the same node, reducing cross-node queries
- **Reference tables:** metadata (collections, indexes, roles) is copied to all nodes
- **Rebalancer:** moves shards when nodes are added or removed

<!--
Presenter Notes:
- Shard colocation keeps related data on one worker, such as a user's documents and indexes
- Reference tables replicate metadata to every node so each worker can resolve collection names and indexes locally
- The rebalancer spreads shards after workers are added or removed
- These are all features of the open source pg_documentdb_distributed extension built on Citus
-->

---

# **Open source compared with Azure managed 🌍**

## Choose a deployment model

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
- The open source version runs in infrastructure you operate
- The managed Azure service handles multi-region replication and automatic failover
- Local development can start with the open source container
- Teams can evaluate the managed service when they need Azure operations and multi-region support
- MongoDB drivers and query syntax carry across both deployment models
-->

---

# **Resources**

## Learn more

- 📚 **DocumentDB Docs:** [learn.microsoft.com/en-us/azure/documentdb/](https://learn.microsoft.com/en-us/azure/documentdb/)
- 💻 **DocumentDB GitHub:** [github.com/microsoft/documentdb](https://github.com/microsoft/documentdb)
- 🧠 **DiskANN GitHub:** [github.com/microsoft/DiskANN](https://github.com/microsoft/DiskANN)
- 🔍 **Vector Search Docs:** [DocumentDB Vector Search](https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/vcore/vector-search)
- 🐳 **Docker Image:** [mcr.microsoft.com/documentdb/documentdb](https://mcr.microsoft.com/documentdb/documentdb)
- 🎯 **Demo Code:** [src/ in this repo](https://github.com/desinole/azure-documentdb/tree/main/src/)

<!--
Presenter Notes:
- Pause here and let audience take a photo or note down the links
- Point to the GitHub repo, which contains the demo code
- Mention the Docker image is the fastest way to get started locally
- The vector search docs cover the APIs used by AI and RAG applications
-->

---

# **Thank you! 🙏**

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
- If time permits, show a demo again or answer a technical question
-->

---

# **📲 Get the talk content**

![w:280](img/talk-qr.png)

**The GitHub repo contains the slides, demo code, and resources:**

### [github.com/desinole/azure-documentdb](https://github.com/desinole/azure-documentdb)

<!--
Presenter Notes:
- Final reminder: scan the QR code to grab all the slides, demo projects, and resources
- This is the repo shown on the opening "Follow along" slide
-->

---

# **Appendix: glossary 📖**

## Plain-English definitions for the AI, vector, and database terms in this talk

Use these reference slides after the talk, or read them online at
### [desinole.github.io/azure-documentdb/glossary.html](https://desinole.github.io/azure-documentdb/glossary.html)

<!--
Presenter Notes:
- This appendix is for people reviewing the deck after the talk
- All the same definitions are hosted online at glossary.html, linked throughout the talk
- Point folks here if they ask "what did that term mean again?"

- AI terms in 30 seconds:
  Computers compare numbers. An embedding turns a sentence, image, or audio clip into a vector. Similar content produces nearby vectors, and cosine similarity measures that distance. Vector search retrieves the nearest documents. RAG sends those documents to an LLM as context. ANN indexes reduce the number of vectors checked. IVF uses clusters, HNSW uses an in-memory graph, and DiskANN keeps most of its graph on SSD.
-->

---

# **Glossary: embeddings and vectors 📖**

- **Embedding:** a list of numbers that captures the meaning of text, an image, or audio. Similar content produces similar embeddings.
- **Vector:** an ordered list of numbers. In AI, an embedding *is* a vector; each number is a coordinate in high-dimensional space.
- **Dimensions:** how many numbers a vector holds. OpenAI's text-embedding-3-small uses 1,536.
- **Cosine similarity:** measures the angle between 2 vectors. Near 1 means very similar; near 0 means unrelated.

<!--
Presenter Notes:
- Foundation terms: text becomes an embedding (a vector of N dimensions)
- Cosine similarity is the most common way vector search scores "closeness"
-->

---

# **Glossary: search and retrieval 📖**

- **Vector search:** finds the items whose vectors sit closest to a query vector.
- **Semantic search:** matches on meaning. "couch" can return "sofa" because their embeddings are close.
- **Vector database:** stores embeddings and searches them at scale. DocumentDB stores vectors with their source documents.
- **LLM (large language model):** a model trained on large text collections to understand and generate language.
- **RAG (retrieval-augmented generation):** gives an LLM documents retrieved from your data before it answers.

<!--
Presenter Notes:
- RAG = retrieval (vector search) + generation (the LLM); DocumentDB provides the vector database piece
- Semantic search is the user-facing payoff of vector search
-->

---

# **Glossary: index algorithms 📖**

- **ANN (approximate nearest neighbor):** checks a subset of records to return results faster.
- **IVF (inverted file index):** groups vectors into clusters; a query searches the nearest clusters.
- **HNSW (hierarchical navigable small world):** a multi-layer graph kept in memory.
- **DiskANN:** a graph-based ANN from Microsoft Research that stores most of its index on SSD.
- **Product quantization:** compresses vectors into a small code used during search.

<!--
Presenter Notes:
- ANN is the category; IVF, HNSW, and DiskANN are different implementations
- Demos 4 and 5 compare an in-memory graph with an SSD-based graph
- Product quantization is what makes DiskANN's memory footprint small
-->

---

# **Glossary: documents and transactions 📖**

- **BSON:** binary JSON, the document format used by MongoDB and DocumentDB.
- **ACID:** atomicity, consistency, isolation, and durability. These properties keep a transaction correct when a step fails.
- **Aggregation pipeline:** runs documents through stages such as filter, group, sort, and reshape.
- **Recall:** the share of true nearest neighbors returned by a search.

<!--
Presenter Notes:
- BSON, ACID, and the aggregation pipeline came up in the CRUD and query demos
- Recall is how you measure whether your ANN index is accurate enough
-->

---

# **Glossary: distribution 📖**

- **Wire protocol:** the network format MongoDB drivers use to talk to a server.
- **Sharding:** splits a collection across nodes using a shard key.
- **Geo-replication:** copies data to other regions for local reads and regional recovery.

<!--
Presenter Notes:
- The MongoDB wire protocol lets existing drivers connect to DocumentDB
- Sharding and geo-replication cover the distributed-scale and resilience story from the architecture section
-->
