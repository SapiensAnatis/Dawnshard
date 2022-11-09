using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Protocol.Core.Types;

namespace DragaliaAPI.Test.Integration;

public class IntegrationTestFixture : CustomWebApplicationFactory<Program>
{
    public IntegrationTestFixture()
    {
        this.SeedDatabase();
        this.SeedCache();
    }

    /// <summary>
    /// The device account ID which links to the seeded savefiles <see cref="SeedDatabase"/>
    /// </summary>
    public string DeviceAccountId => "logged_in_id";

    public string PreparedDeviceAccountId => "prepared_id";

    public async Task AddCharacter(int id)
    {
        using IServiceScope scope = this.Services.CreateScope();
        IUnitRepository unitRepository =
            scope.ServiceProvider.GetRequiredService<IUnitRepository>();
        await unitRepository.AddCharas(this.DeviceAccountId, new List<Charas>() { (Charas)id });
        await unitRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Seed the cache with a valid session, so that controllers can lookup database entries.
    /// </summary>
    private void SeedCache()
    {
        IDistributedCache cache = this.Services.GetRequiredService<IDistributedCache>();
        // Downside of making Session a private nested class: I have to type this manually :(
        string preparedSessionJson = """
            {
                "SessionId": "prepared_session_id",
                "DeviceAccountId": "prepared_id"
            }
            """;
        cache.SetString(":session:id_token:id_token", preparedSessionJson);

        string sessionJson = """
                {
                    "SessionId": "session_id",
                    "DeviceAccountId": "logged_in_id"
                }
                """;
        cache.SetString(":session:session_id:session_id", sessionJson);
        cache.SetString(":session_id:device_account_id:logged_in_id", "session_id");
    }

    private void SeedDatabase()
    {
        ApiContext context = this.Services.GetRequiredService<ApiContext>();
        IDeviceAccountRepository repository =
            this.Services.GetRequiredService<IDeviceAccountRepository>();

        context.DeviceAccounts.AddRange(
            new List<DbDeviceAccount>()
            {
                // Password is a hash of the string "password"
                new("id", "NMvdakTznEF6khwWcz17i6GTnDA="),
            }
        );

        repository.CreateNewSavefile(this.PreparedDeviceAccountId);
        repository.CreateNewSavefile(this.DeviceAccountId);
        context.SaveChanges();
    }
}
