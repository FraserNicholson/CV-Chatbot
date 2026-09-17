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
public class CosineSimilarityService(ILogger<CosineSimilarityService> logger) : ICosineSimilarityService
{
    private const int NumberOfChunksToTake = 3;
    private const double MinSimilarityCutoff = 0.65;
    
    private readonly ILogger<CosineSimilarityService> _logger = logger;

    private record ChunkWithCosineSimilarity(string CVChunk, double CosineSimilarity);
    
    public string[] GetSimilarCVChunks(CosineSimilarityInput input)
    {
        var chunksWithCosineSimilarity = CollectChunksWithCosineSimilarities(input);
        var mostSimilarChunks = chunksWithCosineSimilarity
            .OrderByDescending(x => x.CosineSimilarity);

        var mostSimilarChunkTexts = mostSimilarChunks
            .Take(NumberOfChunksToTake)
            .Select(x => x.CVChunk);
        
        return [.. mostSimilarChunkTexts];
    }

    private IEnumerable<ChunkWithCosineSimilarity> CollectChunksWithCosineSimilarities(CosineSimilarityInput input)
    {
        foreach (var chunk in input.ChunkEmbeddings)
        {
            var cosineSimilarity = CalculateCosineSimilarity(input.QueryEmbedding, chunk.Embedding);

            _logger.LogInformation("Cosine similarity for chunk {chunkId}: {cosineSimilarity}. Request {requestId}",
                chunk.Id, cosineSimilarity, input.requestId);
            
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