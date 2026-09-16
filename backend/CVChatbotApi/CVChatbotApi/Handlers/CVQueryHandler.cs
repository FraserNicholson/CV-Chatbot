using CVChatbotApi.Contract;

namespace CVChatbotApi.Handlers;

public interface ICVQueryHandler
{
    Task<CVQueryResponse> Handle(CVQueryRequest request, CancellationToken cancellationToken);
}

public class CVQueryHandler : ICVQueryHandler
{
    public async Task<CVQueryResponse> Handle(CVQueryRequest request, CancellationToken cancellationToken)
    {
        // Get's embeddings from gemini API
        
        // Get's relevant in memory embedded chunks based on query
        
        // Constructs final prompt
        
        // Sends off to gemini
        
        // Maps response
        
        return new CVQueryResponse
        {
            Response = ""
        };
    }
}