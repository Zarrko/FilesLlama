using FilesLlama.Contracts.Embeddings;

namespace FilesLlama.Ingestion.Embeddings;

public interface IEmbeddingsService
{
    public Task<List<GetEmbeddingsResponse>> EmbedDocuments(List<GetEmbeddingRequest> documents);

    public Task<GetEmbeddingsResponse> EmbedQuery(GetEmbeddingRequest query);
}