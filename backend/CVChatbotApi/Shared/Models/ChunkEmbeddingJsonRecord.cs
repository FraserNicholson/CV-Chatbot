namespace Shared.Models;

public record ChunkEmbeddingJsonRecord(string Id, string Source, string Category, string Text, double[] Embedding);