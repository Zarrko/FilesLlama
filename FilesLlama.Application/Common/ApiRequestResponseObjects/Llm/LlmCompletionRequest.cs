using System.Text.Json.Serialization;

namespace FilesLlama.Application.Common.ApiRequestResponseObjects.Llm;

public class LlmCompletionRequest
{
    [JsonPropertyName("prompt")]
    public required int[] Tokens { get; init; }

    [JsonPropertyName("n_predict")]
    public int PredictionTokens { get; } = 512;
}