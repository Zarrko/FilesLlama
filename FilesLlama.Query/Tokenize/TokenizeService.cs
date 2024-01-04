using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FilesLlama.Contracts.Tokens;

namespace FilesLlama.Query.Tokenize;

public class TokenizeService : ITokenizeService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TokenizeService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GetTokensResponse> Tokenize(GetTokensRequest tokens)
    {
        using var client = _httpClientFactory.CreateClient(QueryClientsConstants.Tokenize);
        var jsonRequest = JsonSerializer.Serialize(tokens);
        
        using var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var defaultTokensResponse = new GetTokensResponse();
        try
        {
            using var response = await client.SendAsync(httpRequestMessage);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = (await response.Content.ReadFromJsonAsync<GetTokensResponse>()) ??
                                    defaultTokensResponse;
                return tokenResponse;
            }

            return defaultTokensResponse;
        }
        catch (Exception ex)
        {
            // Some exception handling here :)
            throw;
        }
    }
}