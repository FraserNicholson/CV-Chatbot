using CVChatbotApi.Contract;
using CVChatbotApi.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace CVChatbotApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CVChatbotController(ICVQueryHandler handler) : ControllerBase
{
    private readonly ICVQueryHandler _handler = handler;

    [HttpPost("ask-question")]
    public Task<CVQueryResponse> AskQuestion([FromBody] CVQueryRequest request, CancellationToken cancellationToken)
    {
        return _handler.Handle(request, cancellationToken);
    }
}