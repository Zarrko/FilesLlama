using FilesLlama.Contracts.Schema;

namespace FilesLlama.Ingestion.Repository;

public interface IVectorStore
{
    public object Client { get; init; }

    public List<string> Add(List<BaseNode> nodes);

    public void Delete(string refDocId);

    public VectorStoreQueryResults Query(VectorStoreQuery query);

    public void Persist(bool inBackground);
}