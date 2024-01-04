using System.Text.Json.Serialization;

namespace FilesLlama.Contracts.Tokens;

public class GetTokensResponse
{
    [JsonPropertyName("tokens")]
    public int[] Tokens { get; init; }
}