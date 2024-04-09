using System.Diagnostics;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Features.Dmode;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Mapping.Mapperly;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services.Game;

public class UpdateDataService(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    IMissionService missionService,
    IMissionProgressionService missionProgressionService,
    IPresentService presentService,
    IEventService eventService,
    IDmodeService dmodeService
) : IUpdateDataService
{
    [Obsolete("Use the SaveChangesAsync overload that accepts a CancellationToken instead.")]
    public Task<UpdateDataList> SaveChangesAsync() => this.SaveChangesAsync(default);

    public async Task<UpdateDataList> SaveChangesAsync(CancellationToken cancellationToken)
    {
        await missionProgressionService.ProcessMissionEvents(cancellationToken);

        List<IDbPlayerData> entities = apiContext
            .ChangeTracker.Entries<IDbPlayerData>()
            .Where(x =>
                x.State is EntityState.Modified or EntityState.Added
                && x.Entity.ViewerId == playerIdentityService.ViewerId
            )
            .Select(x => x.Entity)
            .ToList();

        await apiContext.SaveChangesAsync(cancellationToken);

        return await MapUpdateDataList(entities);
    }

    private async Task<UpdateDataList> MapUpdateDataList(List<IDbPlayerData> entities)
    {
        UpdateDataList list = new();

        foreach (IDbPlayerData entity in entities)
        {
            switch (entity)
            {
                case DbPlayerUserData userData:
                    list.UserData = userData.ToUserData();
                    break;
                case DbPlayerCharaData charaData:
                    list.CharaList ??= [];
                    list.CharaList.Add(charaData.ToCharaList());
                    break;
                case DbPlayerDragonData dragonData:
                    list.DragonList ??= [];
                    list.DragonList.Add(dragonData.ToDragonList());
                    break;
                case DbPlayerDragonReliability reliability:
                    list.DragonReliabilityList ??= [];
                    list.DragonReliabilityList.Add(reliability.ToDragonReliabilityList());
                    break;
                case DbWeaponBody weaponBody:
                    list.WeaponBodyList ??= [];
                    list.WeaponBodyList.Add(weaponBody.ToWeaponBodyList());
                    break;
                case DbWeaponSkin weaponSkin:
                    list.WeaponSkinList ??= [];
                    list.WeaponSkinList.Add(weaponSkin.ToWeaponSkinList());
                    break;
                case DbAbilityCrest abilityCrest:
                    list.AbilityCrestList ??= [];
                    list.AbilityCrestList.Add(abilityCrest.ToAbilityCrestList());
                    break;
                case DbAbilityCrestSet abilityCrestSet:
                    list.AbilityCrestSetList ??= [];
                    list.AbilityCrestSetList.Add(abilityCrestSet.ToAbilityCrestSetList());
                    break;
                case DbParty party:
                    list.PartyList ??= [];
                    list.PartyList.Add(party.ToPartyList());
                    break;
                case DbPlayerStoryState { StoryType: StoryTypes.Quest } story:
                    list.QuestStoryList ??= [];
                    list.QuestStoryList.Add(story.ToQuestStoryList());
                    break;
                case DbPlayerStoryState { StoryType: StoryTypes.Chara or StoryTypes.Dragon } story:
                    list.UnitStoryList ??= [];
                    list.UnitStoryList.Add(story.ToUnitStoryList());
                    break;
                case DbPlayerStoryState { StoryType: StoryTypes.Castle } story:
                    list.CastleStoryList ??= [];
                    list.CastleStoryList.Add(story.ToCastleStoryList());
                    break;
                case DbPlayerStoryState { StoryType: StoryTypes.DungeonMode } story:
                    list.DmodeStoryList ??= [];
                    list.DmodeStoryList.Add(story.ToDmodeStoryList());
                    break;
                case DbPlayerMaterial material:
                    list.MaterialList ??= [];
                    list.MaterialList.Add(material.ToMaterialList());
                    break;
                case DbPlayerDragonGift dragonGift:
                    list.DragonGiftList ??= [];
                    list.DragonGiftList.Add(dragonGift.ToDragonGift());
                    break;
                case DbQuest quest:
                    list.QuestList ??= [];
                    list.QuestList.Add(quest.ToQuestList());
                    break;
                case DbFortBuild build:
                    list.BuildList ??= [];
                    list.BuildList.Add(build.ToBuildList());
                    break;
                case DbWeaponPassiveAbility weaponPassive:
                    list.WeaponPassiveAbilityList ??= [];
                    list.WeaponPassiveAbilityList.Add(weaponPassive.ToWeaponPassiveAbilityList());
                    break;
                case DbPlayerUseItem useItem:
                    list.ItemList ??= [];
                    list.ItemList.Add(useItem.ToItemList());
                    break;
                case DbTalisman talisman:
                    list.TalismanList ??= [];
                    list.TalismanList.Add(talisman.ToTalismanList());
                    break;
                case DbSummonTicket summonTicket:
                    list.SummonTicketList ??= [];
                    list.SummonTicketList.Add(summonTicket.ToSummonTicketList());
                    break;
                case DbQuestEvent questEvent:
                    list.QuestEventList ??= [];
                    list.QuestEventList.Add(questEvent.ToQuestEventList());
                    break;
                case DbQuestTreasureList questTreasure:
                    list.QuestTreasureList ??= [];
                    list.QuestTreasureList.Add(questTreasure.ToQuestTreasureList());
                    break;
                case DbPartyPower partyPower:
                    list.PartyPowerData = partyPower.ToPartyPowerData();
                    break;
                case DbPlayerQuestWall wall:
                    list.QuestWallList ??= [];
                    list.QuestWallList.Add(wall.ToQuestWallList());
                    break;
                case DbPlayerBannerData bannerData:
                    list.SummonPointList ??= [];
                    list.SummonPointList.Add(bannerData.MapToSummonPointList());
                    break;
                default:
                    continue;
            }
        }

        IEnumerable<DbPlayerMission> updatedMissions = entities.OfType<DbPlayerMission>();

        if (updatedMissions.Any())
        {
            ILookup<MissionType, DbPlayerMission> missionsLookup = entities
                .OfType<DbPlayerMission>()
                .ToLookup(x => x.Type);
            if (missionsLookup.Contains(MissionType.MainStory))
            {
                list.CurrentMainStoryMission = await missionService.GetCurrentMainStoryMission();

                list.QuestEntryConditionList = (await missionService.GetEntryConditions()).ToList();
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

        static async Task<List<T>> GetEventDataList<T>(
            IEnumerable<int> ids,
            Func<int, Task<T?>> dataCreator
        )
        {
            List<T> list = new();
            foreach (int id in ids)
            {
                T? data = await dataCreator(id);
                if (data is not null)
                    list.Add(data);
            }

            return list;
        }
    }
}
