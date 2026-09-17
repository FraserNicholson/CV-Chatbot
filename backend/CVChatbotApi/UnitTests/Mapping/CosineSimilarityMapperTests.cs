using CVChatbotApi.Mapping;
using FluentAssertions;
using Shared.Models;

namespace UnitTests.Mapping;

public class CosineSimilarityMapperTests
{
    [Fact]
    public void MapInput_CorrectlyMapsInput()
    {
        var sut = new CosineSimilarityMapper();

        double[] queryEmbedding = [1, 2, 3, 4];
        ChunkEmbeddingJsonRecord[] chunkEmbeddingJsonRecords =
        [
            new("1", "", "", "chunk text 1", [1, 1, 1, 1]),
            new("2", "", "", "chunk text 2", [0, 1, -0.1, 9]),
            new("3", "", "", "chunk text 3", [0.1, -5, -1, 1])
        ];
        var requestId = Guid.NewGuid();
        
        var candidate = sut.MapInput(queryEmbedding,  chunkEmbeddingJsonRecords, requestId);
        
        candidate.requestId.Should().Be(requestId);
        candidate.QueryEmbedding.Should().Equal(queryEmbedding);
        candidate.ChunkEmbeddings.Should().HaveCount(3);
        
        candidate.ChunkEmbeddings.First().Id.Should().Be("1");
        candidate.ChunkEmbeddings.First().CVChunk.Should().Be("chunk text 1");
        candidate.ChunkEmbeddings.First().Embedding.Should().Equal(1, 1, 1, 1);
        
        candidate.ChunkEmbeddings.Skip(1).First().Id.Should().Be("2");
        candidate.ChunkEmbeddings.Skip(1).First().CVChunk.Should().Be("chunk text 2");
        candidate.ChunkEmbeddings.Skip(1).First().Embedding.Should().Equal(0, 1, -0.1, 9);
        
        candidate.ChunkEmbeddings.Last().Id.Should().Be("3");
        candidate.ChunkEmbeddings.Last().CVChunk.Should().Be("chunk text 3");
        candidate.ChunkEmbeddings.Last().Embedding.Should().Equal(0.1, -5, -1, 1);
    }
}