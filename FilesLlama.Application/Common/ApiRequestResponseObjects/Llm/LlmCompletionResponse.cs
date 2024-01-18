using System.Text.Json.Serialization;

namespace FilesLlama.Application.Common.ApiRequestResponseObjects.Llm;

public class LlmCompletionResponse
{
    [JsonPropertyName("content")]
    public required string Content { get; init; }
}