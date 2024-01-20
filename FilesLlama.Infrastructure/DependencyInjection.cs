using FilesLlama.Application.Common.Interfaces;
using FilesLlama.Infrastructure.Embeddings;
using FilesLlama.Infrastructure.Llm;
using FilesLlama.Infrastructure.Tokens;
using FilesLlama.Infrastructure.Vectors.Persistence;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FilesLlama.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient(HttpClientsConstants.LlamaCpp,client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080");
        });
        
        serviceCollection.AddSingleton<IConnectionMultiplexer>(_=> ConnectionMultiplexer.Connect("localhost:6379"));
        serviceCollection.AddSingleton<IEmbeddingsGenerator, LlamaCppEmbeddingsGeneratorClient>();
        serviceCollection.AddSingleton<ICompletionEngine, LlamaCppCompletionEngine>();
        serviceCollection.AddSingleton<ITokensGenerator, TokensGenerator>();
        serviceCollection.AddSingleton<IVectorStore>(s => new RedisVectorStore(
            s.GetRequiredService<IEmbeddingsGenerator>(),
            s.GetRequiredService<IConnectionMultiplexer>()
        ));

        return serviceCollection;
    }
}