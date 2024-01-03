using FilesLlama.CLI;
using FilesLlama.Ingestion;
using FilesLlama.Ingestion.Embeddings;
using FilesLlama.Ingestion.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json");
var config = configuration.Build();

var indexName = config.GetSection("IndexName").Value;
ArgumentException.ThrowIfNullOrEmpty(indexName);

services.AddApplication();

services.AddSingleton<IVectorStore>(s => new RedisVectorStore(
    s.GetRequiredService<IConnectionMultiplexer>(),
    s.GetRequiredService<IEmbeddingsService>(),
    indexName
));

var provider = services.BuildServiceProvider();

var cancellationToken = CancellationToken.None;
var basePath = AppDomain.CurrentDomain.BaseDirectory;
var path = Path.Combine(basePath, "../../../../Documents");

var files = await FilesHelper.ReadAllBytesAsync(path, cancellationToken);

var vectorStore = provider.GetRequiredService<IVectorStore>();
await vectorStore.AddDocuments(files.ToList(), new List<Dictionary<string, string>>(0));

var searchResults = await vectorStore.SimilaritySearch("capital", 2);
foreach (var result in searchResults)
{
    Console.WriteLine(result.Content);
}