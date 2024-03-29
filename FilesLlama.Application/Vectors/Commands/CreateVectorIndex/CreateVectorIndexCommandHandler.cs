using ErrorOr;
using FilesLlama.Application.Common.Interfaces;
using MediatR;

namespace FilesLlama.Application.Vectors.Commands.CreateVectorIndex;

public class CreateVectorIndexCommandHandler : IRequestHandler<CreateVectorIndexCommand, ErrorOr<Created>>
{
    private readonly IVectorStore _vectorStore;

    public CreateVectorIndexCommandHandler(IVectorStore vectorStore)
    {
        _vectorStore = vectorStore;
    }

    public Task<ErrorOr<Created>> Handle(CreateVectorIndexCommand request, CancellationToken cancellationToken)
    {
        return _vectorStore.AddDocuments(request.Index, request.Documents, request.DocumentsMetadata);
    }
}