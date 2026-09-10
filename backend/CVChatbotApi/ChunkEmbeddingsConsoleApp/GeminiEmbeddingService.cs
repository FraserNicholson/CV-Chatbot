using Shared.ExternalServices;

namespace ChunkEmbeddingsConsoleApp;

public record ChunkWithEmbedding(string Chunk, double[] Embedding);

public interface IGeminiEmbeddingService
{
    Task<ChunkWithEmbedding[]> GetChunkEmbeddings(string[] chunks, CancellationToken cancellationToken);
}

public class GeminiEmbeddingService(IGeminiHttpClient geminiHttpClient) : IGeminiEmbeddingService
{
    private readonly IGeminiHttpClient _geminiHttpClient = geminiHttpClient;

    public async Task<ChunkWithEmbedding[]> GetChunkEmbeddings(string[] chunks, CancellationToken cancellationToken)
    {
        var embedTasks = _geminiHttpClient.GetEmbeddings(chunks, cancellationToken);
        var embedResponses = await Task.WhenAll(embedTasks);

        var chunksWithEmbeddings = embedResponses
            .Select(tuple => new ChunkWithEmbedding(tuple.chunk, [.. tuple.response.Embeddings!.Single().Values!]));

        return [.. chunksWithEmbeddings];
    }
}