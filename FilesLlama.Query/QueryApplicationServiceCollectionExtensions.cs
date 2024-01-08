using FilesLlama.Query.Completion;
using FilesLlama.Query.Tokenize;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FilesLlama.Query;

public static class QueryApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddQueryApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient(QueryClientsConstants.Tokenize,client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/tokenize");
        });
        serviceCollection.AddHttpClient(QueryClientsConstants.Completion,client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/completion");
        });
        serviceCollection.AddSingleton<IConnectionMultiplexer>(_=> ConnectionMultiplexer.Connect("localhost:6379"));
        serviceCollection.AddSingleton<ITokenizeService, TokenizeService>();
        serviceCollection.AddSingleton<ICompletionService, CompletionService>();

        return serviceCollection;
    }
}