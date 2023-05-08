using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Serilog;

namespace DragaliaAPI.Integration.Test;

static class TestContainers
{
    private const int PostgresContainerPort = 5432;
    private const int RedisContainerPort = 6379;

    public static string PostgresUser { get; private set; }
    public static string PostgresPassword { get; private set; }
    public static string PostgresHost { get; private set; }
    public static int PostgresPort { get; private set; }

    public static string RedisHost { get; private set; }
    public static int RedisPort { get; private set; }

    static TestContainers()
    {
        Log.Logger.Information(
            "env GITHUB_ACTIONS: {env}",
            Environment.GetEnvironmentVariable("GITHUB_ACTIONS")
        );

        if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") is null)
        {
            PostgresUser = "testing";
            PostgresPassword = "aVerystrong(!)password123";

            CreateContainers(out IContainer postgresContainer, out IContainer redisContainer);

            PostgresHost = postgresContainer.Hostname;
            PostgresPort = postgresContainer.GetMappedPublicPort(PostgresContainerPort);

            RedisHost = redisContainer.Hostname;
            RedisPort = redisContainer.GetMappedPublicPort(RedisContainerPort);
        }
        else
        {
            PostgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER")!;
            PostgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")!;
            PostgresHost = "localhost";
            PostgresPort = PostgresContainerPort;

            RedisHost = "localhost";
            RedisPort = RedisContainerPort;
        }
    }

    private static void CreateContainers(
        out IContainer postgresContainer,
        out IContainer redisContainer
    )
    {
        postgresContainer = new ContainerBuilder()
            .WithImage("postgres")
            .WithEnvironment(
                new Dictionary<string, string>()
                {
                    { "POSTGRES_USER", PostgresUser },
                    { "POSTGRES_PASSWORD", PostgresPassword }
                }
            )
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .WithPortBinding(PostgresContainerPort, true)
            .Build();

        redisContainer = new ContainerBuilder()
            .WithImage("redis")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("redis-cli PING"))
            .WithPortBinding(RedisContainerPort, true)
            .Build();

        postgresContainer.StartAsync().Wait();
        redisContainer.StartAsync().Wait();
    }
}
