using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Database;

/// <summary>
/// Design-time factory class so that dotnet ef tools can construct ApiContext.
/// Allows registering migrations in the Database project, instead of the main project in which ApiContext is registered.
/// </summary>
public class ApiContextFactory : IDesignTimeDbContextFactory<ApiContext>
{
    public ApiContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Join(Directory.GetCurrentDirectory(), "..", "DragaliaAPI"))
            .AddJsonFile("appsettings.json")
            .Build();

        DbContextOptionsBuilder<ApiContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(
            DatabaseConfiguration.GetConnectionString(
                configuration.GetConnectionString("PostgresHost")
            )
        );

        return new ApiContext(optionsBuilder.Options);
    }
}
