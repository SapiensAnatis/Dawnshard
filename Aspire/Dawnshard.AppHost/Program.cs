using Microsoft.Extensions.Configuration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres")
    .WithImage("postgres", "16.4")
    .WithDataVolume("dragalia-api-pgdata");

IResourceBuilder<RedisResource> redis = builder
    .AddRedis("redis")
    .WithImage("redis/redis-stack", "7.4.0-v0");

IResourceBuilder<ProjectResource> dragaliaApi = builder
    .AddProject<Projects.DragaliaAPI>("dragalia-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithExternalHttpEndpoints();

if (builder.Configuration.GetValue<bool>("EnableStateManager"))
{
    IResourceBuilder<ProjectResource> stateManager = builder
        .AddProject<Projects.DragaliaAPI_Photon_StateManager>("photon-state-manager")
        .WithReference(redis)
        .WithEndpoint("http", http => http.TargetHost = "0.0.0.0")
        .WithExternalHttpEndpoints();

    dragaliaApi.WithEnvironment("PhotonOptions__StateManagerUrl", stateManager.GetEndpoint("http"));
}

if (builder.Configuration.GetValue<bool>("EnableWebsite"))
{
    builder
        .AddNpmApp("website", workingDirectory: "../Website", scriptName: "dev")
        .WithEnvironment("PUBLIC_ENABLE_MSW", "false")
        .WithEnvironment("DAWNSHARD_API_URL_SSR", dragaliaApi.GetEndpoint("http"));
}

builder.Build().Run();
