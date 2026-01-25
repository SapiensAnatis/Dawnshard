using System.Data;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Owned;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace DragaliaAPI.Features.Web.Settings;

internal sealed partial class SettingsService(
    ApiContext apiContext,
    IMemoryCache cache,
    IPlayerIdentityService playerIdentityService
)
{
    public async Task<PlayerSettings> GetSettings(CancellationToken cancellationToken)
    {
        if (cache.Get<PlayerSettings>(this.GetCacheKey()) is { } settings)
        {
            return settings;
        }

        PlayerSettings retrievedSettings =
            await apiContext
                .PlayerSettings.AsNoTracking()
                .Select(x => x.SettingsJson)
                .FirstOrDefaultAsync(cancellationToken)
            ?? new PlayerSettings();

        cache.Set(this.GetCacheKey(), retrievedSettings);

        return retrievedSettings;
    }

    public async Task SetSettings(PlayerSettings settings, CancellationToken cancellationToken)
    {
        cache.Remove(this.GetCacheKey());

        await using IDbContextTransaction transaction =
            await apiContext.Database.BeginTransactionAsync(cancellationToken);

        await apiContext.PlayerSettings.ExecuteDeleteAsync(cancellationToken);

        apiContext.PlayerSettings.Add(
            new DbSettings() { ViewerId = playerIdentityService.ViewerId, SettingsJson = settings }
        );

        await apiContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private string GetCacheKey() => $"{playerIdentityService.ViewerId}:settings";
}
