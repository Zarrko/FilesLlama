using System.Text;
using System.Text.Json;
using FilesLlama.Contracts.Embeddings;
using FilesLlama.Ingestion.Embeddings;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;
using Document = FilesLlama.Contracts.VectorStore.Document;
using Schema = NRedisStack.Search.Schema;

namespace FilesLlama.Ingestion.Repository;

public class RedisVectorStore : IVectorStore
{
    private const string VectorPrefix = "files:";
    private readonly IEmbeddingsService _embeddingsService;
    private readonly ISearchCommands _ft;
    private readonly string _index;
    private readonly IDatabase _db;

    public RedisVectorStore(IConnectionMultiplexer connectionMultiplexer, IEmbeddingsService embeddingsService, string index)
    {
        _embeddingsService = embeddingsService;
        _index = index;
        _db = connectionMultiplexer.GetDatabase();
        _ft = _db.FT();
    }

    private bool IsIndexInVectorStore(string index)
    {
        try
        {
            RedisValue idx = index;
            var existingIdx = _ft.Info(idx);
            return !string.IsNullOrEmpty(existingIdx.IndexName);
        }
        catch (Exception ex)
        {
            // Ironically throws an exception if index is not available
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
            ["TYPE"] = "FLOAT32",
            ["DIM"] = "4096", // The size of embedding array from Llama is 4096.
            ["DISTANCE_METRIC"] = "L2",
        };
        var schema = new Schema();
        schema.AddTextField("content");
        schema.AddTextField("metadata");
        schema.AddVectorField("content_vector", Schema.VectorField.VectorAlgo.FLAT, attributes);
        
        var parameters = new FTCreateParams().On(IndexDataType.HASH).Prefix(VectorPrefix);
        _ft.Create(index, parameters, schema);
    }

    public async Task AddDocuments(List<string> docs, List<Dictionary<string, string>> meta)
    {
        CreateIndex(_index);
        
        // ToDo: Prevent adding existing docs. How do we do this?
        var documentList = new List<Document>(docs.Count);
        for (var i = 0; i < docs.Count; i++)
        {
            // ToDo: EmbeddingService accepts multiples requests. Parallelize.
            var vectorList = await _embeddingsService.EmbedDocuments(new List<GetEmbeddingRequest>(1)
                { new() { Content = docs[i] } });
            
            // ToDo: What should we pass as Metadata for the documents?
            var metadata = new Dictionary<string, string>(0);
            var vectors = vectorList[0].Embedding.Select(d => (float)d).ToArray();
            var document = new Document()
            {
                Content = docs[i],
                Meta = metadata,
                Vector = vectors,
            };
            
            documentList.Add(document);
        }

        var cnt = 0;
        foreach (var document in documentList)
        {
            var vectorFloats = document.Vector;

            var contentBytes = Encoding.UTF8.GetBytes(document.Content);
            
            _db.HashSet($"{VectorPrefix}{cnt}", new[]
            {
                new HashEntry("content", contentBytes),
                new HashEntry("metadata", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(document.Meta))),
                new HashEntry("content_vector", vectorFloats.SelectMany(BitConverter.GetBytes).ToArray())
            });

            cnt += 1;
        }
    }

    public async Task<List<Document>> SimilaritySearch(string text, int k)
    {
        var q = await PrepareQuery(text, k);

        var searchResult = _ft.Search(_index, q);
        var docs = searchResult.Documents;
        if (docs.Count < 1)
        {
            return new List<Document>(0);
        }

        var retrievedDocs = new List<Document>();
        foreach (var doc in docs)
        {
            var retrievedDoc = new Document()
            {
                Content = doc["content"].ToString(),
                Meta = JsonSerializer.Deserialize<Dictionary<string, string>>(doc["metadata"])
                // ToDo: Add Vectors too?
            };
            
            retrievedDocs.Add(retrievedDoc);
        }

        return retrievedDocs;
    }

    private async Task<NRedisStack.Search.Query> PrepareQuery(string text, int k)
    {
        var embedding = await _embeddingsService.EmbedQuery(new GetEmbeddingRequest() { Content = text });
        var vector = embedding.Embedding.Select(d => (float)d).ToArray();
        
        return new NRedisStack.Search.Query($"*=>[KNN {k} @content_vector $query_vector AS vector_score]")
            .AddParam("query_vector", vector.SelectMany(BitConverter.GetBytes).ToArray())
            .Dialect(2)
            .ReturnFields("content", "metadata", "vector_score")
            .SetSortBy("vector_score")
            .SetWithScores();
    }
}