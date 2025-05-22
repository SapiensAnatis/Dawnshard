using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Owned;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Web.Settings;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Friends;

internal sealed class HelperService(
    IPartyRepository partyRepository,
    IDungeonRepository dungeonRepository,
    IUserDataRepository userDataRepository,
    IMapper mapper,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    SettingsService settingsService,
    StaticHelperDataService staticDataService,
    RealHelperDataService realDataService,
    ILogger<HelperService> logger
) : IHelperService
{
    public async Task<QuestGetSupportUserListResponse> GetHelperList(
        CancellationToken cancellationToken
    )
    {
        IHelperDataService dataService = await this.GetDataService(cancellationToken);

        return await dataService.GetHelperList(cancellationToken);
    }

    public async Task<UserSupportList?> GetHelper(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug("Looking up UserSupportList for ID: {ViewerId}", viewerId);

        IHelperDataService dataService = await this.GetDataService(cancellationToken);

        return await dataService.GetHelper(viewerId, cancellationToken);
    }

    public async Task<AtgenSupportUserDataDetail?> GetHelperDetail(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug("Looking up AtgenSupportUserDataDetail for ID: {ViewerId}", viewerId);

        IHelperDataService dataService = await this.GetDataService(cancellationToken);

        return await dataService.GetHelperDataDetail(viewerId, cancellationToken);
    }

    public async Task UseHelper(long supportViewerId, CancellationToken cancellationToken = default)
    {
        IHelperDataService dataService = await this.GetDataService(cancellationToken);

        await dataService.UseHelper(supportViewerId, cancellationToken);
    }

    public async Task<UserSupportList> GetLeadUnit(int partyNo)
    {
        DbPlayerUserData userData = await userDataRepository.GetUserDataAsync();

        IQueryable<DbPartyUnit> leadUnitQuery = partyRepository.GetPartyUnits(partyNo).Take(1);
        DbDetailedPartyUnit? detailedUnit = await dungeonRepository
            .BuildDetailedPartyUnit(leadUnitQuery, 0)
            .FirstAsync();

        UserSupportList supportList = new()
        {
            ViewerId = (ulong)userData.ViewerId,
            Name = userData.Name,
            LastLoginDate = userData.LastLoginTime,
            Level = userData.Level,
            EmblemId = userData.EmblemId,
            MaxPartyPower = 1000,
            Guild = new() { GuildId = 0 },
        };

        mapper.Map(detailedUnit, supportList);

        supportList.SupportCrestSlotType1List = supportList.SupportCrestSlotType1List.Where(x =>
            x != null
        );
        supportList.SupportCrestSlotType2List = supportList.SupportCrestSlotType2List.Where(x =>
            x != null
        );
        supportList.SupportCrestSlotType3List = supportList.SupportCrestSlotType3List.Where(x =>
            x != null
        );

        return supportList;
    }

    public async Task<SettingSupport> GetOwnHelper(CancellationToken cancellationToken)
    {
        return await apiContext
            .PlayerHelpers.ProjectToSettingSupport()
            .FirstAsync(cancellationToken);
    }

    public async Task<SettingSupport> SetOwnHelper(
        FriendSetSupportCharaRequest request,
        CancellationToken cancellationToken
    )
    {
        // Foreign keys will validate most of the entities are owned, but the foreign key to
        // the dragons table is only on DragonKeyId, due to that being the primary key of the
        // dragons table.
        // Explicitly validate the dragon is owned to account for this.
        bool dragonValid =
            request.DragonKeyId == 0
            || await apiContext
                .PlayerDragonData.Where(x => x.ViewerId == playerIdentityService.ViewerId)
                .AnyAsync(x => x.DragonKeyId == (long)request.DragonKeyId, cancellationToken);

        if (!dragonValid)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Dragon key id {request.DragonKeyId} is not owned"
            );
        }

        // Same thing for Kaleidoscape prints.
        bool talismanValid =
            request.TalismanKeyId == 0
            || await apiContext
                .PlayerTalismans.Where(x => x.ViewerId == playerIdentityService.ViewerId)
                .AnyAsync(x => x.TalismanKeyId == (long)request.TalismanKeyId, cancellationToken);

        if (!talismanValid)
        {
            throw new DragaliaException(
                ResultCode.CommonInvalidArgument,
                $"Talisman key id {request.TalismanKeyId} is not owned"
            );
        }

        DbPlayerHelper existingHelper = await apiContext
            .PlayerHelpers.AsTracking()
            .FirstAsync(cancellationToken);

        existingHelper.CharaId = request.CharaId;
        existingHelper.EquipDragonKeyId = (long?)NullIfZero(request.DragonKeyId);
        existingHelper.EquipWeaponBodyId = NullIfZeroEnum(request.WeaponBodyId);
        existingHelper.EquipCrestSlotType1CrestId1 = NullIfZeroEnum(request.CrestSlotType1CrestId1);
        existingHelper.EquipCrestSlotType1CrestId2 = NullIfZeroEnum(request.CrestSlotType1CrestId2);
        existingHelper.EquipCrestSlotType1CrestId3 = NullIfZeroEnum(request.CrestSlotType1CrestId3);
        existingHelper.EquipCrestSlotType2CrestId1 = NullIfZeroEnum(request.CrestSlotType2CrestId1);
        existingHelper.EquipCrestSlotType2CrestId2 = NullIfZeroEnum(request.CrestSlotType2CrestId2);
        existingHelper.EquipCrestSlotType3CrestId1 = NullIfZeroEnum(request.CrestSlotType3CrestId1);
        existingHelper.EquipCrestSlotType3CrestId2 = NullIfZeroEnum(request.CrestSlotType3CrestId2);
        existingHelper.EquipTalismanKeyId = (long?)NullIfZero(request.TalismanKeyId);

        return existingHelper.MapToSettingSupport();

        static ulong? NullIfZero(ulong value)
        {
            if (value == 0)
            {
                return null;
            }

            return value;
        }

        static TEnum? NullIfZeroEnum<TEnum>(TEnum value)
            where TEnum : struct, Enum
        {
            if ((int)(object)value == 0)
            {
                return null;
            }

            return value;
        }
    }

    public async Task<UserSupportList?> GetUserSupportList(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        HelperProjection? helper = await GetHelperProjectionOrDefault(viewerId, cancellationToken);

        return helper?.MapToUserSupportList();
    }

    private async Task<HelperProjection?> GetHelperProjectionOrDefault(
        long viewerId,
        CancellationToken cancellationToken
    )
    {
        return await apiContext
            .PlayerHelpers.IgnoreQueryFilters()
            .AsSplitQuery()
            .Where(x => x.ViewerId == viewerId)
            .ProjectToHelperProjection()
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<IHelperDataService> GetDataService(CancellationToken cancellationToken)
    {
        PlayerSettings settings = await settingsService.GetSettings(cancellationToken);
        return settings.UseLegacyHelpers ? staticDataService : realDataService;
    }
}
