namespace FilesLlama.Query;

public interface IQueryEngine
{
// ToDo: Refactor so as to use common interfaces between Query and Ingest
    Task<string> Query(string userQuery, int k = 2);
}