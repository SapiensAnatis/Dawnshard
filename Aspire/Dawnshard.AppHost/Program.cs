using System.Net.Sockets;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIREPROXYENDPOINTS001
IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres")
    .WithImage("postgres", "17.5")
    .WithDataVolume("dragalia-api-pgdata")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(false);
#pragma warning restore ASPIREPROXYENDPOINTS001

IResourceBuilder<RedisResource> redis = builder
    .AddRedis("redis")
    .WithImage("redis/redis-stack", "7.4.0-v3")
    .WithPassword(null)
    .WithEntrypoint("/entrypoint.sh") // Default Aspire entrypoint doesn't load modules correctly
    .WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<ProjectResource> dragaliaApi = builder
    .AddProject<Projects.DragaliaAPI>("dragalia-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithEndpoint("http", http => http.TargetHost = "0.0.0.0")
    .WithExternalHttpEndpoints();

if (builder.Configuration.GetValue<bool>("EnablePhoton"))
{
    string bearerToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    IResourceBuilder<ProjectResource> stateManager = builder
        .AddProject<Projects.DragaliaAPI_Photon_StateManager>("photon-state-manager")
        .WithReference(redis)
        .WithEndpoint("http", http => http.TargetHost = "0.0.0.0")
        .WithExternalHttpEndpoints()
        .WithEnvironment("PhotonOptions__Token", bearerToken);

    // This can't be configured with Aspire, but you also need to edit appsettings.Development.json to set
    // PhotonOptions:Url to the address your client should use to connect to the Photon server. For a typical setup
    // where you connect via your PC's local IP address, this should be 192.168.x.x:5055 (no http://)
    dragaliaApi = dragaliaApi
        .WithEnvironment("PhotonOptions__StateManagerUrl", stateManager.GetEndpoint("http"))
        .WithEnvironment("PhotonOptions__Token", bearerToken);

    // Corresponds to PhotonServer/ directory in repo root
    string photonServerDirectory = Path.Combine(
        AppContext.BaseDirectory,
        "..",
        "..",
        "..",
        "..",
        "..",
        "PhotonServer",
        "deploy",
        "bin_Win64"
    );

    IResourceBuilder<ExecutableResource> photonServer = builder
        .AddExecutable(
            "photon-server",
            Path.Combine(photonServerDirectory, "PhotonSocketServer.exe"),
            photonServerDirectory,
            "/run",
            "LoadBalancing",
            "/configPath",
            photonServerDirectory
        )
        .WithEnvironment("ApiServerUrl", dragaliaApi.GetEndpoint("http"))
        .WithEnvironment("StateManagerUrl", stateManager.GetEndpoint("http"))
        .WithEnvironment("BearerToken", bearerToken)
        .WithEndpoint(
            "gs-to-ms", // https://doc.photonengine.com/server/current/operations/tcp-and-udp-port-numbers
            e =>
            {
                e.Port = 4520;
                e.IsProxied = false;
                e.IsExternal = true;
                e.Protocol = ProtocolType.Tcp;
            }
        )
        .WithEndpoint(
            "client-to-ms",
            e =>
            {
                e.Port = 5055;
                e.IsProxied = false;
                e.IsExternal = true;
                e.Protocol = ProtocolType.Udp;
            }
        )
        .WithEndpoint(
            "client-to-gs",
            e =>
            {
                e.Port = 5056;
                e.IsProxied = false;
                e.IsExternal = true;
                e.Protocol = ProtocolType.Udp;
            }
        );
}

if (builder.Configuration.GetValue<bool>("EnableWebsite"))
{
    builder
        .AddNpmApp("website", workingDirectory: "../../Website", scriptName: "dev")
        .WithEnvironment("PUBLIC_ENABLE_MSW", "false")
        .WithEnvironment("DAWNSHARD_API_URL_SSR", dragaliaApi.GetEndpoint("http"))
        .WithHttpEndpoint(null, 3001, "http");
}

builder.Build().Run();
