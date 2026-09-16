using CVChatbotApi.Models;

namespace CVChatbotApi.Services;

public interface ICosineSimilarityService
{
    string[] GetSimilarCVChunks(CosineSimilarityInput input);
}

/// <summary>
/// Receives query, query embedding, as well as cv chunks and their embeddings.
/// Calculates cosine similarity between query and each cv chunk, then returns
/// top N cv chunks that satisfy the minimum similarity cuttoff
/// </summary>
public class CosineSimilarityService : ICosineSimilarityService
{
    private const int NumberOfChunksToTake = 3;
    private const double MinSimilarityCutoff = 0.5;

    private record ChunkWithCosineSimilarity(string CVChunk, double CosineSimilarity);
    
    public string[] GetSimilarCVChunks(CosineSimilarityInput input)
    {
        var chunksWithCosineSimilarity = CollectChunksWithCosineSimilarities(input);
        var mostSimilarChunks = chunksWithCosineSimilarity
            .OrderByDescending(x => x.CosineSimilarity)
            .Take(NumberOfChunksToTake)
            .Select(x => x.CVChunk);

        return [.. mostSimilarChunks];
    }

    private static IEnumerable<ChunkWithCosineSimilarity> CollectChunksWithCosineSimilarities(CosineSimilarityInput input)
    {
        foreach (var chunk in input.ChunkEmbeddings)
        {
            var cosineSimilarity = CalculateCosineSimilarity(input.QueryEmbedding, chunk.Embedding);
            
            if (cosineSimilarity < MinSimilarityCutoff) continue;
            
            yield return new ChunkWithCosineSimilarity(chunk.CVChunk, cosineSimilarity);
        }
    }
    
    private static double CalculateCosineSimilarity(double[] queryEmbedding, double[] cvChunkEmbedding)
    {
        double dot = 0, magA = 0, magB = 0;

        for (var i = 0; i < queryEmbedding.Length; i++)
        {
            dot += queryEmbedding[i] * cvChunkEmbedding[i];
            magA += queryEmbedding[i] * queryEmbedding[i];
            magB += cvChunkEmbedding[i] * cvChunkEmbedding[i];
        }

        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
    }
}