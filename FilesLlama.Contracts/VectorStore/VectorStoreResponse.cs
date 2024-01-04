namespace FilesLlama.Contracts.VectorStore;

public class VectorStoreResponse
{
    public Dictionary<string, string> Meta { get; init; }
    
    public string Content { get; init; }
    
    public double VectorScore { get; init; }
}