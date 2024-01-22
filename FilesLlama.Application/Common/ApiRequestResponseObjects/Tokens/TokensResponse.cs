using System.Text.Json.Serialization;

namespace FilesLlama.Application.Common.ApiRequestResponseObjects.Tokens;

public class TokensResponse
{
    [JsonPropertyName("tokens")]
    public int[]? Tokens { get; init; }
}