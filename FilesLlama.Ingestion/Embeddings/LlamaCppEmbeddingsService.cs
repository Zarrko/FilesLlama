using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FilesLlama.Contracts.Embeddings;

namespace FilesLlama.Ingestion.Embeddings;

public class LlamaCppEmbeddingsService : IEmbeddingsService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LlamaCppEmbeddingsService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<GetEmbeddingsResponse>> EmbedDocuments(List<GetEmbeddingRequest> documents)
    {
        // API doesn't accept collection of documents (or does it?) 
        var documentTasks = new List<Task<GetEmbeddingsResponse>>(documents.Count);

        foreach (var document in documents)
        {
            documentTasks.Add(GetEmbeddings(document));
        }

        var response = await TaskExtensions.WhenAll(documentTasks);

        return response.ToList();
    }

    public async Task<GetEmbeddingsResponse> EmbedQuery(GetEmbeddingRequest query)
    {
        return await GetEmbeddings(query);
    }

    private async Task<GetEmbeddingsResponse> GetEmbeddings(GetEmbeddingRequest document)
    {
        var client = _httpClientFactory.CreateClient(ClientsConstants.LlamaEmbeddings);
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
                return (await response.Content.ReadFromJsonAsync<GetEmbeddingsResponse>() ?? default(GetEmbeddingsResponse)) ?? defaultGetEmbeddingResponse;
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