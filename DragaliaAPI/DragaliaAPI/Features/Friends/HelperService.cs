using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Owned;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Shared;
using DragaliaAPI.Features.Web.Settings;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Friends;

internal sealed partial class HelperService(
    IPartyRepository partyRepository,
    IDungeonRepository dungeonRepository,
    IUserDataRepository userDataRepository,
    IMapper mapper,
    IBonusService bonusService,
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    SettingsService settingsService,
    StaticHelperDataService staticDataService,
    RealHelperDataService realDataService
) : IHelperService
{
    public async Task<QuestGetSupportUserListResponse> GetHelpers(
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
        IHelperDataService dataService = await this.GetDataService(cancellationToken);

        return await dataService.GetHelper(viewerId, cancellationToken);
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

    public AtgenSupportData BuildHelperData(
        UserSupportList helperInfo,
        AtgenSupportUserDetailList helperDetails
    )
    {
        return new AtgenSupportData()
        {
            ViewerId = helperInfo.ViewerId,
            Name = helperInfo.Name,
            IsFriend = helperDetails.IsFriend,
            CharaData = mapper.Map<CharaList>(helperInfo.SupportChara),
            DragonData = mapper.Map<DragonList>(helperInfo.SupportDragon),
            WeaponBodyData = mapper.Map<GameWeaponBody>(helperInfo.SupportWeaponBody),
            CrestSlotType1CrestList = helperInfo.SupportCrestSlotType1List.Select(
                mapper.Map<GameAbilityCrest>
            ),
            CrestSlotType2CrestList = helperInfo.SupportCrestSlotType2List.Select(
                mapper.Map<GameAbilityCrest>
            ),
            CrestSlotType3CrestList = helperInfo.SupportCrestSlotType3List.Select(
                mapper.Map<GameAbilityCrest>
            ),
            TalismanData = mapper.Map<TalismanList>(helperInfo.SupportTalisman),
        };
    }

    public async Task<AtgenSupportUserDataDetail> BuildStaticSupportUserDataDetail(
        UserSupportList staticHelperInfo
    )
    {
        AtgenSupportUserDetailList helperDetail = new()
        {
            IsFriend = true,
            ViewerId = staticHelperInfo.ViewerId,
            GettableManaPoint = 50,
        };

        FortBonusList bonusList = await bonusService.GetBonusList();

        return new()
        {
            UserSupportData = staticHelperInfo,
            FortBonusList = bonusList,
            ManaCirclePieceIdList = Enumerable.Range(
                1,
                staticHelperInfo?.SupportChara.AdditionalMaxLevel == 20 ? 70 : 50
            ),
            DragonReliabilityLevel = 30,
            IsFriend = helperDetail.IsFriend,
            ApplySendStatus = 0,
        };
    }

    private async Task<IHelperDataService> GetDataService(CancellationToken cancellationToken)
    {
        PlayerSettings settings = await settingsService.GetSettings(cancellationToken);
        return settings.UseLegacyHelpers ? staticDataService : realDataService;
    }
}
