using System.Diagnostics.CodeAnalysis;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Npgsql;
using Testcontainers.PostgreSql;

namespace DragaliaAPI.Integration.Test;

public class TestContainersHelper
{
    private const int PostgresContainerPort = 5432;
    private const int RedisContainerPort = 6379;

    private PostgreSqlContainer? postgresContainer;
    private IContainer? redisContainer;

    private string postgresUser;
    private string postgresPassword;
    private string postgresDatabase;
    private string postgresHost;
    private int postgresPort;

    public string RedisHost { get; private set; }
    public int RedisPort { get; private set; }

    public string PostgresConnectionString
    {
        get
        {
            NpgsqlConnectionStringBuilder builder =
                new()
                {
                    Host = this.postgresHost,
                    Port = this.postgresPort,
                    Username = this.postgresUser,
                    Password = this.postgresPassword,
                    Database = this.postgresDatabase,
                    IncludeErrorDetail = true,
                };

            return builder.ConnectionString;
        }
    }

    private static bool IsGithubActions =>
        Environment.GetEnvironmentVariable("GITHUB_ACTIONS") is not null;

    [MemberNotNullWhen(true, nameof(postgresContainer), nameof(redisContainer))]
    private bool ContainersAvailable
    {
        get
        {
            if (IsGithubActions)
                return false;

            ArgumentNullException.ThrowIfNull(this.postgresContainer);
            ArgumentNullException.ThrowIfNull(this.redisContainer);

            return true;
        }
    }

    public TestContainersHelper()
    {
        if (!IsGithubActions)
        {
            this.postgresUser = "testing";
            this.postgresPassword = "aVerystrong(!)password123";
            this.postgresDatabase = "testing";

            this.postgresContainer = new PostgreSqlBuilder()
                .WithImage("postgres:16")
                .WithUsername(this.postgresUser)
                .WithPassword(this.postgresPassword)
                .WithDatabase(this.postgresDatabase)
                .WithPortBinding(PostgresContainerPort, true)
                .Build();

            this.redisContainer = new ContainerBuilder()
                .WithImage("redis/redis-stack:7.2.0-v6")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("redis-cli PING"))
                .WithPortBinding(RedisContainerPort, true)
                .Build();

            this.postgresHost = this.postgresContainer.Hostname;

            this.RedisHost = this.redisContainer.Hostname;
        }
        else
        {
            postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER")!;
            postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")!;
            postgresDatabase = postgresUser;
            postgresHost = "localhost";
            postgresPort = PostgresContainerPort;

            this.RedisHost = "localhost";
            this.RedisPort = RedisContainerPort;
        }
    }

    public async Task StartAsync()
    {
        if (!this.ContainersAvailable)
            return;

        await postgresContainer.StartAsync();
        await redisContainer.StartAsync();

        this.postgresPort = this.postgresPort = this.postgresContainer.GetMappedPublicPort(
            PostgresContainerPort
        );
        this.RedisPort = this.redisContainer.GetMappedPublicPort(RedisContainerPort);
    }

    public async Task StopAsync()
    {
        if (!this.ContainersAvailable)
            return;

        await this.postgresContainer.StopAsync();
        await this.redisContainer.StopAsync();
    }
}
