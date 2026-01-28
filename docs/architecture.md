# Azure DocumentDB Architecture

## In-Depth Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                              CLIENT APPLICATIONS                                         │
│   ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐   │
│   │   mongosh   │  │  PyMongo    │  │ Node.js     │  │   .NET      │  │    Java     │   │
│   │   Shell     │  │  Driver     │  │ MongoDB     │  │  MongoDB    │  │   MongoDB   │   │
│   │             │  │             │  │  Driver     │  │   Driver    │  │    Driver   │   │
│   └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘   │
│          │                │                │                │                │          │
│          └────────────────┴────────────────┼────────────────┴────────────────┘          │
│                                            │                                             │
│                           MongoDB Wire Protocol (Port 10260)                            │
│                                     TLS Encrypted                                        │
└────────────────────────────────────────────┼────────────────────────────────────────────┘
                                             │
                                             ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                          pg_documentdb_gw (GATEWAY LAYER)                               │
│                              Written in Rust                                             │
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                         PROTOCOL & CONNECTION MANAGEMENT                             ││
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────────┐ ││
│  │  │   Protocol   │  │     TLS      │  │  Connection  │  │     Authentication       │ ││
│  │  │    Reader    │  │ Termination  │  │    Pool      │  │    (SCRAM-SHA-256)       │ ││
│  │  │   (OpCode    │  │              │  │   Manager    │  │                          │ ││
│  │  │   Parsing)   │  │              │  │              │  │                          │ ││
│  │  └──────┬───────┘  └──────────────┘  └──────────────┘  └──────────────────────────┘ ││
│  └─────────┼───────────────────────────────────────────────────────────────────────────┘│
│            ▼                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                           REQUEST PROCESSING                                         ││
│  │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────────────────┐   ││
│  │  │  Request Router  │  │   Command Info   │  │       Query Catalog              │   ││
│  │  │                  │  │                  │  │  (MongoDB → PostgreSQL SQL)      │   ││
│  │  │  Routes 50+      │  │  - find          │  │                                  │   ││
│  │  │  MongoDB         │  │  - insert        │  │  find_cursor_first_page          │   ││
│  │  │  Commands        │  │  - update        │  │  aggregate_cursor_first_page     │   ││
│  │  │                  │  │  - delete        │  │  insert, update, delete          │   ││
│  │  │                  │  │  - aggregate     │  │  create_collection               │   ││
│  │  │                  │  │  - createIndex   │  │  create_indexes_background       │   ││
│  │  │                  │  │  - drop          │  │  list_collections, list_indexes  │   ││
│  │  │                  │  │  - 50+ more...   │  │                                  │   ││
│  │  └────────┬─────────┘  └──────────────────┘  └──────────────────────────────────┘   ││
│  └───────────┼─────────────────────────────────────────────────────────────────────────┘│
│              ▼                                                                           │
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                        TRANSACTION & SESSION MANAGEMENT                              ││
│  │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────────────────┐   ││
│  │  │ Transaction      │  │   Cursor Store   │  │       Session Manager            │   ││
│  │  │ Coordinator      │  │                  │  │                                  │   ││
│  │  │                  │  │  - cursorId      │  │  - Session tracking              │   ││
│  │  │  - BEGIN         │  │  - getMore       │  │  - Timeout management            │   ││
│  │  │  - COMMIT        │  │  - pagination    │  │  - Cleanup                       │   ││
│  │  │  - ROLLBACK      │  │  - cleanup       │  │                                  │   ││
│  │  └──────────────────┘  └──────────────────┘  └──────────────────────────────────┘   ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
│              │                                                                           │
│              │  PostgreSQL libpq Protocol                                               │
└──────────────┼──────────────────────────────────────────────────────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                          pg_documentdb (API EXTENSION)                                  │
│                              Written in C                                                │
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                              PUBLIC API (documentdb_api schema)                      ││
│  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐     ││
│  │  │     CRUD       │  │    Indexing    │  │  Aggregation   │  │   Collection   │     ││
│  │  │                │  │                │  │                │  │   Management   │     ││
│  │  │  insert()      │  │ create_indexes │  │ aggregate()    │  │ create_coll()  │     ││
│  │  │  find()        │  │ drop_indexes() │  │ count_query()  │  │ drop_coll()    │     ││
│  │  │  update()      │  │ list_indexes() │  │ distinct()     │  │ list_colls()   │     ││
│  │  │  delete()      │  │ re_index()     │  │                │  │ coll_stats()   │     ││
│  │  │  find_and_     │  │                │  │                │  │ db_stats()     │     ││
│  │  │  modify()      │  │                │  │                │  │                │     ││
│  │  └────────────────┘  └────────────────┘  └────────────────┘  └────────────────┘     ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                           INTERNAL API (documentdb_api_internal schema)             ││
│  │  ┌────────────────────────────────────────────────────────────────────────────────┐ ││
│  │  │  Query Planner Hooks  │  Index Build Status  │  Background Workers  │  Metadata │ ││
│  │  └────────────────────────────────────────────────────────────────────────────────┘ ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                            QUERY PROCESSING                                          ││
│  │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────────────────┐   ││
│  │  │  Query Planner   │  │   Aggregation    │  │      Expression Evaluator        │   ││
│  │  │                  │  │   Pipeline       │  │                                  │   ││
│  │  │  Converts        │  │                  │  │  $match, $project, $group        │   ││
│  │  │  MongoDB         │  │  $lookup         │  │  $sort, $limit, $skip            │   ││
│  │  │  queries to      │  │  $unwind         │  │  $addFields, $set, $unset        │   ││
│  │  │  PostgreSQL      │  │  $group          │  │  $expr, $cond, $switch           │   ││
│  │  │  execution       │  │  $facet          │  │  Comparison, Logical operators   │   ││
│  │  │  plans           │  │                  │  │                                  │   ││
│  │  └──────────────────┘  └──────────────────┘  └──────────────────────────────────┘   ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                         pg_documentdb_core (FOUNDATION LAYER)                           │
│                              Written in C                                                │
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                              BSON DATA TYPE SYSTEM                                   ││
│  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐     ││
│  │  │  BSON Type     │  │    BSON        │  │   Type         │  │   Memory       │     ││
│  │  │  Definition    │  │  Operators     │  │  Conversion    │  │  Management    │     ││
│  │  │                │  │                │  │                │  │                │     ││
│  │  │  - Document    │  │  - Comparison  │  │  BSON ↔ JSON   │  │  Efficient     │     ││
│  │  │  - Array       │  │  - Arithmetic  │  │  BSON ↔ SQL    │  │  allocation    │     ││
│  │  │  - ObjectId    │  │  - String ops  │  │  BSON ↔ Text   │  │  and cleanup   │     ││
│  │  │  - Date        │  │  - Array ops   │  │                │  │                │     ││
│  │  │  - Binary      │  │  - Path access │  │                │  │                │     ││
│  │  │  - Decimal128  │  │                │  │                │  │                │     ││
│  │  └────────────────┘  └────────────────┘  └────────────────┘  └────────────────┘     ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                           CORE UTILITIES (documentdb_core schema)                   ││
│  │  ┌────────────────────────────────────────────────────────────────────────────────┐ ││
│  │  │  row_get_bson()  │  bson_repath()  │  Path Navigation  │  Document Validation  │ ││
│  │  └────────────────────────────────────────────────────────────────────────────────┘ ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                              POSTGRESQL ENGINE                                          │
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                            STORAGE & INDEXING                                        ││
│  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐     ││
│  │  │  Heap Storage  │  │    B-tree      │  │     GIN        │  │   RUM Index    │     ││
│  │  │                │  │    Indexes     │  │    Indexes     │  │  (Extended)    │     ││
│  │  │  BSON          │  │                │  │                │  │                │     ││
│  │  │  documents     │  │  Primary key   │  │  Multi-key     │  │  Full-text     │     ││
│  │  │  stored as     │  │  lookups       │  │  array         │  │  search with   │     ││
│  │  │  binary data   │  │                │  │  indexing      │  │  ranking       │     ││
│  │  └────────────────┘  └────────────────┘  └────────────────┘  └────────────────┘     ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
│  ┌─────────────────────────────────────────────────────────────────────────────────────┐│
│  │                          TRANSACTION MANAGEMENT                                      ││
│  │  ┌────────────────────────────────────────────────────────────────────────────────┐ ││
│  │  │    MVCC     │    WAL (Durability)    │    Replication    │    ACID Guarantees   │ ││
│  │  └────────────────────────────────────────────────────────────────────────────────┘ ││
│  └─────────────────────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## Component Summaries

