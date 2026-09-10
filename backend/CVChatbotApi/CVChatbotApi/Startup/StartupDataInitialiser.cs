using System.Text.Json;
using CVChatbotApi.DataStore;
using Shared.Models;

namespace CVChatbotApi.Startup;

public class StartupDataInitialiser(IDataStore dataStore) : IHostedService
{
    private readonly IDataStore _dataStore = dataStore;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead("./chunkEmbeddings.json");
        var chunkEmbeddings = JsonSerializer.Deserialize<ChunkEmbeddingJsonRecord[]>(stream)
            ?? throw new InvalidOperationException("Unable to deserialise chunk embeddings");
        
        _dataStore.Initialize(chunkEmbeddings);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}