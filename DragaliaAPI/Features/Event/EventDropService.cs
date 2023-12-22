using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Event;

namespace DragaliaAPI.Features.Event;

public class EventDropService(IRewardService rewardService, IEventRepository eventRepository)
    : IEventDropService
{
    // NOTE: This file will have comments from the relevant event wiki page. For further reference, see https://dragalialost.wiki/w/Special_Events

    private readonly Random rdm = Random.Shared;

    public async Task<IEnumerable<AtgenEventPassiveUpList>> ProcessEventPassiveDrops(
        QuestData quest
    )
    {
        // Raid Boosts are passive effects to improve the player's combat performance during the Raid event.

        Dictionary<int, int> drops = new();

        int eventId = quest.Gid;

        if (!MasterAsset.EventData.TryGetValue(eventId, out EventData? evt))
        {
            return Enumerable.Empty<AtgenEventPassiveUpList>();
        }

        if (evt.IsMemoryEvent || evt.EventKindType != EventKindType.Raid)
            return Enumerable.Empty<AtgenEventPassiveUpList>();

        Dictionary<int, int> progress = new();
        Dictionary<int, (int Id, int Rarity, int MaxProgress)> info = new();

        foreach (
            DbPlayerEventPassive passive in await eventRepository.GetEventPassivesAsync(eventId)
        )
        {
            int id = passive.PassiveId;
            EventPassive passiveInfo = MasterAsset.EventPassive[id];

            if (passive.Progress >= passiveInfo.MaxGrowValue)
                continue;

            progress[id] = passive.Progress;
            info[id] = (passiveInfo.Id, passiveInfo.ViewRarity, passiveInfo.MaxGrowValue);
        }

        if (progress.Count == 0)
            return Enumerable.Empty<AtgenEventPassiveUpList>();

        int amount = rdm.Next(0, progress.Count);
        if (amount == 0)
            return Enumerable.Empty<AtgenEventPassiveUpList>();

        List<int> normal = info.Values.Where(x => x.Rarity == 0).Select(x => x.Id).ToList();

        // Rare Raid Boosts are available as well, but unlike regular Raid Boosts they can be earned only from Raid battles.
        List<int> rare =
            quest.DungeonType is DungeonTypes.Raid
                ? info.Values.Where(x => x.Rarity == 1).Select(x => x.Id).ToList()
                : normal;

        for (int i = 0; i < amount; i++)
        {
            int roll = rdm.Next(100);

            bool isRare = roll > 95;
            List<int> table = isRare ? rare : normal;

            int drop = table[rdm.Next(table.Count - 1)];

            if (drops.ContainsKey(drop))
                drops[drop]++;
            else
                drops[drop] = 1;

            progress[drop]++;
            if (progress[drop] == info[drop].MaxProgress)
            {
                progress.Remove(drop);
                info.Remove(drop);
                if (isRare)
                {
                    rare.Remove(drop);
                    if (rare.Count == 0)
                        rare = normal;
                }
                else
                {
                    normal.Remove(drop);
                    if (normal.Count == 0)
                        normal = rare;
                }

                if (normal.Count == 0 && rare.Count == 0)
                    break;
            }
        }

        foreach ((int passiveId, int quantity) in drops)
        {
            await eventRepository.AddEventPassiveProgressAsync(eventId, passiveId, quantity);
        }

        return drops.Select(x => new AtgenEventPassiveUpList(x.Key, x.Value));
    }

    public async Task<IEnumerable<AtgenDropAll>> ProcessEventMaterialDrops(
        QuestData quest,
        PlayRecord record,
        double buildDropMultiplier
    )
    {
        int eventId = quest.Gid;

        if (!MasterAsset.EventData.TryGetValue(eventId, out EventData? evt))
        {
            // TODO: Add support for story events where all quests should drop rewards
            return Enumerable.Empty<AtgenDropAll>();
        }

        EventKindType type = evt.EventKindType;

        IEnumerable<Entity> drops = type switch
        {
            EventKindType.Build => ProcessBuildEventDrops(quest, evt, record, buildDropMultiplier),
            EventKindType.Raid => ProcessRaidEventDrops(quest, evt, record),
            EventKindType.Combat => ProcessCombatEventDrops(quest, evt, record),
            EventKindType.Clb01 => ProcessClb01EventDrops(quest, evt, record),
            EventKindType.Collect => throw new NotImplementedException(), // see above
            EventKindType.ExRush => ProcessExRushEventDrops(quest, evt, record),
            EventKindType.ExHunter => throw new NotImplementedException(), // TODO: This is similar to Raid events I think
            EventKindType.Simple => throw new NotImplementedException(), // Only item is 'Pup Grub' lol
            EventKindType.BattleRoyal => throw new NotImplementedException(),
            EventKindType.Earn => ProcessEarnEventDrops(quest, evt, record),
            _ => throw new UnreachableException()
        };

        List<AtgenDropAll> dropList = new();
        foreach (Entity drop in drops)
        {
            if (drop.Quantity <= 0)
                continue;

            await rewardService.GrantReward(drop);
            dropList.Add(
                new AtgenDropAll(
                    drop.Id,
                    drop.Type,
                    drop.Quantity,
                    0,
                    drop.Type == EntityTypes.BuildEventItem ? (float)buildDropMultiplier : 0
                )
            );
        }

        return dropList;
    }

    private IEnumerable<Entity> ProcessBuildEventDrops(
        QuestData quest,
        EventData evt,
        PlayRecord record,
        double buildDropMultiplier
    )
    {
        // https://dragalialost.wiki/w/Facility_Events
        DungeonTypes type = quest.DungeonType;
        VariationTypes variation = quest.VariationType;

        // Every Facility Event features 3 forms of currency, differently-named for each event. For simplicity, we will call them T1, T2, and T3 currency.

        // T2 and T3 currency are only used for redeeming shop items, and can appear commonly from Challenge Battle quests.
        if (type == DungeonTypes.Wave)
        {
            // T3 only drops from Challenge Battle quests
            int t3Quantity = GenerateDropAmount(
                10 * record.wave * ((variation - VariationTypes.Hard) / 2d) * buildDropMultiplier
            );
            yield return new Entity(evt.ViewEntityType3, evt.ViewEntityId3, t3Quantity);
        }

        // T1 and T2 drop from all quests
        // But more often on harder quests
        int t2Quantity = GenerateDropAmount(
            10d
                * (int)variation
                * (variation < VariationTypes.VeryHard ? 0.5 : 1)
                * buildDropMultiplier
        );
        yield return new Entity(evt.ViewEntityType2, evt.ViewEntityId2, t2Quantity);

        // T1 drops from every quest
        // But we incentivize playing the other non Challenge Battle quests for it
        int t1Quantity = GenerateDropAmount(
            10d * (int)variation * (type == DungeonTypes.Normal ? 1.5 : 1) * buildDropMultiplier
        );
        yield return new Entity(evt.ViewEntityType1, evt.ViewEntityId1, t1Quantity);

        // Each Facility Event features a differently-named variety of Event Points that can be collected during the event.
        // These points cannot be spent, but instead act as a score system, with rewards available depending on the total amount of points collected.
        // ===> See QuestCompletionService for those
    }

    private IEnumerable<Entity> ProcessRaidEventDrops(
        QuestData quest,
        EventData evt,
        PlayRecord record
    )
    {
        // All quests cost stamina, without any need for Otherworld Fragments, Otherworld Gems, Omega Keys.
        if (evt.IsMemoryEvent)
            yield break;

        // https://dragalialost.wiki/w/Raid_Events
        DungeonTypes type = quest.DungeonType;
        VariationTypes variation = quest.VariationType;

        Dictionary<RaidEventItemType, int> itemDict = MasterAsset
            .RaidEventItem.Enumerable.Where(x => x.RaidEventId == evt.Id)
            .ToDictionary(x => x.RaidEventItemType, x => x.Id);

        switch (type)
        {
            // Otherworld Fragments are a resource that can be used alongside getherwings to challenge raid battles. They are primarily obtained as drops from boss battles.
            case DungeonTypes.Normal:
            {
                int fragmentQuantity = GenerateDropAmount(
                    10d * (int)variation * (variation == VariationTypes.VeryHard ? 2 : 0.5)
                );
                yield return new Entity(
                    EntityTypes.RaidEventItem,
                    itemDict[RaidEventItemType.AdventItem1],
                    fragmentQuantity
                );

                int emblem1Quantity = GenerateDropAmount(20d * (int)variation);
                yield return new Entity(
                    EntityTypes.RaidEventItem,
                    itemDict[RaidEventItemType.RaidPoint1],
                    emblem1Quantity
                );
                break;
            }
            case DungeonTypes.Raid
            or DungeonTypes.RaidSingle:
            {
                // Otherworld Gems are a resource that can be used to challenge EX-difficulty Raid battles.
                // They can be obtained as drops from Expert or Nightmare-difficulty raid battles.
                if (variation is VariationTypes.Extreme or >= VariationTypes.Hell)
                {
                    int quantity = GenerateDropAmount(
                        10d * (int)variation * (variation == VariationTypes.Hell ? 1.5 : 1)
                    );
                    yield return new Entity(
                        EntityTypes.RaidEventItem,
                        itemDict[RaidEventItemType.AdventItem2],
                        quantity
                    );
                }

                // Beginner-difficulty raid battles are the primary source of Silver Emblems,
                // while Expert difficulty gives Gold Emblems.
                int itemId = itemDict[
                    variation >= VariationTypes.Extreme
                        ? RaidEventItemType.RaidPoint3
                        : RaidEventItemType.RaidPoint2
                ];

                // Many raid events provide access to an EX Raid that can be accessed with Otherworld Gems after clearing the Expert-difficulty raid battle.
                // Rewards primarily include a significantly larger amount of Gold Emblems than Expert difficulty offers.
                int emblem2Quantity = GenerateDropAmount(
                    10d
                        * (int)variation
                        * (variation >= VariationTypes.Hell ? 5 * (int)variation : 1)
                );
                yield return new Entity(EntityTypes.RaidEventItem, itemId, emblem2Quantity);
                break;
            }
        }

        // Participating in the raid event will award Peregrine Blazons.
        // TODO: No idea what the quantity should be for this lol
        int blazonQuantity = GenerateDropAmount(50d * (int)variation);
        yield return new Entity(
            EntityTypes.RaidEventItem,
            itemDict[RaidEventItemType.SummonPoint],
            blazonQuantity
        );
    }

    private IEnumerable<Entity> ProcessCombatEventDrops(
        QuestData quest,
        EventData evt,
        PlayRecord record
    )
    {
        // https://dragalialost.wiki/w/Defensive_Events

        Dictionary<CombatEventItemType, int> itemDict = MasterAsset
            .CombatEventItem.Enumerable.Where(x => x.EventId == evt.Id)
            .ToDictionary(x => x.EventItemType, x => x.Id);

        VariationTypes variation = quest.VariationType;

        // General versions of Defensive Events have only one form of currency, Primal Crystals,
        // which can be exchanged in the Event Shop for various rewards.
        int primalCrystalQuantity = GenerateDropAmount(10d * (int)variation);
        yield return new Entity(
            EntityTypes.CombatEventItem,
            itemDict[CombatEventItemType.ExchangeItem],
            primalCrystalQuantity
        );

        // In general versions of defensive events, Stratagems are a resource that can be used to challenge EX defensive battles.
        // They can be obtained as drops from Master-difficulty quests.
        if (variation >= VariationTypes.VeryHard)
        {
            int stratagemQuantity = GenerateDropAmount(2d);
            yield return new Entity(
                EntityTypes.CombatEventItem,
                itemDict[CombatEventItemType.AdventItem],
                stratagemQuantity
            );
        }
    }

    private IEnumerable<Entity> ProcessClb01EventDrops(
        QuestData quest,
        EventData evt,
        PlayRecord record
    )
    {
        // Also https://dragalialost.wiki/w/Defensive_Events

        Dictionary<Clb01EventItemType, int> itemDict = MasterAsset
            .Clb01EventItem.Enumerable.Where(x => x.EventId == evt.Id)
            .ToDictionary(x => x.EventItemType, x => x.Id);

        DungeonTypes type = quest.DungeonType;
        VariationTypes variation = quest.VariationType;

        // T2 currency can only be obtained from event quests made available in the second half of the event
        // TODO: I have no idea how to check this, hopefully DungeonType should be good enough
        if (type != DungeonTypes.Normal)
        {
            int t2Quantity = GenerateDropAmount(10d * (int)variation);
            yield return new Entity(
                EntityTypes.Clb01EventItem,
                itemDict[Clb01EventItemType.ExchangeItem2],
                t2Quantity
            );
        }

        int t1Quantity = GenerateDropAmount(
            20d * (int)variation * (variation >= VariationTypes.Hard ? 2 : 1)
        );
        yield return new Entity(
            EntityTypes.Clb01EventItem,
            itemDict[Clb01EventItemType.ExchangeItem1],
            t1Quantity
        );
    }

    private IEnumerable<Entity> ProcessExRushEventDrops(
        QuestData quest,
        EventData evt,
        PlayRecord record
    )
    {
        // Mega Man event

        // The wiki has no explanation for this so this is just guessed

        Dictionary<ExRushEventItemType, int> itemDict = MasterAsset
            .ExRushEventItem.Enumerable.Where(x => x.EventId == evt.Id)
            .ToDictionary(x => x.EventItemType, x => x.Id);

        VariationTypes variation = quest.VariationType;

        if (variation >= VariationTypes.VeryHard)
        {
            int t2Quantity = GenerateDropAmount(5d * (int)variation);
            yield return new Entity(
                EntityTypes.ExRushEventItem,
                itemDict[ExRushEventItemType.ExRushPoint2],
                t2Quantity
            );
        }

        int t1Quantity = GenerateDropAmount(
            10d * (int)variation * (variation >= VariationTypes.Hard ? 2 : 1)
        );
        yield return new Entity(
            EntityTypes.ExRushEventItem,
            itemDict[ExRushEventItemType.ExRushPoint1],
            t1Quantity
        );
    }

    private IEnumerable<Entity> ProcessEarnEventDrops(
        QuestData quest,
        EventData evt,
        PlayRecord record
    )
    {
        // Invasion Events have two forms of currency, Guardian's Shields and Guardian's Swords,
        // which can be exchanged in the Event Shop for various rewards.

        Dictionary<EarnEventItemType, int> itemDict = MasterAsset
            .EarnEventItem.Enumerable.Where(x => x.EventId == evt.Id)
            .ToDictionary(x => x.EventItemType, x => x.Id);

        VariationTypes variation = quest.VariationType;

        int t1Quantity = GenerateDropAmount(
            10d * (int)variation * (variation >= VariationTypes.Hard ? 2 : 1)
        );
        int t2Quantity = GenerateDropAmount(
            10d * (int)variation * (variation <= VariationTypes.VeryHard ? 0.25 : 1)
        );

        yield return new Entity(
            EntityTypes.EarnEventItem,
            itemDict[EarnEventItemType.ExchangeItem1],
            t1Quantity
        );
        yield return new Entity(
            EntityTypes.EarnEventItem,
            itemDict[EarnEventItemType.ExchangeItem2],
            t2Quantity
        );
    }

    private int GenerateDropAmount(double average)
    {
        double val = rdm.Next(75, 125) / 100d;
        return (int)Math.Floor(average * val);
    }
}
