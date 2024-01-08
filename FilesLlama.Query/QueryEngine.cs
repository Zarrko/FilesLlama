using FilesLlama.Contracts.Mapper;
using FilesLlama.Ingestion.Repository;
using FilesLlama.Query.Completion;
using FilesLlama.Query.Tokenize;

namespace FilesLlama.Query;

// ToDo: Query shouldn't have a dependency on ingestion. Refactor to make them separate concerns.
public class QueryEngine : IQueryEngine
{
    private readonly IVectorStore _vectorStore;
    private readonly ITokenizeService _tokenizeService;
    private readonly ICompletionService _completionService;

    public QueryEngine(IVectorStore vectorStore, ITokenizeService tokenizeService, ICompletionService completionService)
    {
        _vectorStore = vectorStore;
        _tokenizeService = tokenizeService;
        _completionService = completionService;
    }

    public async Task<string> Query(string userQuery, int k = 2)
    {
        var similarDocuments = await _vectorStore.SimilaritySearch(userQuery, k);

        var tokensRequest = similarDocuments.MapVectorStoreResponseToGetTokensRequest(userQuery);
        var tokens = await _tokenizeService.Tokenize(tokensRequest);

        var completionRequest = tokens.MapTokensResponseToGetCompletionRequest();
        var completionResponse = await _completionService.Complete(completionRequest);

        return completionResponse.Content;
    }
}