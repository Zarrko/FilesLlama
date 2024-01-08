using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FilesLlama.Contracts.Embeddings;

namespace FilesLlama.Query.Embeddings;

public class LlamaCppQueryEmbeddingsService : IQueryEmbeddingsService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LlamaCppQueryEmbeddingsService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GetEmbeddingsResponse> EmbedQuery(GetEmbeddingRequest query)
    {
        return await GetEmbeddings(query);
    }

    private async Task<GetEmbeddingsResponse> GetEmbeddings(GetEmbeddingRequest document)
    {
        using var client = _httpClientFactory.CreateClient(QueryClientsConstants.QueryLlamaEmbeddings);
        var jsonRequest = JsonSerializer.Serialize(document);
        
        using var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        
        var defaultGetEmbeddingResponse = new GetEmbeddingsResponse();

        try
        {
            using var response = await client.SendAsync(httpRequestMessage);

            if (response.IsSuccessStatusCode)
            {
                var embeddingResponse = (await response.Content.ReadFromJsonAsync<GetEmbeddingsResponse>()) ??
                                        defaultGetEmbeddingResponse;
                return embeddingResponse;
            }

            return defaultGetEmbeddingResponse;
        }
        catch (Exception ex)
        {
            throw;
            // Some exception handling here :)
        }
    }
}