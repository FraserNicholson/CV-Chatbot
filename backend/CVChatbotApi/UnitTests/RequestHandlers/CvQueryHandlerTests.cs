using AutoFixture;
using CVChatbotApi.Contract;
using CVChatbotApi.DataStore;
using CVChatbotApi.Mapping;
using CVChatbotApi.Models;
using CVChatbotApi.RequestHandlers;
using CVChatbotApi.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shared.Models;
using Shared.Services;

namespace UnitTests.RequestHandlers;

public class CvQueryHandlerTests
{
    private readonly IGeminiEmbeddingService _embeddingService = Substitute.For<IGeminiEmbeddingService>();
    private readonly IDataStore _dataStore = Substitute.For<IDataStore>();
    private readonly ICosineSimilarityMapper _cosineSimilarityMapper = Substitute.For<ICosineSimilarityMapper>();
    private readonly ICosineSimilarityService _cosineSimilarityService = Substitute.For<ICosineSimilarityService>();
    private readonly IGeminiPromptService _promptService = Substitute.For<IGeminiPromptService>();

    private readonly Fixture _fixture = new();
    
    private readonly CvQueryHandler _sut;

    public CvQueryHandlerTests()
    {
        _sut = new CvQueryHandler(
            Substitute.For<ILogger<CvQueryHandler>>(),
            _embeddingService,
            _dataStore,
            _cosineSimilarityMapper,
            _cosineSimilarityService,
            _promptService);
    }

    [Fact]
    public async Task Handle_GivenNoSimilarCvChunksFound_ReturnsRelevantResponse()
    {
        _cosineSimilarityService.GetSimilarCVChunks(Arg.Any<CosineSimilarityInput>())
            .Returns([]);

        var request = new CVQueryRequest { Query = "my query" };
        
        var response = await _sut.Handle(request, CancellationToken.None);

        response.Response.Should().Be("Unable to find any relevant information for your query");
        
        await _embeddingService.Received(1).GetQueryEmbedding("my query", Arg.Any<CancellationToken>());
        _dataStore.Received(1).GetData();
        _cosineSimilarityMapper.Received(1).MapInput(Arg.Any<double[]>(), Arg.Any<ChunkEmbeddingJsonRecord[]>(), Arg.Any<Guid>());
        _cosineSimilarityService.Received(1).GetSimilarCVChunks(Arg.Any<CosineSimilarityInput>());

        await _promptService.DidNotReceive().GetGeneratedContent(Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_GivenSimilarCvChunksFound_ReturnsRelevantResponse()
    {
        var request = new CVQueryRequest { Query = "my query" };
        double[] queryEmbedding = [1, 1, 1];
        var chunkEmbeddingRecords = _fixture.Create<ChunkEmbeddingJsonRecord[]>();
        var cosineSimilarityInput = _fixture.Create<CosineSimilarityInput>();
        string[] similarCvChunks = ["chunk1", "chunk2", "chunk3"];
        const string generatedContent = "Generated custom content";

        _embeddingService.GetQueryEmbedding(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(queryEmbedding);
        _dataStore.GetData().Returns(chunkEmbeddingRecords);
        
        _cosineSimilarityMapper.MapInput(Arg.Any<double[]>(), Arg.Any<ChunkEmbeddingJsonRecord[]>(), Arg.Any<Guid>())
            .Returns(cosineSimilarityInput);
        _cosineSimilarityService.GetSimilarCVChunks(Arg.Any<CosineSimilarityInput>())
            .Returns(similarCvChunks);

        _promptService.GetGeneratedContent(Arg.Any<string>(), Arg.Any<string[]>(), Arg.Any<CancellationToken>())
            .Returns(generatedContent);
        
        var response = await _sut.Handle(request, CancellationToken.None);

        response.Response.Should().Be(generatedContent);
        
        await _embeddingService.Received(1).GetQueryEmbedding("my query", Arg.Any<CancellationToken>());
        _dataStore.Received(1).GetData();
        _cosineSimilarityMapper.Received(1).MapInput(queryEmbedding, chunkEmbeddingRecords, Arg.Any<Guid>());
        _cosineSimilarityService.Received(1).GetSimilarCVChunks(cosineSimilarityInput);
        await _promptService.Received(1).GetGeneratedContent("my query", similarCvChunks, Arg.Any<CancellationToken>());
    }
}