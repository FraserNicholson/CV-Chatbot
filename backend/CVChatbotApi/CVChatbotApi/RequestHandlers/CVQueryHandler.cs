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
    ICosineSimilarityService cosineSimilarityService) : ICVQueryHandler
{
    private readonly IGeminiEmbeddingService _embeddingService = embeddingService;
    private readonly IDataStore _dataStore = dataStore;
    private readonly ICosineSimilarityMapper _cosineSimilarityMapper = cosineSimilarityMapper;
    private readonly ICosineSimilarityService _cosineSimilarityService = cosineSimilarityService;

    public async Task<CVQueryResponse> Handle(CVQueryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Query)) throw new InvalidOperationException("No query provided");
        
        // Get's all embeddings
        var queryEmbedding = await _embeddingService.GetQueryEmbedding(request.Query, cancellationToken);
        var cvChunkEmbeddings = _dataStore.GetData();
        
        // Get's relevant in memory embedded chunks based on query
        var cosineSimilarityInput = _cosineSimilarityMapper.MapInput(request.Query, queryEmbedding, cvChunkEmbeddings);
        var similarCVChunks = _cosineSimilarityService.GetSimilarCVChunks(cosineSimilarityInput);

        if (similarCVChunks.Length == 0)
            return new CVQueryResponse { Response = "Unable to find any relevant information for your query" };
        
        // Constructs final prompt
        // Sends off to gemini
        
        // Maps response
        
        return new CVQueryResponse
        {
            Response = ""
        };
    }
}