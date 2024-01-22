using System.Text.Json.Serialization;

namespace FilesLlama.Application.Common.ApiRequestResponseObjects.Embeddings;

public class EmbeddingRequest
{
    [JsonPropertyName("content")]
    public string? Content { get; init; }
}