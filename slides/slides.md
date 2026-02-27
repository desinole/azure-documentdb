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
## The New Open Source Distributed Database

### Santosh Hari

<!--
Presenter Notes:
- Welcome the audience and introduce yourself briefly
- Set context: this talk covers what Azure DocumentDB is, why it exists, and how to get started
- Mention that there will be live demos — encourage questions throughout
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

1. 📌 Introduction to Azure DocumentDB
2. 🏗️ Architecture Overview
3. 🎯 Why DocumentDB? Why Now?
4. 🆚 Why SQL Server / DBAs Should Care
5. 🐳 **Demo:** Local Setup, CRUD, Querying
6. 📐 Vector Index Algorithms (IVF, HNSW, DiskANN)
7. 🧠 Vector Search & Competitor Comparison
8. 🔍 **Demo:** Vector Search with DiskANN

<!--
Presenter Notes:
- Walk through the agenda quickly — give audience a roadmap of what to expect
- Highlight that there are multiple live demos throughout the talk
- Mention the talk will go from concepts to hands-on in about 45 minutes
- Note: vector search section is especially relevant for anyone building AI/ML applications
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

### **pg_documentdb_gw** (Gateway)
MongoDB wire protocol handler and request router

### **pg_documentdb** (Extension)
Core MongoDB-compatible functionality in PostgreSQL

### **pg_documentdb_core** (Foundation)
Low-level BSON processing and document operations

<!--
Presenter Notes:

**pg_documentdb_gw (Gateway Layer)**
- Entry point for all MongoDB client connections
- Handles MongoDB wire protocol translation
- Manages connection pooling and load balancing
- Performs TLS termination and authentication
- Routes requests to appropriate PostgreSQL instances
- Think of it as the "adapter" between MongoDB clients and PostgreSQL backend

**pg_documentdb (PostgreSQL Extension)**
- The main extension loaded into PostgreSQL
- Implements MongoDB query language and commands
- Provides collection management (create, drop, list)
- Handles CRUD operations (insert, find, update, delete)
- Implements aggregation pipeline
- Manages indexes and query optimization
- This is where MongoDB semantics meet PostgreSQL storage

**pg_documentdb_core (Core Library)**
- Foundational layer for document processing
- BSON (Binary JSON) encoding/decoding
- Document validation and schema enforcement
- Low-level data type conversions
- Efficient document storage structures
- Shared library used by both gateway and extension
- Performance-critical path for all document operations
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
<style>
  .columns {
    display: flex;
    height: 80%; /* Adjust height as needed */
    justify-content: space-evenly;
    align-items: center;
  }
  .column {
    flex: 1;
    padding: 0 20px; /* Add some spacing */
  }
</style>
# **Why Azure DocumentDB Exists**

## The Core Problems It Solves

### A Truly Open Alternative

<div class="columns">
  <div class="column">
    Azure DocumentDB is:

    - Apache 2.0 licensed
    
    - Community-driven
    
    - Cloud-neutral
    
    - Vendor-agnostic
  </div>
  <div class="column">
    This matters for:

    - Governments
    
    - Regulated industries
    
    - Linux distributions
    
    - Companies building database platforms
  </div>
</div>
<!--
Presenter Notes:
- "Let's talk about what 'open source' actually means here. Apache 2.0 isn't just a label — it's a promise. You can fork it, modify it, build a product on it, sell it. No gotchas."
- "Compare that to MongoDB's SSPL license. The short version: if you offer MongoDB as a service, you have to open source your entire stack. That's why AWS, Google, and every Linux distro walked away."
- "For anyone in government or regulated industries — your legal team will love Apache 2.0. SSPL is a procurement nightmare. I've seen MongoDB deployments blocked for months by legal review."
- "Cloud-neutral is a big deal too. You can run DocumentDB on Azure, AWS, GCP, or in your own data center. Try doing that with MongoDB Atlas — you're renting, not owning."
- "Think about it from a platform builder's perspective. If you're building a database-as-a-service, you can embed DocumentDB without worrying about license violations. That's not possible with SSPL."
- "The community angle matters long-term. With MongoDB, one company controls the roadmap. With DocumentDB on Apache 2.0, if Microsoft stops investing, the community can carry it forward. That's real open source insurance."
-->

