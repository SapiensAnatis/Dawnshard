using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Features.DmodeDungeon;

public class DmodeCacheService(
    IDistributedCache cache,
    IPlayerIdentityService playerIdentityService
) : IDmodeCacheService
{
    public async Task StoreIngameInfo(DmodeIngameData data)
    {
        if (data.unique_key == string.Empty)
            data.unique_key = CacheKeys.IngameInfo(playerIdentityService.AccountId);

        await cache.SetStringAsync(data.unique_key, JsonSerializer.Serialize(data));
    }

    public async Task<DmodeIngameData> LoadIngameInfo()
    {
        string key = CacheKeys.IngameInfo(playerIdentityService.AccountId);

        string ingameData =
            await cache.GetStringAsync(key)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "No DmodeIngameData found");

        return JsonSerializer.Deserialize<DmodeIngameData>(ingameData)
            ?? throw new DragaliaException(
                ResultCode.CommonInvalidateJson,
                "Could not parse DmodeIngameData json"
            );
    }

    public async Task DeleteIngameInfo()
    {
        string key = CacheKeys.IngameInfo(playerIdentityService.AccountId);

        await cache.RemoveAsync(key);
    }

    public async Task StoreFloorInfo(DmodeFloorData data)
    {
        if (data.floor_key == string.Empty)
            data.floor_key = Guid.NewGuid().ToString();

        await cache.SetStringAsync(
            CacheKeys.DungeonFloor(data.floor_key),
            JsonSerializer.Serialize(data)
        );
    }

    public async Task<DmodeFloorData> LoadFloorInfo(string floorKey)
    {
        string key = CacheKeys.DungeonFloor(floorKey);

        string ingameData =
            await cache.GetStringAsync(key)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "No DmodeFloorData found");

        return JsonSerializer.Deserialize<DmodeFloorData>(ingameData)
            ?? throw new DragaliaException(
                ResultCode.CommonInvalidateJson,
                "Could not parse DmodeFloorData json"
            );
    }

    public async Task DeleteFloorInfo(string floorKey)
    {
        string key = CacheKeys.DungeonFloor(floorKey);

        await cache.RemoveAsync(key);
    }

    public async Task StorePlayRecord(DmodePlayRecord data)
    {
        string key = CacheKeys.PlayRecord(playerIdentityService.AccountId);

        await cache.SetStringAsync(key, JsonSerializer.Serialize(data));
    }

    public async Task<DmodePlayRecord> LoadPlayRecord()
    {
        string key = CacheKeys.PlayRecord(playerIdentityService.AccountId);

        string playRecord =
            await cache.GetStringAsync(key)
            ?? throw new DragaliaException(ResultCode.CommonDbError, "No DmodePlayRecord found");

        return JsonSerializer.Deserialize<DmodePlayRecord>(playRecord)
            ?? throw new DragaliaException(
                ResultCode.CommonInvalidateJson,
                "Could not parse DmodePlayRecord json"
            );
    }

    public async Task DeletePlayRecord()
    {
        string key = CacheKeys.PlayRecord(playerIdentityService.AccountId);

        await cache.RemoveAsync(key);
    }

    public async Task<bool> DoesPlayRecordExist()
    {
        string key = CacheKeys.PlayRecord(playerIdentityService.AccountId);

        return await cache.GetAsync(key) != null;
    }
}

file static class CacheKeys
{
    public static string IngameInfo(string playerId)
    {
        return $":dmode_ingame_info:{playerId}";
    }

    public static string DungeonFloor(string floorKey)
    {
        return $":dmode_floor:{floorKey}";
    }

    public static string PlayRecord(string playerId)
    {
        return $":dmode_play_record:{playerId}";
    }
}
