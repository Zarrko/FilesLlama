using ErrorOr;
using FilesLlama.Application.Common.Interfaces;
using FilesLlama.Domain.Vectors;
using MediatR;

namespace FilesLlama.Application.Vectors.Queries.ReadVectorIndex;

public class ReadVectorIndexCommandHandler : IRequestHandler<ReadVectorIndexCommand, ErrorOr<List<VectorIndex>>>
{
    private readonly IVectorStore _vectorStore;

    public ReadVectorIndexCommandHandler(IVectorStore vectorStore)
    {
        _vectorStore = vectorStore;
    }

    public Task<ErrorOr<List<VectorIndex>>> Handle(ReadVectorIndexCommand request, CancellationToken cancellationToken)
    {
        return _vectorStore.SearchSimilarDocuments(request.Index, request.UserQuery, request.K);
    }
}