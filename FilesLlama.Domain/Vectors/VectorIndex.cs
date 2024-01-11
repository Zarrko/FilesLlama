namespace FilesLlama.Domain.Vectors;

public class VectorIndex
{
    public VectorIndex(Dictionary<string, string> meta, string content, double vectorScore)
    {
        Meta = meta;
        Content = content;
        VectorScore = vectorScore;
    }

    public Dictionary<string, string> Meta { get; init; }
    
    public string Content { get; init; }
    
    public double VectorScore { get; init; }
}