using System.Text;
using System.Text.Json;
using FilesLlama.Contracts.Embeddings;
using FilesLlama.Contracts.VectorStore;
using FilesLlama.Query.Embeddings;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace FilesLlama.Query.Repository;

public class RedisQueryVectorStore : IQueryVectorStore
{
    private readonly IQueryEmbeddingsService _queryEmbeddingsService;
    private readonly ISearchCommands _ft;
    private readonly string _index;

    public RedisQueryVectorStore(IConnectionMultiplexer connectionMultiplexer, IQueryEmbeddingsService queryEmbeddingsService, string index)
    {
        _queryEmbeddingsService = queryEmbeddingsService;
        _index = index;
        var db = connectionMultiplexer.GetDatabase();
        _ft = db.FT();
    }

    public async Task<List<VectorStoreResponse>> SimilaritySearch(string text, int k)
    {
        var q = await PrepareQuery(text, k);

        var searchResult = _ft.Search(_index, q);
        var docs = searchResult.Documents;
        if (docs.Count < 1)
        {
            return new List<VectorStoreResponse>(0);
        }

        var retrievedDocs = new List<VectorStoreResponse>();
        foreach (var doc in docs)
        {
            var retrievedDoc = new VectorStoreResponse()
            {
                Content = doc["content"].ToString(),
                Meta = JsonSerializer.Deserialize<Dictionary<string, string>>(doc["metadata"]),
                VectorScore = (double)doc["vector_score"]
            };
            
            retrievedDocs.Add(retrievedDoc);
        }

        return retrievedDocs;
    }

    private async Task<NRedisStack.Search.Query> PrepareQuery(string text, int k)
    {
        var embedding = await _queryEmbeddingsService.EmbedQuery(new GetEmbeddingRequest() { Content = text });
        var vector = embedding.Embedding.Select(d => (float)d).ToArray();
        
        return new NRedisStack.Search.Query($"*=>[KNN {k} @content_vector $query_vector AS vector_score]")
            .AddParam("query_vector", vector.SelectMany(BitConverter.GetBytes).ToArray())
            .Dialect(2)
            .ReturnFields("content", "metadata", "vector_score")
            .SetSortBy("vector_score")
            .SetWithScores();
    }
}