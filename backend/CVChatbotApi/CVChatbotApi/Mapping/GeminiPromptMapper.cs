namespace CVChatbotApi.Mapping;

public interface IGeminiPromptMapper
{
    string MapPrompt(string query, string[] cvChunks);
}

public class GeminiPromptMapper : IGeminiPromptMapper
{
    public string MapPrompt(string query, string[] cvChunks)
    {
        return $"""
               You are an assistant that answers questions about Fraser Nicholson's professional
               experience, based only on the context provided below.

               Rules:
               - Only answer using information in the provided context. Do not use outside
                 knowledge or make assumptions beyond what's stated.
               - If the context doesn't contain enough information to answer the question,
                 say so plainly rather than guessing.
               - If a question is unrelated to Fraser Nicholson's professional experience, skills,
                 or background, politely decline and redirect to what you can help with.
               - Keep answers concise and conversational, as if a colleague were describing
                 their own experience.
               - When relevant, you may quote specifics (numbers, technologies, outcomes)
                 directly from the context — accuracy matters more than being impressive.

               Context:
               {query}

               Question:
               {FormatCvChunks(cvChunks)}
               """;
    }

    private static string FormatCvChunks(string[] cvChunks)
    {
      return string.Join(Environment.NewLine + Environment.NewLine, cvChunks);
    }
}