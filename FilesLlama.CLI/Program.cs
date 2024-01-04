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

services.AddIngestApplication();

services.AddSingleton<IVectorStore>(s => new RedisVectorStore(
    s.GetRequiredService<IConnectionMultiplexer>(),
    s.GetRequiredService<IEmbeddingsService>(),
    indexName
));

var provider = services.BuildServiceProvider();

var cancellationToken = CancellationToken.None;
var basePath = AppDomain.CurrentDomain.BaseDirectory;

// ToDo: Add an API here to read from google drive or wikipedia :) And use the additional api details as metadata
// ToDo: GET: https://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&titles=Rabat&explaintext=true
var path = Path.Combine(basePath, "../../../../Documents");

// ToDo: Abstract files reader + add to vector store in one IIngestService
// ToDo: Postprocessing after retrieval from vector store based on different strategies.
var files = await FilesHelper.ReadAllBytesAsync(path, cancellationToken);

var vectorStore = provider.GetRequiredService<IVectorStore>();
await vectorStore.AddDocuments(files.ToList(), new List<Dictionary<string, string>>(0));

var searchResults = await vectorStore.SimilaritySearch("capital city of Kenya", 2);

// Print most similar documents first.[SortBy Vector_Score i.e., L2/Euclidean distance, lower is better]
foreach (var result in searchResults)
{
    Console.WriteLine(result.Content);
}