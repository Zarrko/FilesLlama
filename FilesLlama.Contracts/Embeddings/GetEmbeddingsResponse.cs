using System.Text.Json.Serialization;

namespace FilesLlama.Contracts.Embeddings;

public class GetEmbeddingsResponse
{
    [JsonPropertyName("embedding")]
    public float[] Embedding { get; init; }
}