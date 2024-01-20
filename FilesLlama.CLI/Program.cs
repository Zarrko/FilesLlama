using FilesLlama.Application;
using FilesLlama.Application.Vectors.Commands.CreateVectorIndex;
using FilesLlama.Application.Vectors.Queries.ReadVectorIndex;
using FilesLlama.CLI;
using FilesLlama.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json");
var config = configuration.Build();

var indexName = config.GetSection("IndexName").Value;
ArgumentException.ThrowIfNullOrEmpty(indexName);

services
    .AddApplication()
    .AddInfrastructure();

var provider = services.BuildServiceProvider();
var cancellationToken = CancellationToken.None;

var mediatr = provider.GetRequiredService<ISender>();

// Ingestion: 
var files = await FilesHelper.ReadAllBytesAsync(path: "../../../../Documents");
var ingestCommand = new CreateVectorIndexCommand(Index: indexName, Documents: files.ToList(),
    DocumentsMetadata: new List<KeyValuePair<string, string>>(0));
await mediatr.Send(ingestCommand);

// Query:
var queryCommand = new ReadVectorIndexCommand(Index: indexName, UserQuery: "What is Nairobi?", K: 1);
var queryResults = await mediatr.Send(queryCommand);
if (queryResults.IsError)
{
    Console.WriteLine(queryResults.FirstError.Description);
}
else
{
    Console.WriteLine(queryResults);
}
