using ErrorOr;

namespace FilesLlama.Application.Common.Interfaces;

public interface ITokensGenerator
{
    Task<ErrorOr<int[]>> GenerateTokens(string content);
}