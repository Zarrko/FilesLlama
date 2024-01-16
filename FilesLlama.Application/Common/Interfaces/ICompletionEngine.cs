using ErrorOr;
using FilesLlama.Application.Common.ApiRequestObjects;

namespace FilesLlama.Application.Common.Interfaces;

public interface ICompletionEngine
{
    Task<ErrorOr<string>> CompleteQuery(LlmCompletionRequest llmCompletionRequest);
}