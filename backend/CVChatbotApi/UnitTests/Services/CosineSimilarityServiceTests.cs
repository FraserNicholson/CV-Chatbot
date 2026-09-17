using CVChatbotApi.Models;
using CVChatbotApi.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace UnitTests.Services;

public class CosineSimilarityServiceTests
{
    private readonly CosineSimilarityService _sut = new(Substitute.For<ILogger<CosineSimilarityService>>());

    [Fact]
    public void GetSimilarCVChunks_GivenMixedInputs_ShouldOnlyReturnRelevantChunks()
    {
        double[] queryEmbedding = [1, 1];
        ChunkEmbedding[] chunkEmbeddings =
        [
            new("1", "Exact match chunk", [1, 1]),
            new("2", "Similar match chunk", [0.9, 0.1]),
            new("3", "Slightly unrelated chunk", [0.9, -0.1]),
            new("4", "Completely unrelated chunk", [0.9, -0.1]),
        ];
        
        var input = new CosineSimilarityInput(queryEmbedding, chunkEmbeddings, Guid.NewGuid());
        
        var result = _sut.GetSimilarCVChunks(input);

        result.Should().HaveCount(2);
        result.Should().Contain("Exact match chunk");
        result.Should().Contain("Similar match chunk");
    }
    
    // Should fail when N is changed in CosineSimilarityService
    [Fact]
    public void GetSimilarCVChunks_GivenRelatedInputs_ShouldOnlyReturnNRelevantChunks()
    {
        double[] queryEmbedding = [1, 1];
        ChunkEmbedding[] chunkEmbeddings =
        [
            new("1", "Exact match chunk", [1, 1]),
            new("2", "Similar match chunk", [0.9, 0.1]),
            new("3", "Another similar match chunk", [0.9, 0.05]),
            new("4", "Another similar chunk, but removed because more N similar chunks chosen", [0.9, 0]),
        ];
        
        var input = new CosineSimilarityInput(queryEmbedding, chunkEmbeddings, Guid.NewGuid());
        
        var result = _sut.GetSimilarCVChunks(input);

        result.Should().HaveCount(3);
        result.Should().Contain("Exact match chunk");
        result.Should().Contain("Similar match chunk");
        result.Should().Contain("Another similar match chunk");
    }
    
    [Fact]
    public void GetSimilarCVChunks_GivenUnrelatedInputs_ShouldNotReturnAnyChunks()
    {
        double[] queryEmbedding = [1, 1];
        ChunkEmbedding[] chunkEmbeddings =
        [
            new("1", "Slightly not related", [1, -0.5]),
            new("2", "Completely irrelevant", [1, -1]),
            new("3", "Completely opposite", [-1, -1]),
        ];
        
        var input = new CosineSimilarityInput(queryEmbedding, chunkEmbeddings, Guid.NewGuid());
        
        var result = _sut.GetSimilarCVChunks(input);

        result.Should().BeEmpty();
    }
}