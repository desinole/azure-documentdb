# Azure DocumentDB CRUD Operations - Python

This sample demonstrates basic Create, Read, Update, and Delete (CRUD) operations using Azure DocumentDB's MongoDB-compatible API.

## Overview

This example shows how to:
- Connect to Azure DocumentDB local emulator
- Create single and multiple documents
- Read documents by ID and query
- Update documents individually and in bulk
- Delete documents by ID and query
- Handle errors and connections properly

## Prerequisites

- Python 3.8 or higher
- Azure DocumentDB local emulator running on `localhost:27017`
- pip (Python package manager)

## Installation

1. Navigate to this directory:
   ```bash
   cd code-samples/python/crud
   ```

2. Install required packages:
   ```bash
   pip install -r requirements.txt
   ```

3. Configure connection (optional):
   - Copy `.env.example` to `.env`
   - Modify connection settings if needed
   ```bash
   cp .env.example .env
   ```

## Running the Demo

Execute the main script:
```bash
python crud_operations.py
```

## What the Demo Does

The script performs the following operations:

### 1. Connection
- Establishes connection to DocumentDB emulator
- Creates database and collection if they don't exist

### 2. Create Operations
- Inserts a single user document
- Inserts multiple user documents in batch

### 3. Read Operations
- Retrieves document by ID
- Queries documents by status (active/inactive)
- Queries documents by city
- Retrieves all documents

### 4. Update Operations
- Updates a single document (age and city)
- Updates multiple documents (status change)
- Adds timestamps to track changes

### 5. Delete Operations
- Deletes documents matching a query (age > 40)
- Deletes a single document by ID
- Cleans up all documents

## Code Structure

### `DocumentDBClient` Class

The main class provides organized CRUD operations:

- **Connection Management**
  - `connect()` - Establish connection
  - `disconnect()` - Close connection

- **Create Operations**
  - `create_document(document)` - Insert single document
  - `create_documents(documents)` - Insert multiple documents

- **Read Operations**
  - `read_document_by_id(id)` - Get document by ID
  - `read_documents_by_query(query)` - Query documents
  - `read_all_documents()` - Get all documents

- **Update Operations**
  - `update_document(id, updates)` - Update single document
  - `update_documents_by_query(query, updates)` - Update multiple documents

- **Delete Operations**
  - `delete_document(id)` - Delete single document
  - `delete_documents_by_query(query)` - Delete by query
  - `delete_all_documents()` - Delete all documents

## Configuration

Default configuration connects to local emulator:

```python
CONNECTION_STRING = 'mongodb://localhost:27017'
DATABASE_NAME = 'testdb'
COLLECTION_NAME = 'users'
```

You can modify these in `.env` file or set environment variables.

## Sample Document Structure

```json
{
  "_id": "507f1f77bcf86cd799439011",
  "name": "Alice Johnson",
  "email": "alice@example.com",
  "age": 28,
  "city": "Seattle",
  "status": "active",
  "createdAt": "2026-01-26T10:30:00Z",
  "updatedAt": "2026-01-26T11:00:00Z"
}
```

## Error Handling

The code includes comprehensive error handling for:
- Connection failures
- Duplicate key errors
- Document not found scenarios
- Network timeouts
- Invalid queries

## Best Practices Demonstrated

1. **Connection Management** - Proper connect/disconnect patterns
2. **Error Handling** - Try-catch blocks for all operations
3. **Timestamps** - Automatic `createdAt` and `updatedAt` fields
4. **Type Hints** - Full type annotations for better code clarity
5. **Documentation** - Comprehensive docstrings and comments
6. **Query Efficiency** - Using indexes and limiting results
7. **Batch Operations** - Efficient bulk inserts

## Troubleshooting

### Connection Failed
- Ensure DocumentDB emulator is running
- Check if port 27017 is available
- Verify firewall settings

### Import Errors
- Reinstall dependencies: `pip install -r requirements.txt`
- Check Python version: `python --version`

### Authentication Errors
- Verify connection string format
- Check if emulator requires authentication

## Next Steps

- Explore indexing strategies
- Implement aggregation pipelines
- Add transaction support
- Create more complex queries
- Implement pagination

## Resources

- [Azure DocumentDB Documentation](https://docs.microsoft.com/azure/documentdb)
- [PyMongo Documentation](https://pymongo.readthedocs.io/)
- [MongoDB Query Documentation](https://docs.mongodb.com/manual/tutorial/query-documents/)
