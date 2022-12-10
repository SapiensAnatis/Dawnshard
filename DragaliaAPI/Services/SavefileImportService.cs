using System.Diagnostics;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;

namespace DragaliaAPI.Services;

public class SavefileService : ISavefileService
{
    private readonly ApiContext apiContext;
    private readonly IMapper mapper;
    private readonly ILogger<SavefileService> logger;

    public SavefileService(ApiContext apiContext, IMapper mapper, ILogger<SavefileService> logger)
    {
        this.apiContext = apiContext;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task Import(long viewerId, LoadIndexData savefile)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        DbPlayerUserData userData = await this.apiContext.PlayerUserData.SingleAsync(
            x => x.ViewerId == viewerId
        );

        string deviceAccountId = userData.DeviceAccountId;
        this.logger.LogInformation(
            "Beginning savefile import for account {deviceAccountId}",
            deviceAccountId
        );

        await this.ClearSavefile(deviceAccountId);

        apiContext.PlayerUserData.Remove(userData);
        apiContext.PlayerUserData.Add(
            this.mapper.Map<DbPlayerUserData>(
                savefile.user_data,
                opts =>
                    opts.AfterMap(
                        (src, dest) =>
                        {
                            dest.ViewerId = viewerId;
                            dest.DeviceAccountId = deviceAccountId;
                        }
                    )
            )
        );

        this.apiContext.PlayerCharaData.AddRange(
            savefile.chara_list.Select(
                x => MapWithDeviceAccount<DbPlayerCharaData>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerDragonReliability.AddRange(
            savefile.dragon_reliability_list.Select(
                x => MapWithDeviceAccount<DbPlayerDragonReliability>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerDragonData.AddRange(
            savefile.dragon_list.Select(
                x => MapWithDeviceAccount<DbPlayerDragonData>(x, deviceAccountId)
            )
        );

        // Zero out dragon and talisman key ids, as these won't exist in my database
        var parties = savefile.party_list
            .Select(x => MapWithDeviceAccount<DbParty>(x, deviceAccountId))
            .ToList();

        foreach (var party in parties)
        {
            foreach (var unit in party.Units)
            {
                unit.EquipDragonKeyId = 0;
                unit.EquipTalismanKeyId = 0;
            }
        }

        this.apiContext.PlayerParties.AddRange(parties);

        this.apiContext.PlayerAbilityCrests.AddRange(
            savefile.ability_crest_list.Select(
                x => MapWithDeviceAccount<DbAbilityCrest>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerWeapons.AddRange(
            savefile.weapon_body_list.Select(
                x => MapWithDeviceAccount<DbWeaponBody>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerQuests.AddRange(
            savefile.quest_list.Select(x => MapWithDeviceAccount<DbQuest>(x, deviceAccountId))
        );

        this.apiContext.PlayerStoryState.AddRange(
            savefile.quest_story_list.Select(
                x => MapWithDeviceAccount<DbPlayerStoryState>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerStoryState.AddRange(
            savefile.unit_story_list.Select(
                x => MapWithDeviceAccount<DbPlayerStoryState>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerStoryState.AddRange(
            savefile.castle_story_list.Select(
                x => MapWithDeviceAccount<DbPlayerStoryState>(x, deviceAccountId)
            )
        );

        this.apiContext.PlayerStorage.AddRange(
            savefile.material_list.Select(
                x => MapWithDeviceAccount<DbPlayerMaterial>(x, deviceAccountId)
            )
        );

        // TODO: kaleido prints, unit sets
        // TODO much later: halidom, endeavours, kaleido data

        this.logger.LogInformation(
            "Mapping completed after {seconds} s",
            stopwatch.Elapsed.TotalSeconds
        );

        await apiContext.SaveChangesAsync();

        this.logger.LogInformation(
            "Saved changes after {seconds} s",
            stopwatch.Elapsed.TotalSeconds
        );
    }

    public async Task ClearSavefile(string deviceAccountId)
    {
        await this.apiContext.PlayerCharaData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerDragonReliability
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerDragonData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerAbilityCrests
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerStoryState
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerQuests
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerParties
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerPartyUnits
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerWeapons
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
        await this.apiContext.PlayerStorage
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
    }

    private TDest MapWithDeviceAccount<TDest>(object source, string deviceAccountId)
        where TDest : IDbHasAccountId
    {
        return mapper.Map<TDest>(
            source,
            opts => opts.AfterMap((src, dest) => dest.DeviceAccountId = deviceAccountId)
        );
    }
}
