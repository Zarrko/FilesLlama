using ErrorOr;

namespace FilesLlama.Application.Common.Interfaces;

public interface IVectorStore
{
    Task<ErrorOr<bool>> AddDocuments(string index, List<string> documents, List<KeyValuePair<string, string>> documentsMetadata);

}