using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Event;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset.Models.Event;

namespace DragaliaAPI.Features.Dungeon;

public class QuestDropService(ILogger<QuestEnemyService> logger, IEventRepository eventRepository)
    : IQuestDropService
{
    private readonly Random rdm = Random.Shared;

    public IEnumerable<Materials> GetDrops(int questId)
    {
        if (!MasterAsset.QuestDrops.TryGetValue(questId, out QuestDropInfo? questDropInfo))
        {
            logger.LogWarning("Unable to retrieve drop list for quest id {quest}", questId);
            return Enumerable.Empty<Materials>();
        }

        return questDropInfo.Material;
    }

    public async Task<IEnumerable<AtgenEventPassiveUpList>> GetEventPassiveDrops(QuestData quest)
    {
        // NOTE: QuestData is passed in here so if we ever want do do this weighted we dont have to change the signature

        Dictionary<int, int> drops = new();

        int eventId = quest.Gid;

        if (!MasterAsset.EventData.TryGetValue(eventId, out _))
        {
            return Enumerable.Empty<AtgenEventPassiveUpList>();
        }

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
        List<int> rare = info.Values.Where(x => x.Rarity == 1).Select(x => x.Id).ToList();

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
}
