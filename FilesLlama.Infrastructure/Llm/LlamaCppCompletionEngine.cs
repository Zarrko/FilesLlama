using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ErrorOr;
using FilesLlama.Application.Common.ApiRequestResponseObjects.Llm;
using FilesLlama.Application.Common.Interfaces;

namespace FilesLlama.Infrastructure.Llm;

public class LlamaCppCompletionEngine : ICompletionEngine
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LlamaCppCompletionEngine(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<ErrorOr<string>> CompleteQuery(LlmCompletionRequest llmCompletionRequest)
    {
        using var client = _httpClientFactory.CreateClient(HttpClientsConstants.LlamaCpp);
        var jsonRequest = JsonSerializer.Serialize(llmCompletionRequest);
        
        using var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.RequestUri = new Uri(HttpClientsConstants.LlamaCppCompletion, UriKind.Relative);
        httpRequestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        try
        {
            using var response = await client.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
            {
                return Error.Failure(description: $"Unable to fetch completion results: {response.StatusCode.ToString()}");
            }
            var completionResponse = await response.Content.ReadFromJsonAsync<LlmCompletionResponse>();
            if (completionResponse == null || string.IsNullOrEmpty(completionResponse.Content))
            { 
                return Error.Unexpected(description: "No completion generated or unable to read generated completion.");
            }
            return completionResponse.Content;
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }
}