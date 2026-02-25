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

### Powering Modern Applications with Scale and Performance

---

# **Speaker Introduction**

<!-- Add your information here -->

- **Name:** Santosh Hari
- **Role:** Azure EngOps
- **Connect:** 
  - LinkedIn: santoshhari
  - BlueSky: @santoshhari.dev
  - GitHub: desinole
---

# **Agenda**

1. 📌 Introduction to Azure DocumentDB
2. 🏗️ Architecture Overview
3. ⚡ Key Features
4. 🔄 Differentiation: DocumentDB vs MongoDB
5. 🆚 Differentiation: DocumentDB vs SQL Server
6. 🎯 Why DocumentDB? Why Now?

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

**Result:** Organizations want MongoDB's API—not its licensing terms.

---

# **Why Azure DocumentDB Exists**

## The Core Problems It Solves

### Avoiding Vendor Lock-In

Teams built large applications around MongoDB's:
- Query language
- Drivers  
- Data model

**Rewriting is expensive and risky.**

Azure DocumentDB offers:
- ✅ Zero or near-zero code changes
- ✅ Same MongoDB drivers
- ✅ Same queries
- ✅ Different backend (PostgreSQL)
- ✅ **Exit optionality**

<!--
Presenter Notes:
- MongoDB's 2018 license change (AGPL to SSPL) created major issues for enterprises
- SSPL is not recognized as "open source" by OSI - this matters for procurement and compliance
- Many governments and regulated industries cannot use SSPL-licensed software
- Linux distributions (Red Hat, Debian, Ubuntu) removed MongoDB from their repos
- Legal departments flag SSPL as high-risk for SaaS companies
- The core insight: teams love MongoDB's developer experience but want freedom from MongoDB Inc.
- Vendor lock-in is real: once you've built on MongoDB, switching databases means rewriting everything
- Migration risk: rewriting queries, retesting, potential data loss, business disruption
- Azure DocumentDB decouples the API from the product - you get MongoDB compatibility without MongoDB
- Exit strategy: if you need to move off managed MongoDB, you have options
- This is strategic: avoid single-vendor dependency for critical infrastructure
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
- PostgreSQL has 30+ years of production hardening - this is not a new, unproven database
- ACID transactions: guaranteed consistency, unlike eventual consistency models
- Strong consistency: reads reflect all previous writes - critical for financial, healthcare, etc.
- Durability guarantees: PostgreSQL's WAL (Write-Ahead Logging) is battle-tested
- Indexing: B-tree, hash, GIN, GiST, BRIN - sophisticated index types for any workload
- Operational tooling: pg_dump, pg_restore, replication, monitoring, backup tools - mature ecosystem
- The proposition: don't choose between MongoDB's API and PostgreSQL's reliability - get both
- Teams already know Postgres: DBAs, SREs, and ops teams have existing expertise
- Existing infrastructure: leverage current PostgreSQL clusters, backups, monitoring
- Cost savings: use existing Postgres licenses and infrastructure instead of new MongoDB clusters
- Apache 2.0: truly open source, OSI-approved, no strings attached
- Community-driven: not controlled by a single vendor with changing license terms
- Cloud-neutral: works on Azure, AWS, GCP, on-prem - no lock-in
- Platform builders: companies creating database-as-a-service can use DocumentDB without licensing concerns
- Compliance-friendly: governments and regulated industries can adopt without legal reviews
- Distribution-friendly: Linux distros can package and distribute without SSPL concerns
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
- This isn't about replacing SQL Server - it's about expanding your toolkit
- Document model addresses real pain points: schema evolution, JSON-native apps, horizontal scale
- MongoDB API is ubiquitous: drivers, tools, developer familiarity - it's become a standard interface
- Modern apps demand: flexible schemas (JSON), transactional integrity (ACID), no vendor lock-in (portability)
- PostgreSQL evolution: from relational purist to multi-model powerhouse - JSONB, time-series, graph
- Compatibility layers lesson: Don't rebuild from scratch - translate and adapt (see: TDS, MySQL protocol compatibility)
- Career perspective: DBAs who understand both relational AND document models are more valuable
- Technology perspective: This is convergence, not replacement - SQL + JSON in the same ecosystem
- FerretDB and Azure DocumentDB prove: PostgreSQL foundation + MongoDB interface = powerful combination
- Strategic insight: Future databases will be multi-model with multiple API surfaces
- Action item for DBAs: Learn document modeling, understand JSONB, explore PostgreSQL extensions
-->

---

# **Demo 1: Setting Up DocumentDB Locally 🐳**

