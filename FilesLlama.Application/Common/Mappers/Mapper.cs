using FilesLlama.Application.Common.ApiRequestResponseObjects.Llm;
using FilesLlama.Domain.Llm;

namespace FilesLlama.Application.Common.Mappers;

public static class Mapper
{
    public static LlmCompletionRequest MapLlmPromptToLlmCompletionRequest(this LlmPrompt llmPrompt)
    {
        return new LlmCompletionRequest
        {
            Tokens = llmPrompt.Tokens
        };
    }
}