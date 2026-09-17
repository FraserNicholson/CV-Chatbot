using CVChatbotApi.Contract;
using CVChatbotApi.DataStore;
using CVChatbotApi.Mapping;
using CVChatbotApi.Services;
using Shared.Services;

namespace CVChatbotApi.RequestHandlers;

public interface ICvQueryHandler
{
    Task<CvQueryResponse> Handle(CvQueryRequest request, CancellationToken cancellationToken);
}

public class CvQueryHandler(
    ILogger<CvQueryHandler> logger,
    IGeminiEmbeddingService embeddingService,
    IDataStore dataStore,
    ICosineSimilarityMapper cosineSimilarityMapper,
    ICosineSimilarityService cosineSimilarityService,
    IGeminiPromptService promptService) : ICvQueryHandler
{
    private readonly ILogger<CvQueryHandler> _logger = logger;
    private readonly IGeminiEmbeddingService _embeddingService = embeddingService;
    private readonly IDataStore _dataStore = dataStore;
    private readonly ICosineSimilarityMapper _cosineSimilarityMapper = cosineSimilarityMapper;
    private readonly ICosineSimilarityService _cosineSimilarityService = cosineSimilarityService;
    private readonly IGeminiPromptService _promptService = promptService;

    public async Task<CvQueryResponse> Handle(CvQueryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Query)) throw new InvalidOperationException("No query provided");

        var requestId = Guid.NewGuid();

        _logger.LogInformation("Received CV Query Request with Id {requestId} with Query: {query}", requestId,
            request.Query);
        
        var queryEmbedding = await _embeddingService.GetQueryEmbedding(request.Query, cancellationToken);
        var cvChunkEmbeddings = _dataStore.GetData();
        
        var cosineSimilarityInput =
            _cosineSimilarityMapper.MapInput(queryEmbedding, cvChunkEmbeddings, requestId);
        var similarCvChunks = _cosineSimilarityService.GetSimilarCvChunks(cosineSimilarityInput);

        if (similarCvChunks.Length == 0)
        {
            _logger.LogWarning("No similar CV chunks found for request {requestId}", requestId);
            return new CvQueryResponse { Response = "Unable to find any relevant information for your query" };
        }

        var geminiPromptResponse =
            await _promptService.GetGeneratedContent(request.Query, similarCvChunks, cancellationToken);
        
        return new CvQueryResponse
        {
            Response = geminiPromptResponse
        };
    }
}