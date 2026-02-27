# Azure DocumentDB - Open Source Project

This repository contains materials for a talk on Azure DocumentDB, the new open source database project.

## Project Structure

```
azure-documentdb/
├── slides/           # Presentation slides for the talk
├── code-samples/     # Code examples and demonstrations
├── LICENSE           # Project license
└── README.md         # This file
```

## Contents

### [Slides](./slides/)
Contains the presentation materials for the Azure DocumentDB talk.

### [Code Samples](./code-samples/)
Contains practical code examples demonstrating Azure DocumentDB features and usage patterns.

## About Azure DocumentDB

Azure DocumentDB is an open source distributed database project that provides scalable, high-performance document storage capabilities.

## Getting Started

### Running the Slides Locally

1. Install dependencies:
   ```bash
   npm install
   ```

2. Start the local slide server:
   ```bash
   npm run slides
   ```
   This launches a Marp dev server at [http://localhost:8081](http://localhost:8081) with live reload.

3. (Optional) Export slides to a static HTML file:
   ```bash
   npm run slides:build
   ```

Refer to the README files in each subfolder for additional instructions and details.

### Environment Variables

Set the following environment variables before running the demo projects:

**PowerShell:**
```powershell
$env:DOCUMENTDB_CONNECTION_STRING = "mongodb://admin:DocDBPass123!@localhost:10260/?tls=true&tlsAllowInvalidCertificates=true"
$env:OPENAI_API_KEY = "sk-your-openai-api-key"
```

**Bash / macOS / Linux:**
```bash
export DOCUMENTDB_CONNECTION_STRING="mongodb://admin:DocDBPass123!@localhost:10260/?tls=true&tlsAllowInvalidCertificates=true"
export OPENAI_API_KEY="sk-your-openai-api-key"
```

| Variable | Required By | Description |
|----------|------------|-------------|
| `DOCUMENTDB_CONNECTION_STRING` | Both demos | MongoDB connection string for DocumentDB |
| `OPENAI_API_KEY` | Vector demo only | OpenAI API key for generating embeddings |

### Running the Demos

```bash
# CRUD demo
dotnet run --project src/DocumentDbDemo

# Vector search demo (requires OPENAI_API_KEY)
dotnet run --project src/DocumentDbVectorDemo
```

## License

See [LICENSE](./LICENSE) file for details.