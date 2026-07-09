# Glossary

Plain-English definitions for the AI, vector, and database terms used in this talk. [Back to the slides](index.html).

<h2 id="embedding">Embedding</h2>

An embedding is a list of numbers that captures the meaning of text, an image, or audio. Similar content produces similar embeddings, so you can compare meaning with math instead of matching exact words.

<h2 id="vector">Vector</h2>

A vector is an ordered list of numbers. In AI, an embedding is a vector, and each number is one coordinate in a high-dimensional space.

<h2 id="vector-search">Vector search</h2>

Vector search finds the items whose vectors sit closest to a query vector. It ranks results by meaning rather than exact keyword matches.

<h2 id="vector-database">Vector database</h2>

A vector database stores embeddings and runs vector search over them at scale, usually with a specialized index. DocumentDB adds this capability to your existing document store, so operational data and vectors live in one system.

<h2 id="semantic-search">Semantic search</h2>

Semantic search matches on meaning. A search for "couch" can return "sofa" because their embeddings are close, even though the words differ.

<h2 id="cosine-similarity">Cosine similarity</h2>

Cosine similarity measures the angle between two vectors. A score near 1 means very similar, and a score near 0 means unrelated. It's the default way vector search scores a match.

<h2 id="llm">LLM (large language model)</h2>

A large language model is an AI model trained on huge amounts of text to understand and generate language. GPT-4 and Llama are examples.

<h2 id="rag">RAG (retrieval-augmented generation)</h2>

RAG feeds an LLM relevant documents pulled from your own data before it answers. Vector search does the retrieval step, so the model grounds its answer in your content.

<h2 id="ann">ANN (approximate nearest neighbor)</h2>

ANN search trades a small amount of accuracy for a large speed gain. It finds the vectors that are almost certainly closest without checking every record.

<h2 id="ivf">IVF (inverted file index)</h2>

IVF groups vectors into clusters. A query searches only the nearest clusters instead of scanning everything, which makes it fast to run.

<h2 id="hnsw">HNSW (hierarchical navigable small world)</h2>

HNSW builds a multi-layer graph of vectors for fast, accurate search. The top layers cover long distances, and the bottom layers refine the result. It's quick and precise, but it keeps the whole index in memory.

<h2 id="diskann">DiskANN</h2>

DiskANN is a graph-based ANN algorithm from Microsoft Research that stores the index on SSD instead of RAM. It scales to billions of vectors at a fraction of the memory cost. DocumentDB uses it.

<h2 id="product-quantization">Product quantization</h2>

Product quantization compresses vectors into a small code. DiskANN keeps these compressed vectors in memory for fast navigation, then reads the full vectors from disk when it needs them.

<h2 id="dimensions">Dimensions</h2>

The dimension count is how many numbers a vector holds. OpenAI's text-embedding-3-small uses 1,536 dimensions. More dimensions can capture more detail, at the cost of more storage.

<h2 id="recall">Recall</h2>

Recall is the share of the true nearest neighbors that a search actually returns. Higher recall means more accurate results. ANN algorithms aim for high recall while staying fast.

<h2 id="bson">BSON</h2>

BSON is binary JSON, the format MongoDB and DocumentDB use to store documents. It supports more data types than plain JSON and is faster for the database to scan.

<h2 id="acid">ACID</h2>

ACID stands for atomicity, consistency, isolation, and durability. These are the guarantees that keep a transaction correct even when something fails partway through.

<h2 id="aggregation-pipeline">Aggregation pipeline</h2>

An aggregation pipeline runs documents through a series of stages such as filter, group, sort, and transform. It's how MongoDB and DocumentDB express complex queries and analytics.

<h2 id="wire-protocol">Wire protocol</h2>

The MongoDB wire protocol is the network format that MongoDB drivers use to talk to a server. DocumentDB speaks it, so your existing drivers connect without code changes.

<h2 id="sharding">Sharding</h2>

Sharding splits a collection across many nodes using a shard key. Each node holds part of the data, which lets the database scale out horizontally.

<h2 id="geo-replication">Geo-replication</h2>

Geo-replication copies your data to other regions. It cuts read latency for distant users and keeps a copy available if one region goes down.
