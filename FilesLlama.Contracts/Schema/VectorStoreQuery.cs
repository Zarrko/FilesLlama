namespace FilesLlama.Contracts.Schema;

public enum VectorStoreQueryMode
{
    DEFAULT,
    SPARSE,
    HYBRID,
    TEXT_SEARCH,
    SVM,
    LOGISTIC_REGRESSION,
    LINEAR_REGRESSION,
    MMR
}

public class VectorStoreQuery
{
    public List<float>? QueryEmbedding { get; init; }
    
    public int? SimilarityTopK { get; init; }
    
    public List<string>? NodeIds { get; init; }
    
    public string? QueryString { get; init; }
    
    public List<string>? OutputFields { get; init; }
    
    public string? EmbeddingField { get; init; }

    public VectorStoreQueryMode Mode { get; init; } = VectorStoreQueryMode.DEFAULT;
    
    public float? Alpha { get; init; }
    
    // MetadataFilters
}

public class VectorStoreQueryResults
{
    public SortedSet<BaseNode>? Nodes { get; init; }
    
    public List<float>? Similarities { get; init; }
    
    public List<string>? Ids { get; init; }
}