using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services.Game;

public class UpdateDataService(
    ApiContext apiContext,
    IMapper mapper,
    IPlayerIdentityService playerIdentityService,
    IMissionService missionService,
    IMissionProgressionService missionProgressionService,
    IPresentService presentService,
    IEventService eventService,
    IDmodeService dmodeService
) : IUpdateDataService
{
    public async Task<UpdateDataList> SaveChangesAsync()
    {
        await missionProgressionService.ProcessMissionEvents();

        List<IDbPlayerData> entities = apiContext
            .ChangeTracker.Entries<IDbPlayerData>()
            .Where(x =>
                (x.State is EntityState.Modified or EntityState.Added)
                && x.Entity.ViewerId == playerIdentityService.ViewerId
            )
            .Select(x => x.Entity)
            .ToList();

        await apiContext.SaveChangesAsync();

        return await MapUpdateDataList(entities);
    }

    private async Task<UpdateDataList> MapUpdateDataList(List<IDbPlayerData> entities)
    {
        UpdateDataList list =
            new()
            {
                UserData = ConvertEntities<UserData, DbPlayerUserData>(entities)?.Single(), // Can't use SingleOrDefault if the list itself is null
                CharaList = ConvertEntities<CharaList, DbPlayerCharaData>(entities),
                DragonList = ConvertEntities<DragonList, DbPlayerDragonData>(entities),
                DragonReliabilityList = ConvertEntities<
                    DragonReliabilityList,
                    DbPlayerDragonReliability
                >(entities),
                WeaponBodyList = ConvertEntities<WeaponBodyList, DbWeaponBody>(entities),
                WeaponSkinList = ConvertEntities<WeaponSkinList, DbWeaponSkin>(entities),
                AbilityCrestList = ConvertEntities<AbilityCrestList, DbAbilityCrest>(entities),
                AbilityCrestSetList = ConvertEntities<AbilityCrestSetList, DbAbilityCrestSet>(
                    entities
                ),
                PartyList = ConvertEntities<PartyList, DbParty>(entities),
                QuestStoryList = ConvertEntities<QuestStoryList, DbPlayerStoryState>(
                    entities,
                    x => x.StoryType == StoryTypes.Quest
                ),
                UnitStoryList = ConvertEntities<UnitStoryList, DbPlayerStoryState>(
                    entities,
                    x => x.StoryType == StoryTypes.Chara || x.StoryType == StoryTypes.Dragon
                ),
                CastleStoryList = ConvertEntities<CastleStoryList, DbPlayerStoryState>(
                    entities,
                    x => x.StoryType == StoryTypes.Castle
                ),
                DmodeStoryList = ConvertEntities<DmodeStoryList, DbPlayerStoryState>(
                    entities,
                    x => x.StoryType == StoryTypes.DungeonMode
                ),
                MaterialList = ConvertEntities<MaterialList, DbPlayerMaterial>(entities),
                DragonGiftList = ConvertEntities<DragonGiftList, DbPlayerDragonGift>(
                    entities,
                    x => x.DragonGiftId > DragonGifts.GoldenChalice
                ),
                QuestList = ConvertEntities<QuestList, DbQuest>(entities),
                BuildList = ConvertEntities<BuildList, DbFortBuild>(entities),
                WeaponPassiveAbilityList = ConvertEntities<
                    WeaponPassiveAbilityList,
                    DbWeaponPassiveAbility
                >(entities),
                ItemList = ConvertEntities<ItemList, DbPlayerUseItem>(entities),
                TalismanList = ConvertEntities<TalismanList, DbTalisman>(entities),
                SummonTicketList = ConvertEntities<SummonTicketList, DbSummonTicket>(entities),
                QuestEventList = ConvertEntities<QuestEventList, DbQuestEvent>(entities),
                QuestTreasureList = ConvertEntities<QuestTreasureList, DbQuestTreasureList>(
                    entities
                ),
                PartyPowerData = ConvertEntities<PartyPowerData, DbPartyPower>(entities)?.Single(),
                QuestWallList = ConvertEntities<QuestWallList, DbPlayerQuestWall>(entities)
            };

        IEnumerable<DbPlayerMission> updatedMissions = entities.OfType<DbPlayerMission>();

        if (updatedMissions.Any())
        {
            ILookup<MissionType, DbPlayerMission> missionsLookup = entities
                .OfType<DbPlayerMission>()
                .ToLookup(x => x.Type);
            if (missionsLookup.Contains(MissionType.MainStory))
            {
                list.CurrentMainStoryMission = await missionService.GetCurrentMainStoryMission();

                list.QuestEntryConditionList = await missionService.GetEntryConditions();
            }

            list.MissionNotice = await missionService.GetMissionNotice(missionsLookup);
        }

        if (
            entities.OfType<DbPlayerPresent>().Any()
            || entities.OfType<DbPlayerPresentHistory>().Any()
        )
        {
            list.PresentNotice = await presentService.GetPresentNotice();
        }

        List<DbPlayerShopInfo> updatedInfo = entities.OfType<DbPlayerShopInfo>().ToList();

        if (updatedInfo.Count > 0)
        {
            list.ShopNotice = new ShopNotice(updatedInfo.First().DailySummonCount == 0);
        }

        List<int> updatedEvents = new();

        updatedEvents.AddRange(
            entities.OfType<DbPlayerEventItem>().Select(x => x.EventId).Distinct()
        );
        updatedEvents.AddRange(
            entities.OfType<DbPlayerEventData>().Select(x => x.EventId).Distinct()
        );

        if (updatedEvents.Count > 0)
        {
            ILookup<EventKindType, int> updatedEventIds = updatedEvents
                .Select(x => MasterAsset.EventData[x])
                .ToLookup(x => x.EventKindType, x => x.Id);

            foreach (IGrouping<EventKindType, int> eventGroup in updatedEventIds)
            {
                switch (eventGroup.Key)
                {
                    case EventKindType.Raid:
                        list.RaidEventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetRaidEventUserData
                        );
                        break;
                    case EventKindType.Build:
                        list.BuildEventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetBuildEventUserData
                        );
                        break;
                    case EventKindType.Random:
                        list.MazeEventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetMazeEventUserData
                        );
                        break;
                    case EventKindType.Collect:
                        list.CollectEventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetCollectEventUserData
                        );
                        break;
                    case EventKindType.Clb01:
                        list.Clb01EventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetClb01EventUserData
                        );
                        break;
                    case EventKindType.ExRush:
                        list.ExRushEventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetExRushEventUserData
                        );
                        break;
                    case EventKindType.ExHunter:
                        list.ExHunterEventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetExHunterEventUserData
                        );
                        break;
                    case EventKindType.Simple:
                        list.SimpleEventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetSimpleEventUserData
                        );
                        break;
                    case EventKindType.Combat:
                        list.CombatEventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetCombatEventUserData
                        );
                        break;
                    case EventKindType.Earn:
                        list.EarnEventUserList = await GetEventDataList(
                            eventGroup,
                            eventService.GetEarnEventUserData
                        );
                        break;
                    default:
                        throw new UnreachableException("Invalid EventKindType");
                }
            }
        }

        List<DbPlayerEventPassive> updatedPassives = entities
            .OfType<DbPlayerEventPassive>()
            .ToList();

        if (updatedPassives.Count > 0)
        {
            List<EventPassiveList> passiveList = new();

            foreach (int updatedEventId in updatedPassives.Select(x => x.EventId).Distinct())
            {
                passiveList.Add(await eventService.GetEventPassiveList(updatedEventId));
            }

            list.EventPassiveList = passiveList;
        }

        if (entities.OfType<DbPlayerDmodeInfo>().Any())
        {
            DmodeInfo info = await dmodeService.GetInfo();
            if (info.IsEntry)
                // This is done to ensure that the change tracker does not mess anything up
                list.DmodeInfo = info;
        }

        return list;

        static async Task<IEnumerable<T>> GetEventDataList<T>(
            IEnumerable<int> ids,
            Func<int, Task<T>> dataCreator
        )
        {
            List<T> list = new();
            foreach (int id in ids)
            {
                list.Add(await dataCreator(id));
            }

            return list;
        }
    }

    private List<TNetwork>? ConvertEntities<TNetwork, TDatabase>(
        IEnumerable<IDbPlayerData> baseEntries,
        Func<TDatabase, bool>? filterPredicate = null
    )
        where TDatabase : IDbPlayerData
    {
        List<TDatabase> typedEntries = filterPredicate is not null
            ? baseEntries.OfType<TDatabase>().Where(filterPredicate).ToList()
            : baseEntries.OfType<TDatabase>().ToList();

        return typedEntries.Any()
            ? typedEntries.Select(x => mapper.Map<TNetwork>(x)).ToList()
            : null;
    }
}
