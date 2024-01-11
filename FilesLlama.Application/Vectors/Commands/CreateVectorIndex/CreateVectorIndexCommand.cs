using ErrorOr;
using MediatR;

namespace FilesLlama.Application.Vectors.Commands.CreateVectorIndex;

public record CreateVectorIndexCommand(string Index, List<string> Documents, List<KeyValuePair<string, string>> DocumentsMetadata) : IRequest<ErrorOr<bool>>;