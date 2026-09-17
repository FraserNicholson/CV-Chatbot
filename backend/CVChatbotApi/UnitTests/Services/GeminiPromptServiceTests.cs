using CVChatbotApi.Mapping;
using CVChatbotApi.Services;
using FluentAssertions;
using Google.GenAI.Types;
using NSubstitute;
using Shared.Services;

namespace UnitTests.Services;

public class GeminiPromptServiceTests
{
   private readonly IGeminiPromptMapper _mapperSub = Substitute.For<IGeminiPromptMapper>();
   private readonly IGeminiHttpClient _httpClientSub = Substitute.For<IGeminiHttpClient>();

   private readonly GeminiPromptService _sut;

   public GeminiPromptServiceTests()
   {
      _sut = new GeminiPromptService(_mapperSub, _httpClientSub);
   }

   [Theory, MemberData(nameof(InvalidResponseData))]
   public void GetGeneratedContent_GivenInvalidResponse_ThrowsException(GenerateContentResponse invalidResponse)
   {
      _httpClientSub.GenerateContent(Arg.Any<string>(), Arg.Any<CancellationToken>())
         .Returns(invalidResponse);
      
      var action = () => _sut.GetGeneratedContent("", [], CancellationToken.None);
      
      action.Should().ThrowAsync<InvalidOperationException>()
         .WithMessage("No content returned by gemini");
   }

   [Fact]
   public async Task GetGeneratedContent_GivenValidResponse_ReturnsText()
   {
      var validResponse = new GenerateContentResponse
      {
         Candidates = [new Candidate { Content = new Content { Parts = [new Part { Text = "Custom response"}] } }]
      };
      
      _mapperSub.MapPrompt(Arg.Any<string>(), Arg.Any<string[]>()).Returns("Prompt");
      _httpClientSub.GenerateContent(Arg.Any<string>(), Arg.Any<CancellationToken>())
         .Returns(validResponse);

      var result = await _sut.GetGeneratedContent("query", ["chunk"], CancellationToken.None);
      
      result.Should().Be("Custom response");

      _mapperSub.Received(1).MapPrompt("query", Arg.Is<string[]>(x => ((IEnumerable<string>)x).Contains("chunk")));
      await _httpClientSub.Received(1).GenerateContent("Prompt", Arg.Any<CancellationToken>());
   }
   
   public static TheoryData<GenerateContentResponse> InvalidResponseData()
   {
      return
      [
         new GenerateContentResponse(),
         new GenerateContentResponse
         {
            Candidates = []
         },

         new GenerateContentResponse
         {
            Candidates = [new Candidate()]
         },

         new GenerateContentResponse
         {
            Candidates = [new Candidate { Content = new Content { Parts = [] } }]
         }
      ];
   }
}