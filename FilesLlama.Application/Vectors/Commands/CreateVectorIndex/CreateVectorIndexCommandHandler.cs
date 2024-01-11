using ErrorOr;
using FilesLlama.Application.Common.Interfaces;
using MediatR;

namespace FilesLlama.Application.Vectors.Commands.CreateVectorIndex;

public class CreateVectorIndexCommandHandler : IRequestHandler<CreateVectorIndexCommand, ErrorOr<bool>>
{
    private readonly IVectorStore _vectorStore;

    public CreateVectorIndexCommandHandler(IVectorStore vectorStore)
    {
        _vectorStore = vectorStore;
    }

    public async Task<ErrorOr<bool>> Handle(CreateVectorIndexCommand request, CancellationToken cancellationToken)
    {
        return await _vectorStore.AddDocuments(request.Index, request.Documents, request.DocumentsMetadata);
    }
}