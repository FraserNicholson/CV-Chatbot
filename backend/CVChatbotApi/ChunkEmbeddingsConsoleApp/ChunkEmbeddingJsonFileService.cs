using System.Text.Json;
using Microsoft.Extensions.Options;
using Shared.ExternalServices;
using Shared.Models;

namespace ChunkEmbeddingsConsoleApp;

public interface IChunkEmbeddingJsonFileService
{
    Task CreateChunkEmbeddingJsonFile();
}

public class ChunkEmbeddingJsonFileService(IGeminiEmbeddingService embeddingService, IOptions<OutputOptions> outputOptions)
    : IChunkEmbeddingJsonFileService
{
    private readonly IGeminiEmbeddingService _embeddingService = embeddingService;
    private readonly OutputOptions _outputOptions = outputOptions.Value;

    public async Task CreateChunkEmbeddingJsonFile()
    {
        var chunks = GetChunks();

        var chunksWithEmbeddings = await _embeddingService.GetChunkEmbeddings(chunks, CancellationToken.None);

        var jsonRecords = chunksWithEmbeddings
            .Select(chunkWithEmbedding => new ChunkEmbeddingJsonRecord(
                "", // Will manually add these values for now
                "",
                "",
                chunkWithEmbedding.Chunk,
                chunkWithEmbedding.Embedding));
        
        var content = JsonSerializer.Serialize(jsonRecords);
        await File.WriteAllTextAsync(_outputOptions.FilePath, content);
    }

    private static string[] GetChunks()
    {
        var fileNames = Directory.GetFiles("CVChunks");

        var chunks = new List<string>();

        foreach (var fileName in fileNames)
        {
            chunks.Add(File.ReadAllText(fileName));
        }
        
        return[.. chunks];
    }
}