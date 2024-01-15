using System.Text.Json;
using ErrorOr;
using FilesLlama.Application.Common.Interfaces;
using FilesLlama.Domain.Vectors;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

namespace FilesLlama.Infrastructure.Vectors.Persistence;

public class RedisVectorStore : IVectorStore
{
    private const string VectorPrefix = "files:";
    private IEmbeddingsGenerator _embeddingsGenerator;
    private IDatabase _db;
    private ISearchCommandsAsync _search;

    public RedisVectorStore(IEmbeddingsGenerator embeddingsGenerator, IConnectionMultiplexer connectionMultiplexer)
    {
        _embeddingsGenerator = embeddingsGenerator;
        _db = connectionMultiplexer.GetDatabase();
        _search = _db.FT();
    }

    public Task<ErrorOr<bool>> AddDocuments(string index, List<string> documents, List<KeyValuePair<string, string>> documentsMetadata)
    {
        throw new NotImplementedException();
    }

    public async Task<ErrorOr<List<VectorIndex>>> SearchSimilarDocuments(string index, string userQuery, int k)
    {
        var q = await PrepareQuery(userQuery, k);
        if (q.IsError)
        {
            return Error.Failure(description: q.Errors.FirstOrDefault().Description);
        }

        var searchResult = await _search.SearchAsync(index, q.Value);
        var docs = searchResult.Documents;
        if (docs.Count < 1)
        {
            return new List<VectorIndex>(0);
        }

        var retrievedDocs = new List<VectorIndex>();
        foreach (var doc in docs)
        {
            var retrievedDoc = new VectorIndex
            {
                Content = doc["content"].ToString(),
                Meta = JsonSerializer.Deserialize<Dictionary<string, string>>(doc["metadata"]),
                VectorScore = (double)doc["vector_score"]
            };
            
            retrievedDocs.Add(retrievedDoc);
        }

        return retrievedDocs;
    }
    
    private async Task<bool> IndexExistsInVectorStore(string index)
    {
        try
        {
            var existingIdx = await _search.InfoAsync(index);
            return !string.IsNullOrEmpty(existingIdx.IndexName);
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    private async Task<ErrorOr<Created>> CreateIndex(string index)
    {
        if (await IndexExistsInVectorStore(index))
        {
            return Result.Created;
        }

        var attributes = new Dictionary<string, object>
        {
            ["TYPE"] = "FLOAT32",
            ["DIM"] = "4096", // The size of embedding array from Llama.cpp is 4096.
            ["DISTANCE_METRIC"] = "L2",
        };
        var schema = new Schema();
        schema.AddTextField("content");
        schema.AddTextField("metadata");
        schema.AddVectorField("content_vector", Schema.VectorField.VectorAlgo.FLAT, attributes);
        
        var parameters = new FTCreateParams().On(IndexDataType.HASH).Prefix(VectorPrefix);
        var hasIndexBeenCreated = await _search.CreateAsync(index, parameters, schema);
        if (hasIndexBeenCreated)
        {
            return Result.Created;
        }

        return Error.Failure(description: "Unable to create index");
    }
    
    private async Task<ErrorOr<NRedisStack.Search.Query>> PrepareQuery(string text, int k)
    {
        var embeddingResult = await _embeddingsGenerator.EmbedContentObjects(new List<string>(1) { text });
        var errorOrEmbeddings = embeddingResult as ErrorOr<double[]>[] ?? embeddingResult.ToArray();
        if (errorOrEmbeddings.Length < 1 || errorOrEmbeddings[0].IsError)
        {
            return Error.Failure(description: errorOrEmbeddings[0].FirstError.Description);
        }
        
        var vector = errorOrEmbeddings[0].Value.Select(d => (float)d).ToArray();
        
        return new NRedisStack.Search.Query($"*=>[KNN {k} @content_vector $query_vector AS vector_score]")
            .AddParam("query_vector", vector.SelectMany(BitConverter.GetBytes).ToArray())
            .Dialect(2)
            .ReturnFields("content", "metadata", "vector_score")
            .SetSortBy("vector_score")
            .SetWithScores();
    }
}