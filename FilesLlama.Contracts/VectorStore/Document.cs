namespace FilesLlama.Contracts.VectorStore;

public class Document
{
    public Dictionary<string, string> Meta { get; init; }
    
    public string Content { get; init; }
    
    public float[] Vector { get; init; }
}