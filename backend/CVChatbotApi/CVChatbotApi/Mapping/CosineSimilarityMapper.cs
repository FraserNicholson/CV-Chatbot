using CVChatbotApi.Models;
using Shared.Models;

namespace CVChatbotApi.Mapping;

public interface ICosineSimilarityMapper
{
    CosineSimilarityInput MapInput(string query, double[] queryEmbedding, ChunkEmbeddingJsonRecord[] chunkEmbeddings);
}

public class CosineSimilarityMapper : ICosineSimilarityMapper
{
    public CosineSimilarityInput MapInput(string query, double[] queryEmbedding,
        ChunkEmbeddingJsonRecord[] chunkEmbeddings)
    {
        var mappedChunkEmbeddings = chunkEmbeddings.Select(MapChunkEmbedding);
        
        return new CosineSimilarityInput(query, queryEmbedding, [.. mappedChunkEmbeddings]);
    }

    private static ChunkEmbedding MapChunkEmbedding(ChunkEmbeddingJsonRecord chunkEmbeddingJsonRecord)
    {
        return new ChunkEmbedding(chunkEmbeddingJsonRecord.Text, chunkEmbeddingJsonRecord.Embedding);
    }

}