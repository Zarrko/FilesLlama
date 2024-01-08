namespace FilesLlama.Ingestion;

public interface IIngestService
{
    Task Ingest(string filesPath, CancellationToken cancellationToken);
}