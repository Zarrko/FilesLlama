using FilesLlama.Ingestion;
using FilesLlama.Ingestion.Embeddings;
using FilesLlama.Ingestion.Repository;
using FilesLlama.Query;
using FilesLlama.Query.Embeddings;
using FilesLlama.Query.Repository;
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
services.AddQueryApplication();

// ToDo: Move this to AddIngestApplication()
services.AddSingleton<IVectorIngestStore>(s => new RedisVectorIngestStore(
    s.GetRequiredService<IConnectionMultiplexer>(),
    s.GetRequiredService<IEmbeddingsService>(),
    indexName
));

// ToDo: Move this to AddQueryApplication() 
services.AddSingleton<IQueryVectorStore>(s => new RedisQueryVectorStore(
    s.GetRequiredService<IConnectionMultiplexer>(),
    s.GetRequiredService<IQueryEmbeddingsService>(),
    indexName
));

var provider = services.BuildServiceProvider();

var cancellationToken = CancellationToken.None;

// Ingestion: 
var ingestService = provider.GetRequiredService<IIngestService>();
await ingestService.Ingest(filesPath: "../../../../Documents", cancellationToken);

// Query:
var queryEngine = provider.GetRequiredService<IQueryEngine>();
var queryResults = await queryEngine.Query(userQuery: "What is Nairobi?", k: 1);
Console.WriteLine(queryResults);

// ToDo: Add an API here to read from google drive or wikipedia :) And use the additional api details as metadata
// ToDo: GET: https://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&titles=Rabat&explaintext=true