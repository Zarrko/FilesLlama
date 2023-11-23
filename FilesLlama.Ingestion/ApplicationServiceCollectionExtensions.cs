using FilesLlama.Ingestion.Embeddings;
using FilesLlama.Ingestion.Repository;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FilesLlama.Ingestion;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient(ClientsConstants.LlamaEmbeddings,client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/embedding");
        });
        serviceCollection.AddSingleton<IConnectionMultiplexer>(_=> ConnectionMultiplexer.Connect("localhost"));
        serviceCollection.AddSingleton<IEmbeddingsService, LlamaCppEmbeddingsService>();
        return serviceCollection;
    }
}