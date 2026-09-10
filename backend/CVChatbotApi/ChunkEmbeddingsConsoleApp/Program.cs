using ChunkEmbeddingsConsoleApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.ExternalServices;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var serviceProvider = new ServiceCollection()
    .AddTransient<IGeminiEmbeddingService, GeminiEmbeddingService>()
    .AddTransient<IGeminiHttpClient, GeminiHttpClient>()
    .AddTransient<IChunkEmbeddingJsonFileService, ChunkEmbeddingJsonFileService>()
    .Configure<GeminiOptions>(configuration.GetSection("Gemini"))
    .Configure<OutputOptions>(configuration.GetSection("Output"))
    .BuildServiceProvider();

var enabled = configuration.GetSection("Enabled").Get<bool>();

if (enabled)
{
    Console.WriteLine("ChunkEmbeddingsConsoleApp is enabled. Fetching and storing chunk embeddings...");
    
    var service = serviceProvider.GetRequiredService<IChunkEmbeddingJsonFileService>();
    await service.CreateChunkEmbeddingJsonFile();
}
else
{
    Console.WriteLine("ChunkEmbeddingsConsoleApp is disabled.");
}