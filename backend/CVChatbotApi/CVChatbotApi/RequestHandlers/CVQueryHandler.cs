using CVChatbotApi.Contract;
using CVChatbotApi.DataStore;
using CVChatbotApi.Mapping;
using CVChatbotApi.Services;
using Shared.ExternalServices;

namespace CVChatbotApi.RequestHandlers;

public interface ICVQueryHandler
{
    Task<CVQueryResponse> Handle(CVQueryRequest request, CancellationToken cancellationToken);
}

public class CVQueryHandler(
    IGeminiEmbeddingService embeddingService,
    IDataStore dataStore,
    ICosineSimilarityMapper cosineSimilarityMapper,
    ICosineSimilarityService cosineSimilarityService,
    ILogger<CVQueryHandler> logger) : ICVQueryHandler
{
    private readonly IGeminiEmbeddingService _embeddingService = embeddingService;
    private readonly IDataStore _dataStore = dataStore;
    private readonly ICosineSimilarityMapper _cosineSimilarityMapper = cosineSimilarityMapper;
    private readonly ICosineSimilarityService _cosineSimilarityService = cosineSimilarityService;
    private readonly ILogger<CVQueryHandler> _logger = logger;

    public async Task<CVQueryResponse> Handle(CVQueryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Query)) throw new InvalidOperationException("No query provided");

        var requestId = Guid.NewGuid();

        _logger.LogInformation("Received CV Query Request with Id {requestId} with Query: {query}", requestId,
            request.Query);
        
        // Get's all embeddings
        var queryEmbedding = await _embeddingService.GetQueryEmbedding(request.Query, cancellationToken);
        var cvChunkEmbeddings = _dataStore.GetData();
        
        // Get's relevant in memory embedded chunks based on query
        var cosineSimilarityInput =
            _cosineSimilarityMapper.MapInput(request.Query, queryEmbedding, cvChunkEmbeddings, requestId);
        var similarCVChunks = _cosineSimilarityService.GetSimilarCVChunks(cosineSimilarityInput);

        if (similarCVChunks.Length == 0)
        {
            _logger.LogWarning("No similar CV chunks found for request {requestId}", requestId);
            return new CVQueryResponse { Response = "Unable to find any relevant information for your query" };
        }
        
        // Constructs final prompt
        // Sends off to gemini
        
        // Maps response
        
        return new CVQueryResponse
        {
            Response = ""
        };
    }
}