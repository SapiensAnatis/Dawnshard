using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Features.SavefileUpdate;

/// <summary>
/// Service to apply <see cref="ISavefileUpdate"/> instances.
/// </summary>
public class SavefileUpdateService : ISavefileUpdateService
{
    private readonly ApiContext context;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly IEnumerable<ISavefileUpdate> savefileUpdates;
    private readonly ILogger<SavefileUpdateService> logger;

    public SavefileUpdateService(
        ApiContext context,
        IPlayerIdentityService playerIdentityService,
        IEnumerable<ISavefileUpdate> savefileUpdates,
        ILogger<SavefileUpdateService> logger
    )
    {
        this.context = context;
        this.playerIdentityService = playerIdentityService;
        this.savefileUpdates = savefileUpdates;
        this.logger = logger;
    }

    public async Task UpdateSavefile()
    {
        DbPlayer? player = await this.context.Players.FindAsync(
            this.playerIdentityService.ViewerId
        );

        if (player is null)
        {
            this.logger.LogInformation(
                "Could not find player {player}",
                this.playerIdentityService.AccountId
            );

            return;
        }

        Stopwatch s = new();

        foreach (ISavefileUpdate update in this.savefileUpdates.OrderBy(x => x.SavefileVersion))
        {
            if (player.SavefileVersion < update.SavefileVersion)
            {
                s.Restart();
                await update.Apply();
                s.Stop();

                player.SavefileVersion = update.SavefileVersion;

                this.logger.LogInformation(
                    "Applied savefile update {$update} (took: {time} ms)",
                    update,
                    s.ElapsedMilliseconds
                );
            }
        }

        // Do not need an UpdateDataList as this is called during /load/index
        await this.context.SaveChangesAsync();
    }
}
