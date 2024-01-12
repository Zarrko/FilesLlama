using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ErrorOr;
using FilesLlama.Application.Common.Interfaces;

namespace FilesLlama.Infrastructure.Embeddings;

public class LlamaCppEmbeddingsGeneratorClient : IEmbeddingsGenerator
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LlamaCppEmbeddingsGeneratorClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<ErrorOr<double[]>>> EmbedContentObjects(List<string> contentObjects)
    {
        var contentObjectsTasks = new List<Task<ErrorOr<double[]>>>(contentObjects.Count);
        foreach (var content in contentObjects)
        {
            contentObjectsTasks.Add(GetEmbeddings(content));
        }
        
        return await TaskExtensions.WhenAll(contentObjectsTasks);
    }
    
    private async Task<ErrorOr<double[]>> GetEmbeddings(string content)
    {
        using var client = _httpClientFactory.CreateClient(HttpClientsConstants.LlamaCpp);
        var jsonRequest = JsonSerializer.Serialize(content);

        using var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.RequestUri = new Uri(HttpClientsConstants.LlamaCppEmbeddings, UriKind.Relative);
        httpRequestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        try
        {
            using var response = await client.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
            {
                return Error.Failure(description: $"Unable to fetch embeddings {response.StatusCode.ToString()}");
            }
            var embeddingResponse = await response.Content.ReadFromJsonAsync<double[]>();
            if (embeddingResponse == null)
            { 
                return Error.Unexpected(description: "No embeddings generated or unable to read generated embeddings.");
            }
            return embeddingResponse;
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }
}