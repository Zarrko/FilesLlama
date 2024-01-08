namespace FilesLlama.Query;

public interface IQueryEngine
{
    // ToDo: Query shouldn't have a dependency on ingestion. Refactor to make them separate concerns.
    Task<string> Query(string userQuery, int k = 2);
}