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

        var promptTemplate = $"Context information is below.\n" +
                             $"---------------------\n{content}\n---------------------\n" +
                             $"Given the context information and not prior knowledge, answer the query." +
                             $"\nQuery: {userQuestion}" +
                             $"\nAnswer: \\";
        return new GetTokensRequest()
        {
            Content = promptTemplate
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