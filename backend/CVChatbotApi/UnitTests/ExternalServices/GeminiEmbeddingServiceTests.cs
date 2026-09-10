using AutoFixture;
using FluentAssertions;
using Google.GenAI.Types;
using NSubstitute;
using Shared.ExternalServices;

namespace UnitTests.ExternalServices;

public class GeminiEmbeddingServiceTests
{
    private readonly IGeminiHttpClient _geminiHttpClientSub = Substitute.For<IGeminiHttpClient>();
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task GetChunkEmbeddings_GivenValidGeminiResponse_ShouldReturnChunkEmbeddings()
    {
        string[] chunks = ["chunk1", "chunk2", "chunk3"];
        var firstTask = Task.FromResult((chunks[0], ConstructChunkEmbeddingResponse([1, 1, 1])));
        var secondTask = Task.FromResult((chunks[1], ConstructChunkEmbeddingResponse([2, 2, 2])));
        var thirdTask = Task.FromResult((chunks[2], ConstructChunkEmbeddingResponse([3, 3, 3])));

        Task<(string, EmbedContentResponse)>[] geminiResponse = [firstTask, secondTask, thirdTask];
        
        _geminiHttpClientSub
            .GetChunkEmbeddings(Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(geminiResponse);
        
        var sut = new GeminiEmbeddingService(_geminiHttpClientSub);
        
        var candidate = await sut.GetChunkEmbeddings(chunks, CancellationToken.None);
        
        candidate.Should().HaveCount(geminiResponse.Length);
        candidate.First().Chunk.Should().Be("chunk1");
        candidate.First().Embedding.Should().Equal(1, 1, 1);
        candidate.Skip(1).First().Chunk.Should().Be("chunk2");
        candidate.Skip(1).First().Embedding.Should().Equal(2, 2, 2);
        candidate.Last().Chunk.Should().Be("chunk3");
        candidate.Last().Embedding.Should().Equal(3, 3, 3);
    }

    [Fact]
    public async Task GetQueryEmbeddings_GivenValidGeminiResponse_ShouldReturnQueryEmbeddings()
    {
        var geminiResponse = ConstructChunkEmbeddingResponse([1, 2, 3]);
        _geminiHttpClientSub.GetQueryEmbedding(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(geminiResponse);
        
        var sut = new GeminiEmbeddingService(_geminiHttpClientSub);
        
        var candidate = await sut.GetQueryEmbedding("query", CancellationToken.None);

        candidate.Should().Equal(1, 2, 3);
        
        await _geminiHttpClientSub.Received().GetQueryEmbedding("query", Arg.Any<CancellationToken>());
    }

    private EmbedContentResponse ConstructChunkEmbeddingResponse(List<double> values)
    {
        var embedding = _fixture.Build<ContentEmbedding>()
            .With(e => e.Values, values)
            .Create();
        
        return _fixture.Build<EmbedContentResponse>()
            .With(r => r.Embeddings, [embedding])
            .Create();
    }
}