## Running DocumentDB in a Container

```bash
# Pull the DocumentDB container image
docker pull mcr.microsoft.com/documentdb/documentdb:latest

# Run DocumentDB locally
docker run -dt --name documentdb \
  -p 10260:10260 -p 5432:5432 \
  -e DOCUMENTDB_ADMIN_USER=admin \
  -e DOCUMENTDB_ADMIN_PASSWORD=YourPassword123! \
  mcr.microsoft.com/documentdb/documentdb:latest
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

# **Demo 3: Two Query Interfaces 🔍**

## MongoDB Queries (via Gateway, port 10260)

```javascript
// Find electronics over $100
db.products.find({ category: "electronics", price: { $gt: 100 } })

// Aggregation pipeline
db.products.aggregate([
  { $group: { _id: "$category", avgPrice: { $avg: "$price" } } }
])
```

## PostgreSQL Queries (via Backend, port 5432)

```sql
-- Same data, queried with SQL
SELECT document FROM documentdb_api.collection('demo_db', 'products')
WHERE document @> '{"category": "electronics"}';
```

**Same data, two query languages!** ✨

<!--
Presenter Notes:
- First, show mongosh connecting to port 10260 and running familiar MongoDB queries
- Then, show psql connecting to port 5432 and querying the same data with SQL
- Key takeaway: the data is stored once but accessible through both interfaces
- MongoDB queries go through the gateway which translates to PostgreSQL operations
- PostgreSQL queries hit the backend directly — useful for DBAs and reporting
- This dual-interface approach gives teams flexibility: app developers use MongoDB, DBAs use SQL
-->

---

# **Vector Search with DiskANN 🧠**

## What is DiskANN?

- **Disk-based Approximate Nearest Neighbor** search algorithm
- Developed by **Microsoft Research**
- Graph-structured index for **scalable vector search**
- Handles **billions of vectors** without requiring all data in memory
- Published at NeurIPS 2019, open-sourced on GitHub

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
- Published at NeurIPS 2019 — one of the top ML/AI conferences
- Open source on GitHub: github.com/microsoft/DiskANN
- Now rewritten in Rust for performance and safety
-->

---

# **Vector Search with DiskANN 🧠**

## Azure DocumentDB vs Competitors

| Feature | **DocumentDB** | **Pinecone** | **Weaviate** | **Qdrant** | **Milvus** | **pgvector** |
|---------|---------------|-------------|-------------|-----------|-----------|-------------|
| **Index** | DiskANN | Proprietary | HNSW | HNSW | Multiple | HNSW/IVF |
| **Open Source** | ✅ Apache 2.0 | ❌ | ✅ | ✅ | ✅ | ✅ |
| **Self-Host** | ✅ | ❌ | ✅ | ✅ | ✅ | ✅ |
| **Scale** | 500K+ vectors | Large | Medium | Medium | Large | Small |
| **Memory** | Low (SSD) | Managed | High | High | High | High |
| **Filtered Search** | ✅ Native | ✅ | ✅ | ✅ | ✅ | ❌ Limited |
| **Document DB** | ✅ Built-in | ❌ | ✅ | ❌ | ❌ | ❌ |
| **MongoDB API** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **SQL Access** | ✅ PostgreSQL | ❌ | ❌ | ❌ | ❌ | ✅ |

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

# **Vector Search with DiskANN 🧠**

## Why DiskANN in DocumentDB?

### Integrated Vector Search — Not a Bolt-On

- Vectors stored **alongside your documents** — no separate vector DB
- **No data sync pipelines** to build and maintain
- Query vectors and documents in the **same query**
- Native support for **filtered vector search** (geo, text, numeric)
- Supports up to **16,000 dimensions** with product quantization

### Use Cases

- 🔍 **Semantic search** over product catalogs
- 🤖 **RAG** (Retrieval-Augmented Generation) for AI apps
- 🎯 **Recommendation engines** with contextual filtering
- 📍 **Location-aware similarity** (vector + geospatial)

<!--
Presenter Notes:
- The killer feature: vectors and documents in ONE database — no separate Pinecone/Weaviate/Qdrant
- Data consistency: when you update a document, the vector index updates too
- No ETL pipelines: no need to sync data between your app DB and a vector DB
- Filtered search: combine vector similarity with geo, text, or numeric filters in one query
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

---

# **Thank You! 🙏**

- **Name:** Santosh Hari
- **Role:** Azure EngOps
- **Connect:** 
  - LinkedIn: santoshhari
  - BlueSky: @santoshhari.dev
  - GitHub: desinole
---
