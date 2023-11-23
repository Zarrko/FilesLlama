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
var indexName = config.GetConnectionString("VectorIndex") ?? "index";

services.AddApplication();

services.AddSingleton<IVectorStore>(s => new RedisVectorStore(
    s.GetRequiredService<IConnectionMultiplexer>(),
    s.GetRequiredService<IEmbeddingsService>(),
    indexName
));
