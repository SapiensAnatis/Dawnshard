using System.Diagnostics.CodeAnalysis;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace DragaliaAPI.Photon.StateManager.Test;

public class TestContainersHelper
{
    private const int RedisContainerPort = 6379;

    private readonly IContainer? redisContainer;

    public string GetRedisConnectionString()
    {
        if (IsGithubActions)
        {
            return $"localhost:{RedisContainerPort}";
        }

        this.ThrowIfRedisContainerNull();

        return $"{this.redisContainer.Hostname}:{this.redisContainer.GetMappedPublicPort(RedisContainerPort)}";
    }

    public TestContainersHelper()
    {
        if (!IsGithubActions)
        {
            redisContainer = new ContainerBuilder()
                .WithImage("redis/redis-stack")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("redis-cli PING"))
                .WithPortBinding(RedisContainerPort, true)
                .WithPortBinding(8001, true)
                .Build();
        }
    }

    public async Task StartAsync()
    {
        if (IsGithubActions)
        {
            return;
        }

        this.ThrowIfRedisContainerNull();

        await this.redisContainer.StartAsync();
    }

    public async Task StopAsync()
    {
        if (IsGithubActions)
        {
            return;
        }

        this.ThrowIfRedisContainerNull();

        await this.redisContainer.StopAsync();
    }

    [MemberNotNull(nameof(this.redisContainer))]
    private void ThrowIfRedisContainerNull()
    {
        if (this.redisContainer is null)
        {
            throw new InvalidOperationException("Redis container not initialized!");
        }
    }

    private static bool IsGithubActions =>
        Environment.GetEnvironmentVariable("GITHUB_ACTIONS") is not null;
}
