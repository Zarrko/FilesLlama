namespace FilesLlama.Contracts.Schema;


public enum NodeRelationship
{
    Source,
    Previous,
    Next,
    Parent,
    Child
}

public enum ObjectType
{
    Text,
    Image,
    Index,
    Document
}

public abstract class BaseNode
{
    private static int TruncatedLength = 350;
    private static int WrapWidth = 70;
    
    public required string Id { get; init; } = Guid.NewGuid().ToString();
    public List<float>? Embedding { get; init; }
    
    // Should be Dict<string, Any>
    public Dictionary<string, object>? Metadata { get; init; }
    public List<string>? ExcludedEmbedMetadataKeys { get; init; }
    public List<string>? ExcludedLlmMetadataKeys { get; init; }
    public Dictionary<NodeRelationship, RelatedNodeType>? Relationships { get; init; }
    public string? Hash { get; init; }
    public abstract string GetType();
    public RelatedNodeInfo? SourceNode
    {
        get
        {
            // Source object node logic
            if (Relationships != null && !Relationships.ContainsKey(NodeRelationship.Source))
            {
                return null;
            }

            var relation = Relationships?[NodeRelationship.Source];

            if (relation is List<RelatedNodeInfo>)
            {
                throw new InvalidOperationException("Source object must be a single RelatedNodeInfo object");
            }

            return (RelatedNodeInfo)relation;
        }
    }
    public RelatedNodeInfo? PrevNode
    {
        get
        {
            // Prev node logic
            if (!Relationships.ContainsKey(NodeRelationship.Previous))
            {
                return null;
            }

            var relation = Relationships[NodeRelationship.Previous];

            if (!(relation is RelatedNodeInfo))
            {
                throw new InvalidOperationException("Previous object must be a single RelatedNodeInfo object");
            }

            return (RelatedNodeInfo)relation;
        }
    }
    
    public RelatedNodeInfo? NextNode
    {
        get
        {
            // Next node logic
            if (!Relationships.ContainsKey(NodeRelationship.Next))
            {
                return null;
            }

            var relation = Relationships[NodeRelationship.Next];

            if (!(relation is RelatedNodeInfo))
            {
                throw new InvalidOperationException("Next object must be a single RelatedNodeInfo object");
            }

            return (RelatedNodeInfo)relation;
        }
    }
    
    public RelatedNodeInfo? ParentNode
    {
        get
        {
            // Parent node logic
            if (!Relationships.ContainsKey(NodeRelationship.Parent))
            {
                return null;
            }

            var relation = Relationships[NodeRelationship.Parent];

            if (!(relation is RelatedNodeInfo))
            {
                throw new InvalidOperationException("Parent object must be a single RelatedNodeInfo object");
            }

            return (RelatedNodeInfo)relation;
        }
    }
    
    public List<RelatedNodeInfo>? ChildNodes
    {
        get
        {
            // Child nodes logic
            if (!Relationships.ContainsKey(NodeRelationship.Child))
            {
                return null;
            }

            var relation = Relationships[NodeRelationship.Child];

            if (!(relation is List<RelatedNodeInfo>))
            {
                throw new InvalidOperationException("Child objects must be a list of RelatedNodeInfo objects.");
            }

            return (List<RelatedNodeInfo>)relation;
        }
    }
    
    public string ToString()
    {
        string sourceText = TruncateText(GetContent().Trim(), TruncatedLength);
        
        // ToDo: TextWrapper.Wrap($"Text: {sourceText}\n", WrapWidth);
        string sourceTextWrapped = string.Empty;

        return $"Node ID: {Id}\n{sourceTextWrapped}";
    }
    
    public RelatedNodeInfo AsRelatedNodeInfo()
    {
        return new RelatedNodeInfo
        {
            NodeId = this.Id,
            NodeType = ConvertStringToEnum(this.GetType()),
            Metadata = this.Metadata,
            Hash = this.Hash
        };
    }

    private string TruncateText(string text, int maxLength)
    {
        // Implement the logic to truncate the text
        // Example: return text.Length <= maxLength ? text : text.Substring(0, maxLength);
        throw new NotImplementedException();
    }

    private string GetContent()
    {
        // Implement the logic to get the content
        // Example: return "Content of the node";
        throw new NotImplementedException();
    }
    
    public ObjectType ConvertStringToEnum(string enumString)
    {
        if (Enum.TryParse(typeof(ObjectType), enumString, out var result))
        {
            return (ObjectType)result;
        }

        // Handle the case where the string doesn't represent a valid enum value
        throw new ArgumentException("Invalid enum string");
    }
    
}

public class RelatedNodeInfo
{
    public required string NodeId { get; init; }
    public ObjectType? NodeType { get; init; }
    public Dictionary<string, object> Metadata { get; init; } = new();
    public string? Hash { get; init; } = null;

    public static string class_name()
    {
        return "RelatedNodeInfo";
    }
}

public class RelatedNodeType
{
    public RelatedNodeInfo SingleNode { get; private set; }
    public List<RelatedNodeInfo> NodeList { get; private set; }

    // Constructor for single node
    public RelatedNodeType(RelatedNodeInfo node)
    {
        SingleNode = node;
        NodeList = null;
    }

    // Constructor for list of nodes
    public RelatedNodeType(List<RelatedNodeInfo> nodeList)
    {
        SingleNode = null;
        NodeList = nodeList;
    }
    
    // Implicit conversion from RelatedNodeType to List<RelatedNodeInfo>
    public static implicit operator List<RelatedNodeInfo>(RelatedNodeType relatedNodeType)
    {
        return relatedNodeType?.NodeList;
    }

    // Implicit conversion from RelatedNodeType to RelatedNodeInfo
    public static implicit operator RelatedNodeInfo(RelatedNodeType relatedNodeType)
    {
        return relatedNodeType?.SingleNode;
    }
}