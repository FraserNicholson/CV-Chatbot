using CVChatbotApi.Contract;
using CVChatbotApi.RequestHandlers;
using Microsoft.AspNetCore.Mvc;

namespace CVChatbotApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CvChatbotController(ICvQueryHandler handler) : ControllerBase
{
    private readonly ICvQueryHandler _handler = handler;

    [HttpPost("ask-question")]
    public Task<CvQueryResponse> AskQuestion([FromBody] CvQueryRequest request, CancellationToken cancellationToken)
    {
        return _handler.Handle(request, cancellationToken);
    }
}