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

- **Name:** [Santosh Hari]
- **Role:** [Azure EngOps - Customer Engineer]
- **Experience:** [Brief background]
- **Connect:** 
  - 💼 LinkedIn: [santoshhari]
  - 📧 Email: [your.email@domain.com]

---

# **Agenda**

1. 📌 Introduction to Azure DocumentDB
2. 🏗️ Architecture Overview
3. ⚡ Key Features
4. 💻 **Demo:** Basic CRUD & Schema Flexibility
5. 🚀 **Demo:** Indexing & Performance
6. 📈 Scalability with Azure Portal
7. 🔄 Differentiation: DocumentDB vs MongoDB
8. 🆚 Differentiation: DocumentDB vs SQL Server
9. 🎯 Why DocumentDB? Why Now?

---

# **Introduction to Azure DocumentDB**

## What is Azure DocumentDB?

- **Open Source** distributed document database
- Built for **cloud-native** applications
- Designed for **horizontal scalability**
- **Multi-model** support (documents, key-value, graph)
- **ACID transactions** at document and collection level
- **Global distribution** capabilities

---

# **Introduction to Azure DocumentDB**

## Why Document Databases?

- ✅ **Flexible Schema** - evolve without downtime
- ✅ **Natural data representation** - JSON documents
- ✅ **Developer friendly** - maps to application objects
- ✅ **Fast reads and writes** - optimized for documents
- ✅ **Horizontal scalability** - grow with your data

---

# **Architecture Overview**

## High-Level Design

```
┌─────────────────────────────────────────────┐
│         Application Layer                    │
│  (REST API, SDKs: .NET, Java, Python, JS)  │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│         Query Engine                         │
│  (SQL-like queries, aggregations)           │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│         Storage Engine                       │
│  (Partitioned, Replicated, Indexed)         │
└──────────────────────────────────────────────┘
```

---

# **Architecture Overview**

## Core Components

- **Partition Manager** - data distribution across nodes
- **Replication Layer** - consistency and availability
- **Index Manager** - automatic indexing of all properties
- **Query Processor** - SQL-like query execution
- **Transaction Coordinator** - ACID guarantees
- **Storage Engine** - efficient document storage

---

# **Architecture Overview**

## Partitioning Strategy

- **Logical Partitions** - grouped by partition key
- **Physical Partitions** - distributed across nodes
- **Automatic splitting** - as data grows
- **Transparent to applications** - handled internally
- **Partition key selection** - critical for performance

---

# **Key Features**

## 1️⃣ Flexible Schema

- No predefined schema required
- Documents can have different structures
- Add/remove fields without migrations
- Validate with application logic or triggers

---

# **Key Features**

## 2️⃣ Automatic Indexing

- **All properties indexed** by default
- Configurable indexing policies
- Composite indexes for complex queries
- Spatial indexes for geo-queries
- TTL (Time-to-Live) support

---

# **Key Features**

## 3️⃣ Rich Query Capabilities

- SQL-like query syntax
- Joins within document hierarchy
- Aggregation functions (COUNT, SUM, AVG)
- Filtering, sorting, pagination
- Full-text search capabilities

---

# **Key Features**

## 4️⃣ Multi-Region Distribution

- **Global replication** across Azure regions
- **Multiple consistency levels** (Strong, Bounded Staleness, Session, Consistent Prefix, Eventual)
- **Automatic failover** for high availability
- **Multi-region writes** for low latency

---

# **Key Features**

## 5️⃣ Performance & Scalability

- **Low latency** - single-digit millisecond reads/writes
- **Predictable performance** - reserved throughput (RU/s)
- **Elastic scaling** - scale up/down on demand
- **Unlimited storage and throughput**

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

---

# **Demo Time! 🚀**

## Indexing & Performance Optimization

Exploring DocumentDB's indexing capabilities:

1. **Default indexing** - automatic on all properties
2. **Custom indexing policies** - optimize for your queries
3. **Composite indexes** - multi-field queries
4. **Query performance** - analyze Request Units (RU)
5. **Index tuning** - include/exclude paths

---

# **Demo: Indexing Policy**

## Example Configuration

```json
{
  "indexingMode": "consistent",
  "automatic": true,
  "includedPaths": [
    { "path": "/*" }
  ],
  "excludedPaths": [
    { "path": "/largeTextField/*" }
  ],
  "compositeIndexes": [
    [
      { "path": "/category", "order": "ascending" },
      { "path": "/price", "order": "descending" }
    ]
  ]
}
```

---

# **Demo: Query Performance**

## Request Units (RU) - The Currency of DocumentDB

- **Point reads** - low RU consumption
- **Complex queries** - higher RU consumption
- **Indexes** - reduce RU costs
- **Partition key** in query - critical for performance

```sql
-- Efficient query (uses partition key)
SELECT * FROM c WHERE c.userId = "user-001" AND c.category = "orders"

-- Less efficient (cross-partition)
SELECT * FROM c WHERE c.status = "active"
```

