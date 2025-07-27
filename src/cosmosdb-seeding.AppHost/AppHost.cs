using cosmosdb_seeding.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Api>(ServiceNames.Api)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Web>(ServiceNames.Web)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
