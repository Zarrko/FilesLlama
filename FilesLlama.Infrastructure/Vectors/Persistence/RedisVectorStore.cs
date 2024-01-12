using ErrorOr;
using FilesLlama.Application.Common.Interfaces;
using FilesLlama.Domain.Vectors;

namespace FilesLlama.Infrastructure.Vectors.Persistence;

public class RedisVectorStore : IVectorStore
{
    public Task<ErrorOr<bool>> AddDocuments(string index, List<string> documents, List<KeyValuePair<string, string>> documentsMetadata)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorOr<VectorIndex>> SearchSimilarDocuments(string index, string userQuery, int k)
    {
        throw new NotImplementedException();
    }
}