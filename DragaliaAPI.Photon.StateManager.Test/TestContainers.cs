using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace DragaliaAPI.Photon.StateManager.Test;

public static class TestContainers
{
    private const int RedisContainerPort = 6379;

    public static string RedisHost { get; private set; }
    public static int RedisPort { get; private set; }

    static TestContainers()
    {
        if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") is null)
        {
            CreateContainers(out IContainer redisContainer);

            RedisHost = redisContainer.Hostname;
            RedisPort = redisContainer.GetMappedPublicPort(RedisContainerPort);
        }
        else
        {
            RedisHost = "localhost";
            RedisPort = RedisContainerPort;
        }
    }

    private static void CreateContainers(out IContainer redisContainer)
    {
        redisContainer = new ContainerBuilder()
            .WithImage("redis/redis-stack")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("redis-cli PING"))
            .WithPortBinding(RedisContainerPort, true)
            .WithPortBinding(8001, true)
            .Build();

        redisContainer.StartAsync().Wait();
    }
}
