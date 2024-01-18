using ErrorOr;
using FilesLlama.Application.Common.ApiRequestResponseObjects.Llm;

namespace FilesLlama.Application.Common.Interfaces;

public interface ICompletionEngine
{
    Task<ErrorOr<string>> CompleteQuery(LlmCompletionRequest llmCompletionRequest);
}