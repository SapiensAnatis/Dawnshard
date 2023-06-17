using System.Reflection.Emit;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class MissionProgressionService : IMissionProgressionService
{
    private readonly IMissionRepository missionRepository;
    private readonly Queue<Event> eventQueue;

    public MissionProgressionService(IMissionRepository missionRepository)
    {
        this.missionRepository = missionRepository;
        this.eventQueue = new Queue<Event>();
    }

    public void OnFortPlantUpgraded(FortPlants plant, int level)
    {
        this.eventQueue.Enqueue(
            new Event(MissionProgressType.FortPlantUpgraded, (int)plant, level)
        );
    }

    public void OnFortPlantBuilt(FortPlants plant)
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.FortPlantBuilt, (int)plant));
    }

    public void OnFortLevelup(int level)
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.FortLevelup, level));
    }

    public void OnQuestCleared(int questId)
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.QuestCleared, questId));
    }

    public void OnVoidBattleCleared()
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.VoidBattleCleared));
    }

    public void OnWeaponEarned(int id, int abilityId)
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.WeaponEarned, id, abilityId));
    }

    public void OnWyrmprintUpgraded(int id, int augmentId, int count)
    {
        this.eventQueue.Enqueue(
            new Event(MissionProgressType.WyrmprintUpgraded, id, augmentId, count)
        );
    }

    public async Task ProcessMissionEvents()
    {
        if (this.eventQueue.Count == 0)
            return;

        List<DbPlayerMission>? missionList = null;

        while (this.eventQueue.TryDequeue(out Event evt))
        {
            if (
                !MasterAsset.MissionProgressionInfo.TryGetValue(
                    evt.Type,
                    out MissionProgressionInfo? info
                )
            )
                continue;

            IEnumerable<int> affectedMissions = info.Requirements
                .Where(
                    x =>
                        x.Parameter == evt.Parameter
                        && x.Parameter2 == evt.Parameter2
                        && x.Parameter3 == evt.Parameter3
                )
                .SelectMany(x => x.Missions)
                .Select(x => x.Id)
                .ToList();

            if (affectedMissions.Any())
            {
                missionList ??= await this.missionRepository.Missions
                    .Where(x => x.State == MissionState.InProgress)
                    .ToListAsync();

                foreach (
                    DbPlayerMission progressingMission in missionList.Where(
                        x => affectedMissions.Contains(x.Id)
                    )
                )
                {
                    Mission mission = Mission.From(progressingMission.Type, progressingMission.Id);
                    progressingMission.Progress++;
                    if (progressingMission.Progress == mission.CompleteValue)
                    {
                        progressingMission.State = MissionState.Receivable;
                    }
                }
            }
        }
    }

    private readonly struct Event
    {
        public readonly MissionProgressType Type;
        public readonly int Parameter;
        public readonly int Parameter2;
        public readonly int Parameter3;

        public Event(
            MissionProgressType type,
            int parameter = -1,
            int parameter2 = -1,
            int parameter3 = -1
        )
        {
            Type = type;
            Parameter = parameter;
            Parameter2 = parameter2;
            Parameter3 = parameter3;
        }
    }
}
