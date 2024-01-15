using ErrorOr;
using FilesLlama.Domain.Vectors;

namespace FilesLlama.Application.Common.Interfaces;

public interface IVectorStore
{
    Task<ErrorOr<bool>> AddDocuments(string index, List<string> documents, List<KeyValuePair<string, string>> documentsMetadata);

    Task<ErrorOr<List<VectorIndex>>> SearchSimilarDocuments(string index, string userQuery, int k);
}