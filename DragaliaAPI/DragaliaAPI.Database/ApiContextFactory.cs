using DragaliaAPI.Shared.PlayerDetails;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Database;

[UsedImplicitly]
public class ApiContextFactory : IDesignTimeDbContextFactory<ApiContext>
{
    public ApiContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        PostgresOptions? postgresOptions = configuration
            .GetSection(nameof(PostgresOptions))
            ?.Get<PostgresOptions>();

        DbContextOptions<ApiContext> contextOptions = new DbContextOptionsBuilder<ApiContext>()
            .UseNpgsql(postgresOptions?.GetConnectionString("IDesignTimeDbContextFactory"))
            .Options;

        return new ApiContext(contextOptions, new StubPlayerIdentityService());
    }

    private class StubPlayerIdentityService : IPlayerIdentityService
    {
        public string AccountId =>
            throw new NotImplementedException(
                "StubPlayerIdentityService cannot function as an actual identity service"
            );

        public long ViewerId =>
            throw new NotImplementedException(
                "StubPlayerIdentityService cannot function as an actual identity service"
            );

        public IDisposable StartUserImpersonation(long? viewer = null, string? account = null) =>
            throw new NotImplementedException(
                "StubPlayerIdentityService cannot function as an actual identity service"
            );
    }
}
