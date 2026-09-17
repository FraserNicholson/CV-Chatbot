using CVChatbotApi.Models;
using Shared.Models;

namespace CVChatbotApi.Mapping;

public interface ICosineSimilarityMapper
{
    CosineSimilarityInput MapInput(string query, double[] queryEmbedding, ChunkEmbeddingJsonRecord[] chunkEmbeddings, Guid requestId);
}

public class CosineSimilarityMapper : ICosineSimilarityMapper
{
    public CosineSimilarityInput MapInput(string query, double[] queryEmbedding,
        ChunkEmbeddingJsonRecord[] chunkEmbeddings, Guid requestId)
    {
        var mappedChunkEmbeddings = chunkEmbeddings.Select(MapChunkEmbedding);
        
        return new CosineSimilarityInput(query, queryEmbedding, [.. mappedChunkEmbeddings], requestId);
    }

    private static ChunkEmbedding MapChunkEmbedding(ChunkEmbeddingJsonRecord chunkEmbeddingJsonRecord)
    {
        return new ChunkEmbedding(chunkEmbeddingJsonRecord.Id, chunkEmbeddingJsonRecord.Text,
            chunkEmbeddingJsonRecord.Embedding);
    }

}