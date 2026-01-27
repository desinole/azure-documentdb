"""
Azure DocumentDB CRUD Operations Demo
======================================

This script demonstrates basic Create, Read, Update, and Delete (CRUD) operations
using Azure DocumentDB's MongoDB-compatible API with a local emulator.

Prerequisites:
- Azure DocumentDB local emulator running on localhost:27017
- Python 3.8 or higher
- Required packages: pymongo, python-dotenv

Author: Azure DocumentDB Demo
Date: 2026
"""

import os
from datetime import datetime
from typing import Dict, List, Optional
from pymongo import MongoClient, errors
from dotenv import load_dotenv

# Load environment variables from .env file
load_dotenv()

# Connection configuration
CONNECTION_STRING = os.getenv('DOCUMENTDB_CONNECTION_STRING', 'mongodb://localhost:27017')
DATABASE_NAME = os.getenv('DOCUMENTDB_DATABASE_NAME', 'testdb')
COLLECTION_NAME = os.getenv('DOCUMENTDB_COLLECTION_NAME', 'users')


class DocumentDBClient:
    """
    A client class for Azure DocumentDB operations using MongoDB API.
    
    This class provides a simplified interface for common database operations
    including connection management and CRUD operations.
    """
    
    def __init__(self, connection_string: str, database_name: str, collection_name: str):
        """
        Initialize the DocumentDB client.
        
        Args:
            connection_string: MongoDB connection string for DocumentDB
            database_name: Name of the database to use
            collection_name: Name of the collection to use
        """
        self.connection_string = connection_string
        self.database_name = database_name
        self.collection_name = collection_name
        self.client = None
        self.database = None
        self.collection = None
        
    def connect(self) -> bool:
        """
        Establish connection to Azure DocumentDB.
        
        Returns:
            bool: True if connection successful, False otherwise
        """
        try:
            # Create MongoClient instance to connect to DocumentDB
            self.client = MongoClient(
                self.connection_string,
                serverSelectionTimeoutMS=5000  # 5 second timeout
            )
            
            # Test the connection by pinging the server
            self.client.admin.command('ping')
            print(f"✓ Successfully connected to Azure DocumentDB at {self.connection_string}")
            
            # Select database (creates if doesn't exist)
            self.database = self.client[self.database_name]
            
            # Select collection (creates if doesn't exist)
            self.collection = self.database[self.collection_name]
            
            return True
            
        except errors.ConnectionFailure as e:
            print(f"✗ Failed to connect to DocumentDB: {e}")
            return False
        except Exception as e:
            print(f"✗ Unexpected error during connection: {e}")
            return False
    
    def disconnect(self):
        """Close the connection to DocumentDB."""
        if self.client:
            self.client.close()
            print("✓ Disconnected from Azure DocumentDB")
    
    # ==================== CREATE Operations ====================
    
    def create_document(self, document: Dict) -> Optional[str]:
        """
        Insert a single document into the collection.
        
        Args:
            document: Dictionary containing the document data
            
        Returns:
            str: The inserted document's ID, or None if failed
        """
        try:
            # Add timestamp to document
            document['createdAt'] = datetime.utcnow()
            
            # Insert the document
            result = self.collection.insert_one(document)
            
            print(f"✓ Document created with ID: {result.inserted_id}")
            return str(result.inserted_id)
            
        except errors.DuplicateKeyError:
            print(f"✗ Document with ID already exists")
            return None
        except Exception as e:
            print(f"✗ Error creating document: {e}")
            return None
    
    def create_documents(self, documents: List[Dict]) -> List[str]:
        """
        Insert multiple documents into the collection.
        
        Args:
            documents: List of dictionaries containing document data
            
        Returns:
            List[str]: List of inserted document IDs
        """
        try:
            # Add timestamps to all documents
            for doc in documents:
                doc['createdAt'] = datetime.utcnow()
            
            # Insert multiple documents
            result = self.collection.insert_many(documents)
            
            inserted_ids = [str(id) for id in result.inserted_ids]
            print(f"✓ Created {len(inserted_ids)} documents")
            
            return inserted_ids
            
        except Exception as e:
            print(f"✗ Error creating documents: {e}")
            return []
    
    # ==================== READ Operations ====================
    
    def read_document_by_id(self, document_id: str) -> Optional[Dict]:
        """
        Retrieve a single document by its ID.
        
        Args:
            document_id: The ID of the document to retrieve
            
        Returns:
            Dict: The document data, or None if not found
        """
        try:
            from bson.objectid import ObjectId
            
            # Query for document by _id
            document = self.collection.find_one({'_id': ObjectId(document_id)})
            
            if document:
                print(f"✓ Document found: {document_id}")
                return document
            else:
                print(f"✗ Document not found: {document_id}")
                return None
                
        except Exception as e:
            print(f"✗ Error reading document: {e}")
            return None
    
    def read_documents_by_query(self, query: Dict, limit: int = 10) -> List[Dict]:
        """
        Retrieve multiple documents matching a query.
        
        Args:
            query: MongoDB query filter (e.g., {'status': 'active'})
            limit: Maximum number of documents to return
            
        Returns:
            List[Dict]: List of matching documents
        """
        try:
            # Execute query with limit
            documents = list(self.collection.find(query).limit(limit))
            
            print(f"✓ Found {len(documents)} documents matching query")
            return documents
            
        except Exception as e:
            print(f"✗ Error querying documents: {e}")
            return []
    
    def read_all_documents(self, limit: int = 100) -> List[Dict]:
        """
        Retrieve all documents from the collection.
        
        Args:
            limit: Maximum number of documents to return
            
        Returns:
            List[Dict]: List of all documents
        """
        try:
            # Query all documents with limit
            documents = list(self.collection.find().limit(limit))
            
            print(f"✓ Retrieved {len(documents)} documents from collection")
            return documents
            
        except Exception as e:
            print(f"✗ Error reading all documents: {e}")
            return []
    
    # ==================== UPDATE Operations ====================
    
    def update_document(self, document_id: str, updates: Dict) -> bool:
        """
        Update a single document by its ID.
        
        Args:
            document_id: The ID of the document to update
            updates: Dictionary of fields to update
            
        Returns:
            bool: True if update successful, False otherwise
        """
        try:
            from bson.objectid import ObjectId
            
            # Add update timestamp
            updates['updatedAt'] = datetime.utcnow()
            
            # Update the document using $set operator
            result = self.collection.update_one(
                {'_id': ObjectId(document_id)},
                {'$set': updates}
            )
            
            if result.modified_count > 0:
                print(f"✓ Document updated: {document_id}")
                return True
            else:
                print(f"✗ No document updated (may not exist or no changes): {document_id}")
                return False
                
        except Exception as e:
            print(f"✗ Error updating document: {e}")
            return False
    
    def update_documents_by_query(self, query: Dict, updates: Dict) -> int:
        """
        Update multiple documents matching a query.
        
        Args:
            query: MongoDB query filter
            updates: Dictionary of fields to update
            
        Returns:
            int: Number of documents updated
        """
        try:
            # Add update timestamp
            updates['updatedAt'] = datetime.utcnow()
            
            # Update multiple documents
            result = self.collection.update_many(
                query,
                {'$set': updates}
            )
            
            print(f"✓ Updated {result.modified_count} documents")
            return result.modified_count
            
        except Exception as e:
            print(f"✗ Error updating documents: {e}")
            return 0
    
    # ==================== DELETE Operations ====================
    
    def delete_document(self, document_id: str) -> bool:
        """
        Delete a single document by its ID.
        
        Args:
            document_id: The ID of the document to delete
            
        Returns:
            bool: True if deletion successful, False otherwise
        """
        try:
            from bson.objectid import ObjectId
            
            # Delete the document
            result = self.collection.delete_one({'_id': ObjectId(document_id)})
            
            if result.deleted_count > 0:
                print(f"✓ Document deleted: {document_id}")
                return True
            else:
                print(f"✗ Document not found for deletion: {document_id}")
                return False
                
        except Exception as e:
            print(f"✗ Error deleting document: {e}")
            return False
    
    def delete_documents_by_query(self, query: Dict) -> int:
        """
        Delete multiple documents matching a query.
        
        Args:
            query: MongoDB query filter
            
        Returns:
            int: Number of documents deleted
        """
        try:
            # Delete multiple documents
            result = self.collection.delete_many(query)
            
            print(f"✓ Deleted {result.deleted_count} documents")
            return result.deleted_count
            
        except Exception as e:
            print(f"✗ Error deleting documents: {e}")
            return 0
    
    def delete_all_documents(self) -> int:
        """
        Delete all documents from the collection.
        
        Returns:
            int: Number of documents deleted
        """
        try:
            # Delete all documents (empty query)
            result = self.collection.delete_many({})
            
            print(f"✓ Deleted all {result.deleted_count} documents from collection")
            return result.deleted_count
            
        except Exception as e:
            print(f"✗ Error deleting all documents: {e}")
            return 0


