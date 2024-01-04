using FilesLlama.Contracts.Tokens;

namespace FilesLlama.Query.Tokenize;

public interface ITokenizeService
{
    Task<GetTokensResponse> Tokenize(GetTokensRequest tokens);
}