---


# **Why SQL Server / DBAs Should Care**

## A Strategic Shift in Database Architecture

From a SQL pro's perspective, DocumentDB represents a larger trend:

- **The document model is here to stay**
- **MongoDB's API has become a de facto standard**
- **Teams want JSON + ACID + portability**
- **PostgreSQL keeps absorbing new workloads**
- **Compatibility layers are strategic architectural tools**

<!--
Presenter Notes:
- "Okay, show of hands — how many of you are SQL Server DBAs or come from a relational background? This slide is for you."
- "I'm not here to tell you SQL Server is dead. Far from it. But the world your developers live in has changed. They're building APIs that think in JSON, not in rows and columns."
- "Here's the uncomfortable truth: when developers can't get flexible schemas from the DBA team fast enough, they go rogue and spin up a MongoDB instance. DocumentDB gives you a way to meet them in the middle — document model with relational reliability underneath."
- "MongoDB's query language has become the SQL of the document world. Whether you like it or not, your junior devs probably know MongoDB syntax better than T-SQL. That's the reality."
- "PostgreSQL is quietly eating the database world. It started as relational, then added JSON, then time-series, then vector search. It's becoming the operating system of data. DocumentDB rides that wave."
- "The career angle is real: DBAs who can speak both relational AND document are unicorns in the job market. This isn't about replacing what you know — it's about adding to your toolkit."
-->

---

# **Why SQL Server / DBAs Should Care**

## Part of a Broader Movement

DocumentDB is part of the same movement as:

- **Azure DocumentDB** - MongoDB API on PostgreSQL
- **FerretDB** - Open source MongoDB compatibility
- **PostgreSQL JSONB adoption** - Native JSON handling
- **"API-first databases"** - Protocol compatibility over native implementation

<!--
Presenter Notes:
- "This isn't happening in isolation. There's a whole movement of projects saying: 'let's keep PostgreSQL as the engine and put different APIs on top.'"
- "FerretDB is doing the same thing as DocumentDB — MongoDB compatibility on Postgres. The fact that multiple projects are doing this tells you the demand is real."
- "JSONB in PostgreSQL is already huge. Half the new Postgres schemas I see have at least one JSONB column. Developers want flexible fields without ALTER TABLE."
- "The 'API-first' pattern is powerful. Instead of forcing everyone to learn a new query language, you meet them where they are. MongoDB devs get MongoDB syntax. SQL devs get SQL. Same data."
- "For the architects in the room: this is how you avoid the 'best tool for the job' sprawl. Instead of running 5 different databases, you run PostgreSQL with different interfaces."
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

# **Demo 2: .NET Client App 🟣**

## Creating Database, Collection & Inserting Data

```csharp
using MongoDB.Driver;
using MongoDB.Bson;

// Connect to DocumentDB gateway
var client = new MongoClient("mongodb://admin:YourPassword123!@localhost:10260");

// Create database and collection
var db = client.GetDatabase("demo_db");
var collection = db.GetCollection<BsonDocument>("products");

// Insert documents
collection.InsertMany(new[] {
    new BsonDocument { {"name","Laptop"}, {"price",1299.99}, {"category","electronics"} },
    new BsonDocument { {"name","Headphones"}, {"price",79.99}, {"category","electronics"} },
    new BsonDocument { {"name","Notebook"}, {"price",4.99}, {"category","office"} }
});
```

**Standard MongoDB.Driver NuGet — zero DocumentDB-specific code!** ✨

<!--
Presenter Notes:
- Show the .NET console app running live against the local container
- Emphasize that this is the standard MongoDB.Driver NuGet package — the same code works against MongoDB
- No special SDK or driver needed — existing MongoDB drivers just work
- Database and collection are created implicitly on first insert
- Point out the connection string uses the gateway port (10260)
-->

---

# **Demo 3a: MongoDB Queries via mongosh 🔍**

## Connect to the Gateway

```bash
mongosh "mongodb://admin:DocDBPass123!@localhost:10260/?tls=true&tlsAllowInvalidCertificates=true"
```

## Run Queries

