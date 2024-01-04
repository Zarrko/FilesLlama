using FilesLlama.Query.Tokenize;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FilesLlama.Query;

public static class QueryApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddIngestApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient(QueryClientsConstants.Tokenize,client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/tokenize");
        });
        serviceCollection.AddSingleton<IConnectionMultiplexer>(_=> ConnectionMultiplexer.Connect("localhost:6379"));
        serviceCollection.AddSingleton<ITokenizeService, TokenizeService>();
        return serviceCollection;
    }
}