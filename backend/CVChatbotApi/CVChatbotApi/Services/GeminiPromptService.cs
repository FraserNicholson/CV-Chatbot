using CVChatbotApi.Mapping;
using Shared.Services;

namespace CVChatbotApi.Services;

public interface IGeminiPromptService
{
    Task<string> GetGeneratedContent(string query, string[] cvChunks, CancellationToken cancellationToken);
}

public class GeminiPromptService(IGeminiPromptMapper geminiPromptMapper, IGeminiHttpClient geminiHttpClient) : IGeminiPromptService
{
    private readonly IGeminiPromptMapper _geminiPromptMapper = geminiPromptMapper;
    private readonly IGeminiHttpClient _geminiHttpClient = geminiHttpClient;

    public async Task<string> GetGeneratedContent(string query, string[] cvChunks, CancellationToken cancellationToken)
    {
        var prompt = _geminiPromptMapper.MapPrompt(query, cvChunks);
        var generatedContent = await _geminiHttpClient.GenerateContent(prompt, cancellationToken);

        return generatedContent.Candidates?[0].Content?.Parts?[0].Text ??
               throw new InvalidOperationException("No content returned by gemini");
    }
}