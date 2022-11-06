using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Test.Integration;

public class IntegrationTestFixture : CustomWebApplicationFactory<Program>
{
    public IntegrationTestFixture()
    {
        this.SeedCache();
    }

    /// <summary>
    /// Seed the cache with a valid session, so that controllers can lookup database entries.
    /// </summary>
    public void SeedCache()
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

    public async Task AddCharacter(int id)
    {
        using IServiceScope scope = this.Services.CreateScope();
        IUnitRepository unitRepository =
            scope.ServiceProvider.GetRequiredService<IUnitRepository>();
        await unitRepository.AddCharas(this.DeviceAccountId, new List<Charas>() { (Charas)id });
    }
}