---

# **Scalability with Azure Portal**

## Elastic Throughput Management

- **Provision throughput** - at database or collection level
- **Autoscale** - automatically adjust RU/s
- **Monitor metrics** - RU consumption, latency, availability
- **Scale horizontally** - add partitions as data grows
- **Scale vertically** - increase RU/s for higher throughput

---

# **Scalability with Azure Portal**

## Scaling Options

| **Aspect** | **Capability** | **Limit** |
|------------|---------------|-----------|
| Storage | Unlimited | Per partition: 50GB |
| Throughput | 400 - 1M+ RU/s | Virtually unlimited |
| Partitions | Auto-managed | Transparent scaling |
| Regions | Multi-region | 30+ Azure regions |

---

# **Differentiation: DocumentDB vs MongoDB**

## Similarities

- ✅ Document-oriented data model
- ✅ Flexible schema (JSON/BSON)
- ✅ Rich query language
- ✅ Horizontal scalability
- ✅ Indexing capabilities

---

# **Differentiation: DocumentDB vs MongoDB**

## Key Differences

| **Feature** | **Azure DocumentDB** | **MongoDB** |
|-------------|---------------------|-------------|
| **Licensing** | Open Source (Azure) | Open Source + Enterprise |
| **Consistency** | 5 consistency levels | Eventual/Strong |
| **Integration** | Native Azure services | Self-managed or Atlas |
| **Pricing Model** | RU/s based | Instance/storage based |
| **Global Distribution** | Built-in multi-region | Requires configuration |
| **SLA** | 99.999% availability | Depends on deployment |

---

# **Differentiation: DocumentDB vs MongoDB**

## When to Choose DocumentDB

- 🎯 **Azure-native** applications
- 🎯 Need **predictable performance** (RU/s)
- 🎯 Require **multiple consistency** levels
- 🎯 Want **turnkey global distribution**
- 🎯 Prefer **fully managed** service
- 🎯 Need **comprehensive SLAs**

---

# **Differentiation: DocumentDB vs SQL Server**

## Fundamental Differences

| **Aspect** | **DocumentDB** | **SQL Server** |
|------------|----------------|----------------|
| **Data Model** | Document (JSON) | Relational (Tables) |
| **Schema** | Flexible/Dynamic | Fixed/Predefined |
| **Scaling** | Horizontal (sharding) | Vertical (bigger servers) |
| **Queries** | SQL-like + document | ANSI SQL |
| **Transactions** | Document/collection | Database-wide |
| **Best For** | Unstructured/semi-structured | Structured data |

---

# **Differentiation: DocumentDB vs SQL Server**

## Complementary, Not Competing

**Use DocumentDB when:**
- Data structure evolves frequently
- Need horizontal scalability
- Working with JSON/document data
- Building microservices
- Global distribution required

**Use SQL Server when:**
- Complex relationships and joins
- Strong ACID across tables
- Mature SQL expertise
- Reporting and analytics
- Legacy application integration

---

# **Why DocumentDB?**

## The Value Proposition

1. 🚀 **Developer Productivity** - flexible schema, natural data model
2. ⚡ **Performance** - low latency, predictable throughput
3. 📈 **Scalability** - horizontal scaling, global distribution
4. 🔒 **Enterprise Ready** - ACID, security, compliance
5. 💰 **Cost Effective** - pay for what you use (RU/s model)
6. 🌍 **Open Source** - community-driven innovation

---

# **Why Now?**

## The Perfect Storm

- **Cloud-native applications** are the new standard
- **Microservices architecture** requires flexible data stores
- **Global applications** need low-latency access everywhere
- **Data velocity** - need to handle rapid schema changes
- **DevOps culture** - agility and speed to market
- **Open source momentum** - community collaboration

---

# **Why Now?**

## Industry Trends

```
📊 Market Forces Driving Adoption:

- 70% of new apps are cloud-native (2026)
- Document databases growing 40% YoY
- NoSQL adoption accelerating in enterprises
- Hybrid cloud deployments increasing
- API-first architectures becoming standard
- Real-time data requirements expanding
```

---

# **Getting Started**

## Quick Start Guide

1. **Create Azure account** (free tier available)
2. **Provision DocumentDB instance**
3. **Choose SDK** (.NET, Java, Python, Node.js)
4. **Install packages**
   ```bash
   npm install @azure/cosmos
   # or
   pip install azure-cosmos
   # or
   dotnet add package Microsoft.Azure.Cosmos
   ```
5. **Start building!**

---

# **Resources**

## Learn More

- 📚 **Documentation:** [docs.microsoft.com/azure/documentdb](https://docs.microsoft.com/azure/documentdb)
- 💻 **GitHub:** [github.com/Azure/azure-documentdb](https://github.com/Azure/azure-documentdb)
- 🎓 **Learn Path:** Microsoft Learn modules
- 👥 **Community:** Stack Overflow, GitHub Discussions
- 📺 **Videos:** Channel 9, Azure Friday
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

