using FilesLlama.Contracts.Completion;
using FilesLlama.Contracts.Tokens;
using FilesLlama.Contracts.VectorStore;

namespace FilesLlama.Contracts.Mapper;

public static class Mappers
{
    public static GetTokensRequest MapVectorStoreResponseToGetTokensRequest(this List<VectorStoreResponse> vectorStoreResponses, string userQuestion)
    {
        var content = string.Empty;
        foreach (var vectorStoreResponse in vectorStoreResponses)
        {
            content += $"{vectorStoreResponse.Content} ";
        }

        content += userQuestion;
        return new GetTokensRequest()
        {
            Content = content
        };
    }

    public static GetCompletionRequest MapTokensResponseToGetCompletionRequest(this GetTokensResponse tokensResponse)
    {
        return new GetCompletionRequest
        {
            Tokens = tokensResponse.Tokens
        };
    }
}