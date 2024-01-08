using FilesLlama.Contracts.Embeddings;

namespace FilesLlama.Query.Embeddings;

public interface IQueryEmbeddingsService
{
    public Task<GetEmbeddingsResponse> EmbedQuery(GetEmbeddingRequest query);
}