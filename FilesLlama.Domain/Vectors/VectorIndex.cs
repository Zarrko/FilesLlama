namespace FilesLlama.Domain.Vectors;

public class VectorIndex
{
    public Dictionary<string, string> Meta { get; init; }
    
    public string Content { get; init; }

    public double VectorScore { get; init; }
}