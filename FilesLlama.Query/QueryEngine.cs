using FilesLlama.Contracts.Mapper;
using FilesLlama.Query.Completion;
using FilesLlama.Query.Repository;
using FilesLlama.Query.Tokenize;

namespace FilesLlama.Query;

// ToDo: Refactor so as to use common interfaces between Query and Ingest
public class QueryEngine : IQueryEngine
{
    private readonly IQueryVectorStore _queryVectorStore;
    private readonly ITokenizeService _tokenizeService;
    private readonly ICompletionService _completionService;

    public QueryEngine(IQueryVectorStore queryVectorStore, ITokenizeService tokenizeService, ICompletionService completionService)
    {
        _queryVectorStore = queryVectorStore;
        _tokenizeService = tokenizeService;
        _completionService = completionService;
    }

    public async Task<string> Query(string userQuery, int k = 2)
    {
        var similarDocuments = await _queryVectorStore.SimilaritySearch(userQuery, k);
        if (similarDocuments.Count < 1)
        {
            // ToDo: Throw an exception maybe?
            return string.Empty;
        }
        
        var tokensRequest = similarDocuments.MapVectorStoreResponseToGetTokensRequest(userQuery);
        var tokens = await _tokenizeService.Tokenize(tokensRequest);

        var completionRequest = tokens.MapTokensResponseToGetCompletionRequest();
        var completionResponse = await _completionService.Complete(completionRequest);

        return completionResponse.Content;
    }
}