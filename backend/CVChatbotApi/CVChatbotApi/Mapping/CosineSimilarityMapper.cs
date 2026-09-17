using CVChatbotApi.Models;
using Shared.Models;

namespace CVChatbotApi.Mapping;

public interface ICosineSimilarityMapper
{
    CosineSimilarityInput MapInput( double[] queryEmbedding, ChunkEmbeddingJsonRecord[] chunkEmbeddings, Guid requestId);
}

public class CosineSimilarityMapper : ICosineSimilarityMapper
{
    public CosineSimilarityInput MapInput(double[] queryEmbedding, ChunkEmbeddingJsonRecord[] chunkEmbeddings, Guid requestId)
    {
        var mappedChunkEmbeddings = chunkEmbeddings.Select(MapChunkEmbedding);
        
        return new CosineSimilarityInput(queryEmbedding, [.. mappedChunkEmbeddings], requestId);
    }

    private static ChunkEmbedding MapChunkEmbedding(ChunkEmbeddingJsonRecord chunkEmbeddingJsonRecord)
    {
        return new ChunkEmbedding(chunkEmbeddingJsonRecord.Id, chunkEmbeddingJsonRecord.Text,
            chunkEmbeddingJsonRecord.Embedding);
    }

}