### 1. **pg_documentdb_gw** (Gateway Layer)

**Language:** Rust  
**Purpose:** Protocol translation layer between MongoDB clients and PostgreSQL backend

| Module | Description |
|--------|-------------|
| **Protocol Reader** | Parses MongoDB wire protocol messages, decodes OpCodes (OP_MSG, OP_QUERY), and extracts BSON documents from incoming requests |
| **TLS Termination** | Handles secure connections, certificate management, and encrypted communication with MongoDB clients |
| **Connection Pool Manager** | Manages PostgreSQL connection pools for efficient resource utilization; supports separate pools for authentication and data operations |
| **Authentication** | Implements SCRAM-SHA-256 authentication mechanism compatible with MongoDB clients; validates credentials against PostgreSQL roles |
| **Request Router** | Routes 50+ MongoDB commands (find, insert, update, delete, aggregate, createIndex, etc.) to appropriate handlers |
| **Query Catalog** | Maps MongoDB operations to PostgreSQL function calls (e.g., `find` → `documentdb_api.find_cursor_first_page()`) |
| **Transaction Coordinator** | Manages multi-document transactions using PostgreSQL's BEGIN/COMMIT/ROLLBACK; tracks transaction states |
| **Cursor Store** | Manages cursor-based pagination; handles `getMore` operations and cursor lifecycle cleanup |
| **Session Manager** | Tracks logical sessions, manages timeout, and handles session cleanup for long-running operations |

