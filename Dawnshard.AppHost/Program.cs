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

if (builder.Configuration.GetValue<bool>("EnableGrafana"))
{
    builder
        .AddContainer("grafana", "grafana/grafana")
        .WithBindMount("./grafana/config", "/etc/grafana", isReadOnly: true)
        .WithBindMount("./grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
        .WithHttpEndpoint(targetPort: 3000, name: "http");

    builder
        .AddContainer("prometheus", "prom/prometheus")
        .WithBindMount("./prometheus", "/etc/prometheus", isReadOnly: true)
        .WithHttpEndpoint( /* This port is fixed as it's referenced from the Grafana config */
            port: 9090,
            targetPort: 9090
        );
}

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
