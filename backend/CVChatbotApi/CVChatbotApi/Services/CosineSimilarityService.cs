using CVChatbotApi.Models;

namespace CVChatbotApi.Services;

public interface ICosineSimilarityService
{
    string[] GetSimilarCvChunks(CosineSimilarityInput input);
}

/// <summary>
/// Receives query, query embedding, as well as cv chunks and their embeddings.
/// Calculates cosine similarity between query and each cv chunk, then returns
/// top N cv chunks that satisfy the minimum similarity cuttoff, as well as
/// an always present index chunk.
/// </summary>
public class CosineSimilarityService(ILogger<CosineSimilarityService> logger) : ICosineSimilarityService
{
    private const int NumberOfChunksToTake = 3;
    private const double MinSimilarityCutoff = 0.5;
    private const string ChunkIdToAlwaysReturn = "corpus-index";
    
    private readonly ILogger<CosineSimilarityService> _logger = logger;

    private record ChunkWithCosineSimilarity(string CvChunk, double CosineSimilarity);
    
    public string[] GetSimilarCvChunks(CosineSimilarityInput input)
    {
        var chunksWithCosineSimilarity = CollectChunksWithCosineSimilarities(input);
        var mostSimilarChunks = chunksWithCosineSimilarity
            .OrderByDescending(x => x.CosineSimilarity);

        var mostSimilarChunkTexts = mostSimilarChunks
            .Take(NumberOfChunksToTake)
            .Select(x => x.CvChunk);

        var chunkToAlwaysReturn = input.ChunkEmbeddings.SingleOrDefault(c => c.Id == ChunkIdToAlwaysReturn) ??
                                  throw new InvalidOperationException($"Chunk with Id {ChunkIdToAlwaysReturn} not found");
        var chunksToReturn = mostSimilarChunkTexts.Append(chunkToAlwaysReturn.CvChunk);
        
        return [.. chunksToReturn];
    }

    private IEnumerable<ChunkWithCosineSimilarity> CollectChunksWithCosineSimilarities(CosineSimilarityInput input)
    {
        foreach (var chunk in input.ChunkEmbeddings)
        {
            // We always include this chunk, but add it on at the end
            if (chunk.Id == ChunkIdToAlwaysReturn) continue;
            
            var cosineSimilarity = CalculateCosineSimilarity(input.QueryEmbedding, chunk.Embedding);

            _logger.LogInformation("Cosine similarity for chunk {chunkId}: {cosineSimilarity}. Request {requestId}",
                chunk.Id, cosineSimilarity, input.requestId);
            
            if (cosineSimilarity < MinSimilarityCutoff) continue;
            
            yield return new ChunkWithCosineSimilarity(chunk.CvChunk, cosineSimilarity);
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