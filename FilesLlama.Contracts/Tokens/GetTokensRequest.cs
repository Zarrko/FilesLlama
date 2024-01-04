using System.Text.Json.Serialization;

namespace FilesLlama.Contracts.Tokens;

public class GetTokensRequest
{
    [JsonPropertyName("content")]
    public string Content { get; init; }
}