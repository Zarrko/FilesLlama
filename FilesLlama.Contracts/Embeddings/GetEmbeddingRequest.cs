using System.Text.Json.Serialization;

namespace FilesLlama.Contracts.Embeddings;

public class GetEmbeddingRequest
{
    [JsonPropertyName("content")]
    public string Content { get; init; }
}