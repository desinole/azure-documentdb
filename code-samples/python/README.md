# Python Code Samples for Azure DocumentDB

This directory contains Python code examples demonstrating various Azure DocumentDB features and operations.

## Samples

### [CRUD Operations](./crud/)
Basic Create, Read, Update, and Delete operations using MongoDB-compatible API.

**Topics covered:**
- Database and collection management
- Document insertion (single and batch)
- Querying documents
- Updating documents
- Deleting documents
- Error handling
- Connection management

## Prerequisites

- Python 3.8 or higher
- Azure DocumentDB instance or local emulator
- pip package manager

## Getting Started

1. Navigate to a specific sample directory
2. Install requirements: `pip install -r requirements.txt`
3. Configure connection settings (see sample's README)
4. Run the sample: `python <script_name>.py`

## Common Dependencies

Most samples use:
- `pymongo` - MongoDB driver for Python
- `python-dotenv` - Environment variable management

## Structure

Each sample includes:
- `README.md` - Detailed instructions and explanations
- `requirements.txt` - Python dependencies
- `.env.example` - Sample configuration
- Source code with comprehensive comments

## Additional Resources

- [Python MongoDB Driver Documentation](https://pymongo.readthedocs.io/)
- [Azure DocumentDB Documentation](https://docs.microsoft.com/azure/documentdb)
- [Python Best Practices](https://docs.python.org/3/library/index.html)
