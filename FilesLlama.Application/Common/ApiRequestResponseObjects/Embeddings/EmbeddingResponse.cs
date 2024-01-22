using System.Text.Json.Serialization;

namespace FilesLlama.Application.Common.ApiRequestResponseObjects.Embeddings;

public class EmbeddingResponse
{
    [JsonPropertyName("embedding")]
    public double[]? Embedding { get; init; }
}