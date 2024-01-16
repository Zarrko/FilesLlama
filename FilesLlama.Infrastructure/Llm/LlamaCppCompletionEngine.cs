using ErrorOr;
using FilesLlama.Application.Common.ApiRequestObjects;
using FilesLlama.Application.Common.Interfaces;

namespace FilesLlama.Infrastructure.Llm;

public class LlamaCppCompletionEngine : ICompletionEngine
{
    public Task<ErrorOr<string>> CompleteQuery(LlmCompletionRequest llmCompletionRequest)
    {
        throw new NotImplementedException();
    }
}