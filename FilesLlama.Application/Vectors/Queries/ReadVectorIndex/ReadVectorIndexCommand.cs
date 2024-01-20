using ErrorOr;
using FilesLlama.Domain.Vectors;
using MediatR;

namespace FilesLlama.Application.Vectors.Queries.ReadVectorIndex;

public record ReadVectorIndexCommand(string Index, string UserQuery, int K) : IRequest<ErrorOr<string>>;