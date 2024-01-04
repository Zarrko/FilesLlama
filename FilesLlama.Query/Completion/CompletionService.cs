using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FilesLlama.Contracts.Completion;

namespace FilesLlama.Query.Completion;

public class CompletionService : ICompletionService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CompletionService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GetCompletionResponse> Complete(GetCompletionRequest completionRequest)
    {
        using var client = _httpClientFactory.CreateClient(QueryClientsConstants.Completion);
        var jsonRequest = JsonSerializer.Serialize(completionRequest);
        
        using var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var defaultCompletionResponse = new GetCompletionResponse();
        try
        {
            using var response = await client.SendAsync(httpRequestMessage);

            if (response.IsSuccessStatusCode)
            {
                var completionResponse = (await response.Content.ReadFromJsonAsync<GetCompletionResponse>()) ??
                                         defaultCompletionResponse;
                return completionResponse;
            }

            return defaultCompletionResponse;
        }
        catch (Exception ex)
        {
            // Some exception handling here :)
            throw;
        }
    }
}