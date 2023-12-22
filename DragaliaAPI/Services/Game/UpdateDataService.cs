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
            .Where(
                x =>
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
                user_data = ConvertEntities<UserData, DbPlayerUserData>(entities)?.Single(), // Can't use SingleOrDefault if the list itself is null
                chara_list = ConvertEntities<CharaList, DbPlayerCharaData>(entities),
                dragon_list = ConvertEntities<DragonList, DbPlayerDragonData>(entities),
                dragon_reliability_list = ConvertEntities<
                    DragonReliabilityList,
                    DbPlayerDragonReliability
                >(entities),
                weapon_body_list = ConvertEntities<WeaponBodyList, DbWeaponBody>(entities),
                weapon_skin_list = ConvertEntities<WeaponSkinList, DbWeaponSkin>(entities),
                ability_crest_list = ConvertEntities<AbilityCrestList, DbAbilityCrest>(entities),
                ability_crest_set_list = ConvertEntities<AbilityCrestSetList, DbAbilityCrestSet>(
                    entities
                ),
                party_list = ConvertEntities<PartyList, DbParty>(entities),
                quest_story_list = ConvertEntities<QuestStoryList, DbPlayerStoryState>(
                    entities,
                    x => x.StoryType == StoryTypes.Quest
                ),
                unit_story_list = ConvertEntities<UnitStoryList, DbPlayerStoryState>(
                    entities,
                    x => x.StoryType == StoryTypes.Chara || x.StoryType == StoryTypes.Dragon
                ),
                castle_story_list = ConvertEntities<CastleStoryList, DbPlayerStoryState>(
                    entities,
                    x => x.StoryType == StoryTypes.Castle
                ),
                dmode_story_list = ConvertEntities<DmodeStoryList, DbPlayerStoryState>(
                    entities,
                    x => x.StoryType == StoryTypes.DungeonMode
                ),
                material_list = ConvertEntities<MaterialList, DbPlayerMaterial>(entities),
                dragon_gift_list = ConvertEntities<DragonGiftList, DbPlayerDragonGift>(
                    entities,
                    x => x.DragonGiftId > DragonGifts.GoldenChalice
                ),
                quest_list = ConvertEntities<QuestList, DbQuest>(entities),
                build_list = ConvertEntities<BuildList, DbFortBuild>(entities),
                weapon_passive_ability_list = ConvertEntities<
                    WeaponPassiveAbilityList,
                    DbWeaponPassiveAbility
                >(entities),
                item_list = ConvertEntities<ItemList, DbPlayerUseItem>(entities),
                talisman_list = ConvertEntities<TalismanList, DbTalisman>(entities),
                summon_ticket_list = ConvertEntities<SummonTicketList, DbSummonTicket>(entities),
                quest_event_list = ConvertEntities<QuestEventList, DbQuestEvent>(entities),
                quest_treasure_list = ConvertEntities<QuestTreasureList, DbQuestTreasureList>(
                    entities
                ),
                party_power_data = ConvertEntities<PartyPowerData, DbPartyPower>(entities)
                    ?.Single(),
                quest_wall_list = ConvertEntities<QuestWallList, DbPlayerQuestWall>(entities)
            };

        IEnumerable<DbPlayerMission> updatedMissions = entities.OfType<DbPlayerMission>();

        if (updatedMissions.Any())
        {
            ILookup<MissionType, DbPlayerMission> missionsLookup = entities
                .OfType<DbPlayerMission>()
                .ToLookup(x => x.Type);
            if (missionsLookup.Contains(MissionType.MainStory))
            {
                list.current_main_story_mission = await missionService.GetCurrentMainStoryMission();

                list.quest_entry_condition_list = await missionService.GetEntryConditions();
            }

            list.mission_notice = await missionService.GetMissionNotice(missionsLookup);
        }

        if (
            entities.OfType<DbPlayerPresent>().Any()
            || entities.OfType<DbPlayerPresentHistory>().Any()
        )
        {
            list.present_notice = await presentService.GetPresentNotice();
        }

        List<DbPlayerShopInfo> updatedInfo = entities.OfType<DbPlayerShopInfo>().ToList();

        if (updatedInfo.Count > 0)
        {
            list.shop_notice = new ShopNotice(updatedInfo.First().DailySummonCount == 0);
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
                        list.raid_event_user_list = await GetEventDataList(
                            eventGroup,
                            eventService.GetRaidEventUserData
                        );
                        break;
                    case EventKindType.Build:
                        list.build_event_user_list = await GetEventDataList(
                            eventGroup,
                            eventService.GetBuildEventUserData
                        );
                        break;
                    case EventKindType.Random:
                        list.maze_event_user_list = await GetEventDataList(
                            eventGroup,
                            eventService.GetMazeEventUserData
                        );
                        break;
                    case EventKindType.Collect:
                        list.collect_event_user_list = await GetEventDataList(
                            eventGroup,
                            eventService.GetCollectEventUserData
                        );
                        break;
                    case EventKindType.Clb01:
                        list.clb_01_event_user_list = await GetEventDataList(
                            eventGroup,
                            eventService.GetClb01EventUserData
                        );
                        break;
                    case EventKindType.ExRush:
                        list.ex_rush_event_user_list = await GetEventDataList(
                            eventGroup,
                            eventService.GetExRushEventUserData
                        );
                        break;
                    case EventKindType.ExHunter:
                        list.ex_hunter_event_user_list = await GetEventDataList(
                            eventGroup,
                            eventService.GetExHunterEventUserData
                        );
                        break;
                    case EventKindType.Simple:
                        list.simple_event_user_list = await GetEventDataList(
                            eventGroup,
                            eventService.GetSimpleEventUserData
                        );
                        break;
                    case EventKindType.Combat:
                        list.combat_event_user_list = await GetEventDataList(
                            eventGroup,
                            eventService.GetCombatEventUserData
                        );
                        break;
                    case EventKindType.Earn:
                        list.earn_event_user_list = await GetEventDataList(
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

            list.event_passive_list = passiveList;
        }

        if (entities.OfType<DbPlayerDmodeInfo>().Any())
        {
            DmodeInfo info = await dmodeService.GetInfo();
            if (info.is_entry)
                // This is done to ensure that the change tracker does not mess anything up
                list.dmode_info = info;
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
