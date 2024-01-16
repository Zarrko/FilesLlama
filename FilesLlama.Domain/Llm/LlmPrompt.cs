namespace FilesLlama.Domain.Llm;

public class LlmPrompt
{
    public int[] Tokens { get; init; } = Array.Empty<int>();
    
    public int PredictionTokens { get; } = 512;
}