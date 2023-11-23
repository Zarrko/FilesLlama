using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FilesLlama.Contracts.Embeddings;
using FilesLlama.Ingestion.Embeddings;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using StackExchange.Redis;
using Document = FilesLlama.Contracts.VectorStore.Document;
using Schema = NRedisStack.Search.Schema;

namespace FilesLlama.Ingestion.Repository;

public class RedisVectorStore : IVectorStore
{
    private const int Dim = 1536;
    private static int EMBEDDING_SIZE = 8192;
    private static string MD5 = "MD5";
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
            { "DIM", Dim },
            { "DISTANCE_METRIC", "L2" },
        };
        var schema = new Schema();
        schema.AddTextField("content", 1.0);
        schema.AddTextField("metadata", 1.0);
        schema.AddVectorField("content_vector", Schema.VectorField.VectorAlgo.FLAT, attributes);
        
        var parameters = FTCreateParams.CreateParams().AddPrefix(index);
        _ft.Create(index, parameters, schema);
    }

    public async Task AddDocuments(List<string> docs, List<Dictionary<string, string>> meta)
    {
        CreateIndex(_index);
        
        // ToDo: Do we need to handle any exceptions below?
        var documentList = new List<Document>(docs.Count);
        for (var i = 0; i < docs.Count; i++)
        {
            var vectorList = await _embeddingsService.EmbedDocuments(new List<GetEmbeddingRequest>(1)
                { new() { Content = docs[i] } });
            var metadata = meta[i];
            var vectors = vectorList[0].Embedding;
            var document = new Document()
            {
                Content = docs[i],
                Meta = metadata,
                Vector = vectors,
            };
            
            documentList.Add(document);
        }

        foreach (var document in documentList)
        {
            var vectorFloats = document.Vector;

            var contentBytes = Encoding.UTF8.GetBytes(document.Content);
            using var h = new HMACMD5();
            var hash = h.ComputeHash(contentBytes);
            var key = _index + Encoding.Convert(Encoding.UTF8, Encoding.UTF8, hash);

            _db.HashSet(Encoding.UTF8.GetBytes(key), new[]
            {
                new HashEntry(Encoding.UTF8.GetBytes("content"), contentBytes),
                new HashEntry(Encoding.UTF8.GetBytes("metadata"), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(document.Meta))),
                new HashEntry(Encoding.UTF8.GetBytes("content_vector"), FloatArrayToByteArray(vectorFloats))
            });
        }
    }

    public async Task<List<Document>> SimilaritySearch(string text, int k)
    {
        var q = await PrepareQuery(text, k);

        var searchResult = this._ft.Search(_index, q);
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
                Meta = JsonSerializer.Deserialize<Dictionary<string, string>>(doc["meta"])
                // Add Vectors too?
            };
            
            retrievedDocs.Add(retrievedDoc);
        }

        return retrievedDocs;
    }
    
    private static byte[] FloatArrayToByteArray(float[] floatArray)
    {
        var byteArray = new byte[floatArray.Length * sizeof(float)];
        Array.Copy(floatArray, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    private async Task<NRedisStack.Search.Query> PrepareQuery(string text, int k)
    {
        var embedding = await _embeddingsService.EmbedQuery(new GetEmbeddingRequest() { Content = text });
        var vector = FloatArrayToByteArray(embedding.Embedding);
        
        return new NRedisStack.Search.Query("*=>[KNN " + k + " @content_vector $vector AS vector_score]")
            .Dialect(2)
            .ReturnFields("content", "metadata", "vector_score")
            .SetSortBy("vector_score", false)
            .SetWithScores()
            .AddParam("vector", vector);
    }
}