using FilesLlama.Contracts.Completion;

namespace FilesLlama.Query.Completion;

public interface ICompletionService
{
    Task<GetCompletionResponse> Complete(GetCompletionRequest completionRequest);
}