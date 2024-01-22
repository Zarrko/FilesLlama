using System.Text.Json.Serialization;

namespace FilesLlama.Application.Common.ApiRequestResponseObjects.Tokens;

public class TokensRequest
{
    [JsonPropertyName("content")]
    public string? Content { get; init; }
}