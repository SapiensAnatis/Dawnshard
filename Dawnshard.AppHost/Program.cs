using Microsoft.Extensions.Configuration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres")
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
    builder
        .AddProject<Projects.DragaliaAPI_Photon_StateManager>("photon-state-manager")
        .WithReference(redis)
        .WithExternalHttpEndpoints();
}

if (builder.Configuration.GetValue<bool>("EnableWebsite"))
{
    builder
        .AddNpmApp("website", workingDirectory: "../Website", scriptName: "dev")
        .WithEnvironment("PUBLIC_ENABLE_MSW", "false")
        .WithEnvironment("DAWNSHARD_API_URL_SSR", dragaliaApi.GetEndpoint("http"));
}

builder.Build().Run();
