using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;

namespace Shared.ExternalServices;

public interface IGeminiHttpClient
{
    IEnumerable<Task<(string chunk, EmbedContentResponse response)>> GetChunkEmbeddings(string[] chunks,
        CancellationToken cancellationToken);
    
    Task<EmbedContentResponse> GetQueryEmbedding(string query, CancellationToken cancellationToken);
}

public class GeminiHttpClient : IGeminiHttpClient
{
    private readonly GeminiOptions _geminiOptions;

    public GeminiHttpClient(IOptions<GeminiOptions> geminiOptions)
    {
        _geminiOptions = geminiOptions.Value;
    }

    public IEnumerable<Task<(string chunk, EmbedContentResponse response)>> GetChunkEmbeddings(string[] chunks,
        CancellationToken cancellationToken)
    {
        var client = new Client(apiKey: _geminiOptions.ApiKey);
        var embeddingTasks = chunks.Select(async chunk =>
        {
            var response = await client.Models.EmbedContentAsync(
                model: "gemini-embedding-001",
                contents: chunk,
                config: new EmbedContentConfig
                {
                    // Optimises for documents/chunks
                    TaskType = "RETRIEVAL_DOCUMENT"
                },
                cancellationToken: cancellationToken);

            return (chunk, response);
        });

        return embeddingTasks;
    }

    public Task<EmbedContentResponse> GetQueryEmbedding(string query, CancellationToken cancellationToken)
    {
        var client = new Client(apiKey: _geminiOptions.ApiKey);

        var response = client.Models.EmbedContentAsync(
            model: "gemini-embedding-001", 
            contents: query, 
            config: new EmbedContentConfig 
            { 
                // Optimises for documents/chunks
                TaskType = "RETRIEVAL_QUERY" 
            }, 
            cancellationToken: cancellationToken);
        
        return response;
    }
}