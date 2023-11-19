using FilesLlama.Ingestion;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

var services = new ServiceCollection();

// Add Redis
services.AddSingleton<IConnectionMultiplexer>(_=> ConnectionMultiplexer.Connect("localhost"));
services.AddApplication();