def print_document(document: Dict):
    """Helper function to print a document in a readable format."""
    print("\n" + "=" * 60)
    for key, value in document.items():
        print(f"  {key}: {value}")
    print("=" * 60)


def demo_crud_operations():
    """
    Demonstrate all CRUD operations with Azure DocumentDB.
    
    This function runs through a complete example of:
    1. Connecting to DocumentDB
    2. Creating documents
    3. Reading documents
    4. Updating documents
    5. Deleting documents
    """
    
    print("\n" + "=" * 60)
    print("Azure DocumentDB CRUD Operations Demo")
    print("=" * 60 + "\n")
    
    # Initialize the client
    client = DocumentDBClient(CONNECTION_STRING, DATABASE_NAME, COLLECTION_NAME)
    
    # Step 1: Connect to DocumentDB
    print("\n[1] CONNECTING TO DOCUMENTDB")
    print("-" * 60)
    if not client.connect():
        print("Failed to connect. Exiting.")
        return
    
    # Clean up any existing data for demo purposes
    client.delete_all_documents()
    
    # Step 2: CREATE Operations
    print("\n[2] CREATE OPERATIONS")
    print("-" * 60)
    
    # Create a single document
    user1 = {
        'name': 'Alice Johnson',
        'email': 'alice@example.com',
        'age': 28,
        'city': 'Seattle',
        'status': 'active'
    }
    user1_id = client.create_document(user1)
    
    # Create multiple documents
    users = [
        {
            'name': 'Bob Smith',
            'email': 'bob@example.com',
            'age': 35,
            'city': 'Portland',
            'status': 'active'
        },
        {
            'name': 'Charlie Brown',
            'email': 'charlie@example.com',
            'age': 42,
            'city': 'Seattle',
            'status': 'inactive'
        },
        {
            'name': 'Diana Prince',
            'email': 'diana@example.com',
            'age': 31,
            'city': 'New York',
            'status': 'active'
        }
    ]
    user_ids = client.create_documents(users)
    
    # Step 3: READ Operations
    print("\n[3] READ OPERATIONS")
    print("-" * 60)
    
    # Read by ID
    if user1_id:
        document = client.read_document_by_id(user1_id)
        if document:
            print_document(document)
    
    # Read by query - find all active users
    print("\nQuerying for active users:")
    active_users = client.read_documents_by_query({'status': 'active'})
    for user in active_users:
        print(f"  - {user['name']} ({user['email']})")
    
    # Read by query - find users in Seattle
    print("\nQuerying for users in Seattle:")
    seattle_users = client.read_documents_by_query({'city': 'Seattle'})
    for user in seattle_users:
        print(f"  - {user['name']} (Age: {user['age']})")
    
    # Read all documents
    print("\nReading all documents:")
    all_users = client.read_all_documents()
    print(f"Total users in collection: {len(all_users)}")
    
    # Step 4: UPDATE Operations
    print("\n[4] UPDATE OPERATIONS")
    print("-" * 60)
    
    # Update single document
    if user1_id:
        print(f"\nUpdating user {user1_id}:")
        client.update_document(user1_id, {
            'age': 29,
            'city': 'San Francisco',
            'lastLogin': datetime.utcnow()
        })
        
        # Verify the update
        updated_user = client.read_document_by_id(user1_id)
        if updated_user:
            print(f"  Updated age: {updated_user['age']}")
            print(f"  Updated city: {updated_user['city']}")
    
    # Update multiple documents
    print("\nUpdating all inactive users to active:")
    updated_count = client.update_documents_by_query(
        {'status': 'inactive'},
        {'status': 'active', 'statusChangedAt': datetime.utcnow()}
    )
    
    # Step 5: DELETE Operations
    print("\n[5] DELETE OPERATIONS")
    print("-" * 60)
    
    # Delete by query - remove users over 40
    print("\nDeleting users over age 40:")
    deleted_count = client.delete_documents_by_query({'age': {'$gt': 40}})
    
    # Verify remaining documents
    remaining_users = client.read_all_documents()
    print(f"\nRemaining users: {len(remaining_users)}")
    for user in remaining_users:
        print(f"  - {user['name']} (Age: {user['age']})")
    
    # Delete single document
    if user1_id:
        print(f"\nDeleting user {user1_id}:")
        client.delete_document(user1_id)
    
    # Final cleanup
    print("\n[6] CLEANUP")
    print("-" * 60)
    print("Deleting all remaining documents:")
    client.delete_all_documents()
    
    # Step 6: Disconnect
    print("\n[7] DISCONNECTING")
    print("-" * 60)
    client.disconnect()
    
    print("\n" + "=" * 60)
    print("Demo completed successfully!")
    print("=" * 60 + "\n")


if __name__ == '__main__':
    """
    Main entry point for the application.
    
    Run this script to see a complete demonstration of CRUD operations
    with Azure DocumentDB.
    """
    try:
        demo_crud_operations()
    except KeyboardInterrupt:
        print("\n\nDemo interrupted by user.")
    except Exception as e:
        print(f"\n\nUnexpected error: {e}")