---

### 2. **pg_documentdb** (API Extension)

**Language:** C  
**Purpose:** Public API surface providing MongoDB-compatible document operations within PostgreSQL

| Module | Description |
|--------|-------------|
| **CRUD Operations** | Implements `insert()`, `find()`, `update()`, `delete()`, and `find_and_modify()` functions that operate on BSON documents |
| **Indexing** | Provides `create_indexes()`, `drop_indexes()`, `list_indexes()`, and `re_index()` for managing document indexes |
| **Aggregation Pipeline** | Executes MongoDB aggregation framework operations including `$match`, `$group`, `$lookup`, `$unwind`, `$facet` |
| **Collection Management** | Handles `create_collection()`, `drop_collection()`, `list_collections()`, statistics gathering |
| **Query Planner Hooks** | Integrates with PostgreSQL's query planner to optimize document query execution paths |
| **Expression Evaluator** | Evaluates MongoDB query expressions, comparison operators, logical operators, and projection expressions |
| **Background Workers** | Manages background index building, maintenance tasks, and async operations |

**Schemas:**
- `documentdb_api` - Public functions callable via the gateway
- `documentdb_api_internal` - Internal implementation details
- `documentdb_api_catalog` - Metadata and catalog functions
- `documentdb_data` - Document storage tables

---

### 3. **pg_documentdb_core** (Foundation Layer)

**Language:** C  
**Purpose:** Low-level BSON data type support and document operations for PostgreSQL

| Module | Description |
|--------|-------------|
| **BSON Type Definition** | Defines the BSON data type for PostgreSQL including Document, Array, ObjectId, Date, Binary, Decimal128, and all BSON types |
| **BSON Operators** | Implements comparison, arithmetic, string, and array operators for BSON values within SQL queries |
| **Type Conversion** | Converts between BSON and JSON, BSON and SQL types, BSON and text representations |
| **Memory Management** | Efficient memory allocation and cleanup for BSON document processing using PostgreSQL's memory contexts |
| **Path Navigation** | Supports dot-notation path access for nested document fields (e.g., `address.city`) |
| **Document Validation** | Validates BSON document structure and enforces schema constraints when configured |

**Key Functions:**
- `row_get_bson()` - Extracts BSON from PostgreSQL rows
- `bson_repath()` - Navigates and restructures document paths
- `bson_array_agg()` - Aggregates BSON documents into arrays

---

### 4. **pg_documentdb_gw_host** (Optional)

**Language:** Rust (pgrx)  
**Purpose:** PostgreSQL extension that hosts the gateway as a background worker

| Module | Description |
|--------|-------------|
| **Background Worker** | Runs the gateway process within PostgreSQL's background worker framework |
| **GUC Settings** | Configuration parameters for gateway database and setup file paths |
| **Lifecycle Management** | Handles gateway startup, shutdown, and restart within PostgreSQL |

---

### 5. **Optional Extensions**

#### pg_documentdb_distributed
For distributed/sharded deployments using Citus

#### pg_documentdb_extended_rum  
Enhanced RUM (Really Useful Module) index for advanced full-text search with ranking capabilities

---

## Data Flow

```
1. Client sends MongoDB command (e.g., db.users.find({age: {$gt: 25}}))
                    │
                    ▼
2. Gateway (pg_documentdb_gw):
   - Parses MongoDB wire protocol
   - Authenticates connection (SCRAM-SHA-256)
   - Routes command to appropriate handler
   - Translates to PostgreSQL function call:
     SELECT * FROM documentdb_api.find_cursor_first_page('mydb', '{"find": "users", "filter": {"age": {"$gt": 25}}}')
                    │
                    ▼
3. API Extension (pg_documentdb):
   - Parses BSON query document
   - Plans query execution
   - Evaluates filter expressions
   - Returns cursor with results
                    │
                    ▼
4. Core Extension (pg_documentdb_core):
   - BSON type operations
   - Document field access
   - Comparison evaluations
                    │
                    ▼
5. PostgreSQL Engine:
   - Executes optimized query plan
   - Uses indexes (B-tree, GIN, RUM)
   - Returns BSON documents
                    │
                    ▼
6. Response flows back through layers
   - Results serialized to BSON
   - Cursor ID assigned
   - MongoDB wire protocol response sent to client
```

---

## Key Design Principles

1. **MongoDB API Compatibility** - Use existing MongoDB drivers and tools unchanged
2. **PostgreSQL Foundation** - Leverage 30+ years of PostgreSQL reliability, ACID, and tooling
3. **Layered Architecture** - Clean separation between protocol handling, API logic, and data types
4. **Extensibility** - Plugin architecture for distributed deployments and advanced indexing
5. **Performance** - Connection pooling, query optimization, and efficient BSON handling
6. **Open Source** - Apache 2.0 licensed, community-driven development
