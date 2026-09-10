using CVChatbotApi.DataStore;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace CVChatbotApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CVChatbotController(IDataStore dataStore) : ControllerBase
{
    private readonly IDataStore _dataStore = dataStore;

    [HttpPost("ask-question")]
    public ChunkEmbeddingJsonRecord[] AskQuestion()
    {
        return _dataStore.GetData();
    }
}