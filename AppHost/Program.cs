IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres")
    .WithDataVolume("dragalia-api-pgdata");

IResourceBuilder<RedisResource> redis = builder
    .AddRedis("redis")
    .WithImage("redis/redis-stack", "7.4.0-v0");

IResourceBuilder<ProjectResource> dawnshard = builder
    .AddProject<Projects.DragaliaAPI>("dragalia-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithExternalHttpEndpoints();

builder
    .AddProject<Projects.DragaliaAPI_Photon_StateManager>("photon-state-manager")
    .WithReference(redis)
    .WithExternalHttpEndpoints();

builder
    .AddNpmApp("website", workingDirectory: "../Website", scriptName: "dev")
    .WithEnvironment("PUBLIC_ENABLE_MSW", "false")
    .WithEnvironment("DAWNSHARD_API_URL_SSR", dawnshard.GetEndpoint("http"));

builder.Build().Run();
