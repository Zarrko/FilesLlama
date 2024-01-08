using FilesLlama.CLI;
using FilesLlama.Ingestion.Repository;

namespace FilesLlama.Ingestion;

public class IngestService : IIngestService
{
    private readonly IVectorIngestStore _vectorIngestStore;

    public IngestService(IVectorIngestStore vectorIngestStore)
    {
        _vectorIngestStore = vectorIngestStore;
    }

    public async Task Ingest(string filesPath, CancellationToken cancellationToken)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var path = Path.Combine(basePath, filesPath);
        
        var files = await FilesHelper.ReadAllBytesAsync(path, cancellationToken);
        
        // ToDo: This constant reallocation of memory i.e., toList() is very expensive at scale.
        // ToDo: Error Handling & Metadata E.g., filename:) 
        await _vectorIngestStore.AddDocuments(files.ToList(), new List<Dictionary<string, string>>(0));
    }
}