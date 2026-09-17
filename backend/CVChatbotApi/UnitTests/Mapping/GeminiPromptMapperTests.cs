using CVChatbotApi.Mapping;
using FluentAssertions;

namespace UnitTests.Mapping;

public class GeminiPromptMapperTests
{
    [Fact]
    public void MapPrompt_CorrectlyMapsPrompt()
    {
        const string expectedPrompt = """
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
                              chunk1
                              
                              chunk2
                              
                              chunk3

                              Question:
                              custom query please
                              """;

        var sut = new GeminiPromptMapper();
        
        var candidate = sut.MapPrompt("custom query please", ["chunk1", "chunk2", "chunk3"]);
        
        candidate.Should().BeEquivalentTo(expectedPrompt);
    }
}