using System.Collections.Concurrent;
using Shared.Models;

namespace CVChatbotApi.DataStore;

public interface IDataStore
{
    void Initialize(ChunkEmbeddingJsonRecord[] data);
    ChunkEmbeddingJsonRecord[] GetData();
}

public class InMemoryDataStore : IDataStore
{
    private readonly ConcurrentBag<ChunkEmbeddingJsonRecord> _data = [];

    public void Initialize(ChunkEmbeddingJsonRecord[] data)
    {
        if (!_data.IsEmpty)  throw new InvalidOperationException("In memory data has already been initialised");
        
        foreach (var item in data)
        {
            _data.Add(item);
        }
    }

    public ChunkEmbeddingJsonRecord[] GetData()
    {
        if (_data.IsEmpty) throw new InvalidOperationException("In memory data has not been initialised");

        return [.. _data];
    }
}