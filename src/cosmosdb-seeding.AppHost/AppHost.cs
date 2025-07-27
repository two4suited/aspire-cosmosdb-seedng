 #pragma warning disable ASPIRECOSMOSDB001 // Enable preview emulator

using cosmosdb_seeding.Shared;

var builder = DistributedApplication.CreateBuilder(args);

// Add Azure Cosmos DB with preview emulator and Data Explorer
var cosmosDb = builder.AddAzureCosmosDB(ServiceNames.CosmosDb)
    .RunAsPreviewEmulator(emulator =>
    {
        emulator.WithDataExplorer();
        emulator.WithLifetime(ContainerLifetime.Persistent);
        emulator.WithDataVolume();
    });

// Add a database and container for seeding
var seedingDatabase = cosmosDb.AddCosmosDatabase("seeding");
var itemsContainer = seedingDatabase.AddContainer("items", "/id");

var apiService = builder.AddProject<Projects.Api>(ServiceNames.Api)
    .WithHttpHealthCheck("/health")
    .WithReference(itemsContainer);

builder.AddProject<Projects.Web>(ServiceNames.Web)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
