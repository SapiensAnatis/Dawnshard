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
using DragaliaAPI.Database.Repositories;

namespace DragaliaAPI.Services;

public class SavefileService : ISavefileService
{
    private readonly ApiContext apiContext;
    private readonly IDeviceAccountRepository deviceAccountRepository;
    private readonly IMapper mapper;
    private readonly ILogger<SavefileService> logger;

    public SavefileService(
        ApiContext apiContext,
        IDeviceAccountRepository deviceAccountRepository,
        IMapper mapper,
        ILogger<SavefileService> logger
    )
    {
        this.apiContext = apiContext;
        this.deviceAccountRepository = deviceAccountRepository;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task Import(string deviceAccountId, LoadIndexData savefile)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Preserve the existing viewer ID if there is one.
        // Could reassign, but this makes it easier for people to remember their ID.
        long? oldViewerId = await this.apiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .Select(x => x.ViewerId)
            .SingleOrDefaultAsync();

        this.logger.LogInformation(
            "Beginning savefile import for account {accountId}",
            deviceAccountId
        );

        await this.Delete(deviceAccountId);

        apiContext.PlayerUserData.Add(
            this.mapper.Map<DbPlayerUserData>(
                savefile.user_data,
                opts =>
                    opts.AfterMap(
                        (_, dest) =>
                        {
                            dest.ViewerId = oldViewerId ?? default;
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
        List<DbParty> parties = savefile.party_list
            .Select(x => MapWithDeviceAccount<DbParty>(x, deviceAccountId))
            .ToList();

        foreach (DbParty party in parties)
        {
            foreach (DbPartyUnit unit in party.Units)
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

        this.apiContext.PlayerTalismans.AddRange(
            savefile.talisman_list.Select(x => MapWithDeviceAccount<DbTalisman>(x, deviceAccountId))
        );

        // TODO: unit sets
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

    private async Task Delete(string deviceAccountId)
    {
        await this.apiContext.PlayerUserData
            .Where(x => x.DeviceAccountId == deviceAccountId)
            .ExecuteDeleteAsync();
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

    public async Task Reset(string deviceAccountId)
    {
        await this.Delete(deviceAccountId);

        await this.deviceAccountRepository.CreateNewSavefile(deviceAccountId);
        await this.apiContext.SaveChangesAsync();
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
