using System;
using System.Text;

namespace FilesLlama.CLI;

public static class FilesHelper
{
    public static async Task<IEnumerable<string>> ReadAllBytesAsync(string path, CancellationToken token = default)
    {
        if (!Directory.Exists(path))
        {
            return Enumerable.Empty<string>();
        }

        var files = Directory.GetFiles(path, searchPattern: "*.txt");
        var results = new List<string>(files.Length);
        foreach (var file in files)
        {
            try
            {
                string content;
                await using (var source = File.Open(file, FileMode.Open))
                using (var reader = new StreamReader(source, Encoding.UTF8))
                {
                    content = await reader.ReadToEndAsync(token);
                }

                results.Add(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {file}: {ex.Message}");
            }
        }

        return results;
    }
}