```javascript
use sampledb

// Find electronics over $100
db.products.find({ category: "electronics", price: { $gt: 100 } })

// Aggregation pipeline
db.products.aggregate([
  { $group: { _id: { $toLower: "$category" }, avgPrice: { $avg: "$price" } } }
])
```

**Standard MongoDB shell — no special syntax!** ✨

<!--
Presenter Notes:
- Show mongosh connecting to port 10260 — the MongoDB-compatible gateway
- The connection string uses TLS with tlsAllowInvalidCertificates for the self-signed cert
- Run familiar MongoDB queries — find, aggregate, etc.
- Emphasize: this is the same mongosh you'd use with any MongoDB instance
- The gateway translates these commands to PostgreSQL operations behind the scenes
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
SELECT document FROM documentdb_api.collection('sampledb', 'products')
WHERE document @? '{"category": {"$regex": "electronics", "$options": "i"}}';
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

# **Vector Index Algorithms 101 📐**

## How Do You Search Millions of Vectors Quickly?

Finding the **exact** nearest vector in millions of records is too slow. These algorithms trade a tiny bit of accuracy for massive speed gains — called **Approximate Nearest Neighbor (ANN)** search.

### **IVF** — Inverted File Index
Divides vectors into **clusters** (like zip codes). At query time, only searches the closest clusters instead of everything. Fast to build, but accuracy depends on hitting the right cluster.

### **HNSW** — Hierarchical Navigable Small World
Builds a **multi-layer graph** of connections between vectors (think express lanes on a highway). Top layers have long-distance links for fast traversal; bottom layers have fine-grained links for precision. High accuracy, but keeps **everything in memory**.

### **DiskANN** — Disk-based ANN *(Microsoft Research)*
Similar graph approach to HNSW, but engineered to work from **SSD storage** instead of RAM. Achieves comparable accuracy at a **fraction of the memory cost** — enabling billion-scale vector search.

<!--
Presenter Notes:
- Start with WHY: brute-force search is O(n) — checking every vector is impractical at scale
- ANN algorithms give you 95-99% accuracy at 100-1000x the speed
- IVF analogy: imagine sorting mail by zip code, then only checking the relevant zip codes
- IVF weakness: if your query is near a cluster boundary, you might miss nearby vectors in adjacent clusters
- HNSW analogy: like an airport hub system — fly to a hub (top layer), then to regional (mid layer), then to local (bottom layer)
- HNSW strength: excellent recall and speed. Weakness: entire index must fit in RAM
- DiskANN innovation: Microsoft Research figured out how to build a similar graph that pages efficiently from SSD
- DiskANN means you can index 1 billion vectors on a machine with 64GB RAM instead of needing 1TB+
- This is why DocumentDB chose DiskANN — it scales to real-world datasets without breaking the bank
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

# **Demo 4: Vector Search with DiskANN 🔍**

## .NET Vector Index Example

```csharp
// Create a DiskANN vector index
var createIndex = new BsonDocument("createIndexes", "products");
createIndex.Add("indexes", new BsonArray {
    new BsonDocument {
        { "name", "vectorIndex" },
        { "key", new BsonDocument("embedding", "cosmosSearch") },
        { "cosmosSearchOptions", new BsonDocument {
            { "kind", "vector-diskann" },
            { "dimensions", 3 },
            { "similarity", "COS" }
        }}
    }
});
db.RunCommand<BsonDocument>(createIndex);
```

<!--
Presenter Notes:
- Show creating a DiskANN index — just a few lines of configuration
- kind: "vector-diskann" is the key parameter
- dimensions must match your embedding model output size
- Similarity options: COS (cosine), L2 (Euclidean), IP (inner product)
-->

---
# **Demo 4: Vector Search with DiskANN 🔍**

## .NET Vector Search Example

```csharp
// Vector similarity search
var pipeline = new[] {
    new BsonDocument("$search", new BsonDocument("cosmosSearch",
        new BsonDocument {
            { "path", "embedding" },
            { "vector", new BsonArray { 0.52, 0.28, 0.12 } },
            { "k", 3 }
        }))
};
```

<!--
Presenter Notes:
- The $search aggregation stage performs the vector search
- k parameter controls how many nearest neighbors to return
- Run the .NET demo app to see results with similarity scores
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

