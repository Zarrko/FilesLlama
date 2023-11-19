using FilesLlama.Ingestion.Embeddings;
using Microsoft.Extensions.DependencyInjection;

namespace FilesLlama.Ingestion;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient(ClientsConstants.LlamaEmbeddings,client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/embedding");
        });
        serviceCollection.AddSingleton<IEmbeddingsService, LlamaCppEmbeddingsService>();
        return serviceCollection;
    }
}