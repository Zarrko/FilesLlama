using FilesLlama.Contracts.VectorStore;

namespace FilesLlama.Query.Repository;

public interface IQueryVectorStore
{
    Task<List<VectorStoreResponse>> SimilaritySearch(string text, int k);
}