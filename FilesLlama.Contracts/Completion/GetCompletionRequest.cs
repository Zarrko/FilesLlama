using System.Text.Json.Serialization;

namespace FilesLlama.Contracts.Completion;

public class GetCompletionRequest
{
    [JsonPropertyName("prompt")]
    public int[] Tokens { get; init; }

    [JsonPropertyName("n_predict")]
    public int PredictionTokens { get; } = 512;
}