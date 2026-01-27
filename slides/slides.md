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


# **Demo Time! 🚀**

## Basic CRUD Operations & Schema Flexibility

Let's see Azure DocumentDB in action:

1. **Create** a database and collection
2. **Insert** documents with different schemas
3. **Read** documents with queries
4. **Update** documents
5. **Delete** documents

---

# **Demo: Basic CRUD**

## Sample Documents (Different Schemas!)

```json
// User Document
{
  "id": "user-001",
  "type": "user",
  "name": "Jane Doe",
  "email": "jane@example.com",
  "joinDate": "2026-01-15"
}

// Product Document
{
  "id": "prod-001", 
  "type": "product",
  "name": "Laptop",
  "price": 1299.99,
  "specs": { "cpu": "Intel i7", "ram": "16GB" }
}
```

**Same collection, different structures!** ✨

---

# **Demo: Schema Flexibility**

## Evolution Without Downtime

```json
// Version 1 - Original User
{
  "id": "user-002",
  "name": "John Smith",
  "email": "john@example.com"
}

// Version 2 - Extended User (no migration needed!)
{
  "id": "user-003",
  "name": "Alice Johnson",
  "email": "alice@example.com",
  "phone": "+1-555-0123",
  "address": {
    "city": "Seattle",
    "country": "USA"
  }
}
```

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

# **Why Azure DocumentDB Exists**

## The Core Problems It Solves

### A Truly Open Alternative

Azure DocumentDB is:
- **Apache 2.0 licensed**
- **Community-driven**
- **Cloud-neutral**
- **Vendor-agnostic**

This matters for:
- Governments
- Regulated industries
- Linux distributions
- Companies building database platforms

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

# **Resources**

## Learn More

- 📚 **Documentation:** [docs.microsoft.com/azure/documentdb](https://docs.microsoft.com/azure/documentdb)
- 💻 **GitHub:** [github.com/Azure/azure-documentdb](https://github.com/Azure/azure-documentdb)
- 🎓 **Learn Path:** Microsoft Learn modules
- 👥 **Community:** Stack Overflow, GitHub Discussions
- 🎯 **Samples:** [Code samples in this repo](../code-samples/)

---

# **Q&A**

## Questions?

**Thank you for attending!**

Let's discuss:
- Your specific use cases
- Migration strategies
- Architecture patterns
- Performance optimization
- Best practices

---

# **Contact & Follow-Up**

## Stay Connected

- 📧 **Email:** [your.email@domain.com]
- 🐦 **Twitter:** [@yourusername]
- 💼 **LinkedIn:** [Your Profile]
- 💻 **GitHub:** [This Repository]
- 📝 **Blog:** [Your Blog URL]

### Resources from this talk:
- Slides: Available in this repository
- Code samples: [/code-samples](../code-samples/)
- Demo scripts: Coming soon

---

<!-- Last slide -->
# Thank You! 🙏

### Start building with Azure DocumentDB today!

**Remember:** The best database is the one that fits your use case.

