using FilesLlama.Contracts.VectorStore;

namespace FilesLlama.Ingestion.Repository;

public interface IVectorStore
{
    Task AddDocuments(List<string> docs, List<Dictionary<string, string>> meta);

    Task<List<VectorStoreResponse>> SimilaritySearch(string text, int k);
}