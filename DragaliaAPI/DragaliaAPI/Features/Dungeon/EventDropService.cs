using System.Diagnostics;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.EventItemTypes;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Event;
using DragaliaAPI.Shared.MasterAsset.Models.QuestDrops;

namespace DragaliaAPI.Features.Dungeon;

public class EventDropService(IEventRepository eventRepository)
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

        int amount = this.rdm.Next(progress.Count);
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
            int roll = this.rdm.Next(101);

            bool isRare = roll > 95;
            List<int> table = isRare ? rare : normal;

            int drop = this.rdm.Next(table);

            if (!drops.TryAdd(drop, 1))
                drops[drop]++;

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

    public IEnumerable<DropEntity> GenerateEventMaterialDrops(QuestData quest)
    {
        int eventId = quest.Gid;

        if (!MasterAsset.EventData.TryGetValue(eventId, out EventData? evt))
        {
            // TODO: Add support for story events where all quests should drop rewards
            return [];
        }

        EventKindType type = evt.EventKindType;

        IEnumerable<DropEntity> drops = type switch
        {
            EventKindType.Build => this.GenerateBuildEventDrops(quest, evt),
            EventKindType.Raid => this.GenerateRaidEventDrops(quest, evt),
            EventKindType.Combat => this.GenerateCombatEventDrops(quest, evt),
            EventKindType.Clb01 => this.GenerateClb01EventDrops(quest, evt),
            EventKindType.Collect => throw new NotImplementedException(), // see above
            EventKindType.ExRush => this.GenerateExRushEventDrops(quest, evt),
            EventKindType.ExHunter => throw new NotImplementedException(), // TODO: This is similar to Raid events I think
            EventKindType.Simple => throw new NotImplementedException(), // Only item is 'Pup Grub' lol
            EventKindType.BattleRoyal => throw new NotImplementedException(),
            EventKindType.Earn => this.GenerateEarnEventDrops(quest, evt),
            _ => throw new UnreachableException(),
        };

        return drops;
    }

    private IEnumerable<DropEntity> GenerateBuildEventDrops(QuestData quest, EventData evt)
    {
        // https://dragalialost.wiki/w/Facility_Events
        DungeonTypes type = quest.DungeonType;
        VariationTypes variation = quest.VariationType;

        // Every Facility Event features 3 forms of currency, differently-named for each event. For simplicity, we will call them T1, T2, and T3 currency.

        // T2 and T3 currency are only used for redeeming shop items, and can appear commonly from Challenge Battle quests.
        if (type == DungeonTypes.Wave)
        {
            // T3 only drops from Challenge Battle quests

            // Typically, there are two challenge battle quests - an easier one and a harder one. We want to give more
            // rewards for the harder one. This previously used questData.VariationType, but that is inconsistent
            // between events; see https://github.com/SapiensAnatis/Dawnshard/issues/507.
            // This hack instead relies on the fact that the quest IDs for wave quests are always organized like:
            //     208170501  Season's Beatings: Expert
            //     208170502  Season's Beatings: Master
            // It therefore provides a multiplier of 1 for the hard quest, and 0.5 for the easy quest.
            double t3Multiplier = quest.Id % 10 / 2d;

            int t3Quantity = this.GenerateDropAmount(10 * t3Multiplier);
            yield return new DropEntity(evt.ViewEntityId3, evt.ViewEntityType3, t3Quantity);
        }

        // T1 and T2 drop from all quests
        // But more often on harder quests
        int t2Quantity = this.GenerateDropAmount(
            10d * (int)variation * (variation < VariationTypes.VeryHard ? 0.5 : 1)
        );
        yield return new DropEntity(evt.ViewEntityId2, evt.ViewEntityType2, t2Quantity);

        // T1 drops from every quest
        // But we incentivize playing the other non Challenge Battle quests for it
        int t1Quantity = this.GenerateDropAmount(
            10d * (int)variation * (type == DungeonTypes.Normal ? 1.5 : 1)
        );
        yield return new DropEntity(evt.ViewEntityId1, evt.ViewEntityType1, t1Quantity);

        // Each Facility Event features a differently-named variety of Event Points that can be collected during the event.
        // These points cannot be spent, but instead act as a score system, with rewards available depending on the total amount of points collected.
        // ===> See QuestCompletionService for those
    }

    private IEnumerable<DropEntity> GenerateRaidEventDrops(QuestData quest, EventData evt)
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
                int fragmentQuantity = this.GenerateDropAmount(
                    10d * (int)variation * (variation == VariationTypes.VeryHard ? 2 : 0.5)
                );
                yield return new DropEntity(
                    itemDict[RaidEventItemType.AdventItem1],
                    EntityTypes.RaidEventItem,
                    fragmentQuantity
                );

                int emblem1Quantity = this.GenerateDropAmount(20d * (int)variation);
                yield return new DropEntity(
                    itemDict[RaidEventItemType.RaidPoint1],
                    EntityTypes.RaidEventItem,
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
                    int quantity = this.GenerateDropAmount(
                        10d * (int)variation * (variation == VariationTypes.Hell ? 1.5 : 1)
                    );
                    yield return new DropEntity(
                        itemDict[RaidEventItemType.AdventItem2],
                        EntityTypes.RaidEventItem,
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
                int emblem2Quantity = this.GenerateDropAmount(
                    10d
                        * (int)variation
                        * (variation >= VariationTypes.Hell ? 5 * (int)variation : 1)
                );
                yield return new DropEntity(itemId, EntityTypes.RaidEventItem, emblem2Quantity);
                break;
            }
        }

        // Participating in the raid event will award Peregrine Blazons.
        // TODO: No idea what the quantity should be for this lol
        int blazonQuantity = this.GenerateDropAmount(50d * (int)variation);
        yield return new DropEntity(
            itemDict[RaidEventItemType.SummonPoint],
            EntityTypes.RaidEventItem,
            blazonQuantity
        );
    }

    private IEnumerable<DropEntity> GenerateCombatEventDrops(QuestData quest, EventData evt)
    {
        // https://dragalialost.wiki/w/Defensive_Events

        Dictionary<CombatEventItemType, int> itemDict = MasterAsset
            .CombatEventItem.Enumerable.Where(x => x.EventId == evt.Id)
            .ToDictionary(x => x.EventItemType, x => x.Id);

        VariationTypes variation = quest.VariationType;

        // General versions of Defensive Events have only one form of currency, Primal Crystals,
        // which can be exchanged in the Event Shop for various rewards.
        int primalCrystalQuantity = this.GenerateDropAmount(10d * (int)variation);
        yield return new DropEntity(
            itemDict[CombatEventItemType.ExchangeItem],
            EntityTypes.CombatEventItem,
            primalCrystalQuantity
        );

        // In general versions of defensive events, Stratagems are a resource that can be used to challenge EX defensive battles.
        // They can be obtained as drops from Master-difficulty quests.
        if (variation >= VariationTypes.VeryHard)
        {
            int stratagemQuantity = this.GenerateDropAmount(2d);
            yield return new DropEntity(
                itemDict[CombatEventItemType.AdventItem],
                EntityTypes.CombatEventItem,
                stratagemQuantity
            );
        }
    }

    private IEnumerable<DropEntity> GenerateClb01EventDrops(QuestData quest, EventData evt)
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
            int t2Quantity = this.GenerateDropAmount(10d * (int)variation);
            yield return new DropEntity(
                itemDict[Clb01EventItemType.ExchangeItem2],
                EntityTypes.Clb01EventItem,
                t2Quantity
            );
        }

        int t1Quantity = this.GenerateDropAmount(
            20d * (int)variation * (variation >= VariationTypes.Hard ? 2 : 1)
        );
        yield return new DropEntity(
            itemDict[Clb01EventItemType.ExchangeItem1],
            EntityTypes.Clb01EventItem,
            t1Quantity
        );
    }

    private IEnumerable<DropEntity> GenerateExRushEventDrops(QuestData quest, EventData evt)
    {
        // Mega Man event

        // The wiki has no explanation for this so this is just guessed

        Dictionary<ExRushEventItemType, int> itemDict = MasterAsset
            .ExRushEventItem.Enumerable.Where(x => x.EventId == evt.Id)
            .ToDictionary(x => x.EventItemType, x => x.Id);

        VariationTypes variation = quest.VariationType;

        if (variation >= VariationTypes.VeryHard)
        {
            int t2Quantity = this.GenerateDropAmount(5d * (int)variation);
            yield return new DropEntity(
                itemDict[ExRushEventItemType.ExRushPoint2],
                EntityTypes.ExRushEventItem,
                t2Quantity
            );
        }

        int t1Quantity = this.GenerateDropAmount(
            10d * (int)variation * (variation >= VariationTypes.Hard ? 2 : 1)
        );
        yield return new DropEntity(
            itemDict[ExRushEventItemType.ExRushPoint1],
            EntityTypes.ExRushEventItem,
            t1Quantity
        );
    }

    private IEnumerable<DropEntity> GenerateEarnEventDrops(QuestData quest, EventData evt)
    {
        // Invasion Events have two forms of currency, Guardian's Shields and Guardian's Swords,
        // which can be exchanged in the Event Shop for various rewards.

        Dictionary<EarnEventItemType, int> itemDict = MasterAsset
            .EarnEventItem.Enumerable.Where(x => x.EventId == evt.Id)
            .ToDictionary(x => x.EventItemType, x => x.Id);

        VariationTypes variation = quest.VariationType;

        int t1Quantity = this.GenerateDropAmount(
            10d * (int)variation * (variation >= VariationTypes.Hard ? 2 : 1)
        );
        int t2Quantity = this.GenerateDropAmount(
            10d * (int)variation * (variation <= VariationTypes.VeryHard ? 0.25 : 1)
        );

        yield return new DropEntity(
            itemDict[EarnEventItemType.ExchangeItem1],
            EntityTypes.EarnEventItem,
            t1Quantity
        );
        yield return new DropEntity(
            itemDict[EarnEventItemType.ExchangeItem2],
            EntityTypes.EarnEventItem,
            t2Quantity
        );
    }

    private int GenerateDropAmount(double average)
    {
        double val = this.rdm.Next(75, 126) / 100d;
        return (int)Math.Floor(average * val);
    }
}
