using FilesLlama.Ingestion.Embeddings;
using NRediSearch;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using StackExchange.Redis;
using Document = FilesLlama.Contracts.VectorStore.Document;
using Schema = NRedisStack.Search.Schema;

namespace FilesLlama.Ingestion.Repository;

public class RedisVectorStore : IVectorStore
{
    private static int DIM = 1536;
    private static int EMBEDDING_SIZE = 8192;
    private static string MD5 = "MD5";
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IEmbeddingsService _embeddingsService;
    private readonly IDatabase _db;
    private readonly ISearchCommands _ft;

    public RedisVectorStore(IConnectionMultiplexer connectionMultiplexer, IEmbeddingsService embeddingsService)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _embeddingsService = embeddingsService;
        _db = _connectionMultiplexer.GetDatabase();
        _ft = _db.FT();
    }

    private bool IsIndexInVectorStore(string index)
    {
        try
        {
            var existingIdx = _ft.Info(index);
            return !string.IsNullOrEmpty(existingIdx.IndexName);
        }
        catch (Exception ex)
        {
            // Log or handle the exception appropriately
            return false;
        }
    }

    private void CreateIndex(string index)
    {
        if (IsIndexInVectorStore(index))
        {
            return;
        }

        var attributes = new Dictionary<string, object>
        {
            { "TYPE", "FLOAT32" },
            { "DIM", DIM },
            { "DISTANCE_METRIC", "L2" },
        };
        var schema = new Schema();
        schema.AddTextField("content", 1.0);
        schema.AddTextField("metadata", 1.0);
        schema.AddVectorField("content_vector", Schema.VectorField.VectorAlgo.FLAT, attributes);
        
        var parameters = FTCreateParams.CreateParams().AddPrefix(index);
        _ft.Create(index, parameters, schema);
    }

    public Task AddDocuments(List<string> docs, List<Dictionary<string, string>> meta)
    {
        throw new NotImplementedException();
    }

    public Task<List<Document>> SimilaritySearch(string text, int k)
    {
        throw new NotImplementedException();
    }
}