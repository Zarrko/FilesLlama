using System.Text.Json.Serialization;

namespace FilesLlama.Contracts.Completion;

public class GetCompletionResponse
{
    [JsonPropertyName("content")]
    public string Content { get; init; }
}