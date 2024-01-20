using ErrorOr;
using FilesLlama.Application.Common.ApiRequestResponseObjects.Llm;
using FilesLlama.Application.Common.Interfaces;
using FilesLlama.Domain.Vectors;
using MediatR;

namespace FilesLlama.Application.Vectors.Queries.ReadVectorIndex;

public class ReadVectorIndexCommandHandler : IRequestHandler<ReadVectorIndexCommand, ErrorOr<string>>
{
    private readonly IVectorStore _vectorStore;
    private readonly ITokensGenerator _tokensGenerator;
    private readonly ICompletionEngine _completionEngine;

    public ReadVectorIndexCommandHandler(IVectorStore vectorStore, ICompletionEngine completionEngine, ITokensGenerator tokensGenerator)
    {
        _vectorStore = vectorStore;
        _completionEngine = completionEngine;
        _tokensGenerator = tokensGenerator;
    }

    public async Task<ErrorOr<string>> Handle(ReadVectorIndexCommand request, CancellationToken cancellationToken)
    {
        var errorOrSimilarDocuments = await _vectorStore.SearchSimilarDocuments(request.Index, request.UserQuery, request.K);
        if (errorOrSimilarDocuments.IsError)
        {
            return Error.Failure(description: errorOrSimilarDocuments.FirstError.Description);
        }

        var errorOrTokens =
            await _tokensGenerator.GenerateTokens(
                CreatePromptTemplate(errorOrSimilarDocuments.Value, request.UserQuery));
        if (errorOrTokens.IsError)
        {
            return Error.Failure(description: errorOrTokens.FirstError.Description);
        }

        var errorOrPromptResponse = await _completionEngine.CompleteQuery(new LlmCompletionRequest { Tokens = errorOrTokens.Value });
        if (errorOrPromptResponse.IsError)
        {
            return Error.Failure(description: errorOrPromptResponse.FirstError.Description);
        }

        return errorOrPromptResponse.Value;
    }

    private string CreatePromptTemplate(List<VectorIndex> similarDocuments, string query)
    {
        var context = string.Empty;
        foreach (var similarDocument in similarDocuments)
        {
            context += similarDocument.Content;
        }
        
        var promptTemplate = $"Context information is below.\n" +
                             $"---------------------\n{context}\n---------------------\n" +
                             $"Given the context information and not prior knowledge, answer the query." +
                             $"\nQuery: {query}" +
                             $"\nAnswer: \\";

        return promptTemplate;
    }
}