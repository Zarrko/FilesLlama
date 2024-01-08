using FilesLlama.Ingestion.Embeddings;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FilesLlama.Ingestion;

public static class IngestApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddIngestApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient(IngestClientsConstants.LlamaEmbeddings,client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/embedding");
        });
        serviceCollection.AddSingleton<IConnectionMultiplexer>(_=> ConnectionMultiplexer.Connect("localhost:6379"));
        serviceCollection.AddSingleton<IEmbeddingsService, LlamaCppEmbeddingsService>();
        return serviceCollection;
    }
}