using FilesLlama.Contracts.Schema;
using StackExchange.Redis;

namespace FilesLlama.Ingestion.Repository;

public class RedisVectorStore : IVectorStore
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    
    public RedisVectorStore(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public List<string> Add(List<BaseNode> nodes)
    {
        throw new NotImplementedException();
    }

    public void Delete(string refDocId)
    {
        throw new NotImplementedException();
    }

    public VectorStoreQueryResults Query(VectorStoreQuery query)
    {
        throw new NotImplementedException();
    }

    public void Persist(bool inBackground)
    {
        throw new NotImplementedException();
    }
}