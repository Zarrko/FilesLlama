using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FilesLlama.Ingestion;

public static class TaskExtensions
{
    public static async Task<IEnumerable<T>> WhenAll<T>(IEnumerable<Task<T>> tasks)
    {
        var allTasks = Task.WhenAll(tasks);

        try
        {
            return await allTasks;
        }
        catch (Exception)
        {
            //ignore
        }

        throw allTasks.Exception ?? throw new UnreachableException();
    }
}