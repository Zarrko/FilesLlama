using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ErrorOr;
using FilesLlama.Application.Common.ApiRequestResponseObjects.Llm;
using FilesLlama.Application.Common.Interfaces;

namespace FilesLlama.Infrastructure.Tokens;

public class TokensGenerator : ITokensGenerator
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TokensGenerator(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorOr<int[]>> GenerateTokens(string content)
    {
        using var client = _httpClientFactory.CreateClient(HttpClientsConstants.LlamaCpp);
        var jsonRequest = JsonSerializer.Serialize(content);
        
        using var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.RequestUri = new Uri(HttpClientsConstants.LlamaCppTokens, UriKind.Relative);
        httpRequestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        try
        {
            using var response = await client.SendAsync(httpRequestMessage);
            if (!response.IsSuccessStatusCode)
            {
                return Error.Failure(description: $"Unable to fetch tokens: {response.StatusCode.ToString()}");
            }
            var tokensResponse = await response.Content.ReadFromJsonAsync<int[]>();
            if (tokensResponse == null || tokensResponse.Length < 1)
            { 
                return Error.Unexpected(description: "No tokens generated or unable to read generated tokens.");
            }
            return tokensResponse;
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }
}