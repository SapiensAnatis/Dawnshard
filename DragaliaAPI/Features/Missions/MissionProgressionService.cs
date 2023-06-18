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
    private readonly ILogger<MissionProgressionService> logger;

    private readonly Queue<Event> eventQueue;

    public MissionProgressionService(
        IMissionRepository missionRepository,
        ILogger<MissionProgressionService> logger
    )
    {
        this.missionRepository = missionRepository;
        this.logger = logger;

        this.eventQueue = new Queue<Event>();
    }

    public void OnFortPlantUpgraded(FortPlants plant)
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.FortPlantUpgraded, (int)plant));
    }

    public void OnFortPlantBuilt(FortPlants plant)
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.FortPlantBuilt, (int)plant));
    }

    public void OnFortLevelup()
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.FortLevelup));
    }

    public void OnQuestCleared(int questId)
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.QuestCleared, questId));
    }

    public void OnVoidBattleCleared()
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.VoidBattleCleared));
    }

    public void OnWeaponEarned(UnitElement element, int stars, WeaponSeries series)
    {
        this.eventQueue.Enqueue(
            new Event(MissionProgressType.WeaponEarned, (int)element, stars, (int)series)
        );
    }

    public void OnWeaponRefined(UnitElement element, int stars, WeaponSeries series)
    {
        this.eventQueue.Enqueue(
            new Event(MissionProgressType.WeaponRefined, (int)element, stars, (int)series)
        );
    }

    public void OnWyrmprintAugmentBuildup(PlusCountType type)
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.WyrmprintAugmentBuildup, (int)type));
    }

    public void OnCharacterBuildup(PlusCountType type)
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.CharacterBuildup, (int)type));
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
            {
                continue;
            }

            IEnumerable<int> affectedMissions = info.Requirements
                .Where(
                    x =>
                        (x.Parameter == -1 || x.Parameter == evt.Parameter)
                        && (x.Parameter2 == -1 || x.Parameter2 == evt.Parameter2)
                        && (x.Parameter3 == -1 || x.Parameter3 == evt.Parameter3)
                        && (x.Parameter4 == -1 || x.Parameter4 == evt.Parameter4)
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
                        x => affectedMissions.Contains(x.Id) && x.State == MissionState.InProgress
                    )
                )
                {
                    Mission mission = Mission.From(progressingMission.Type, progressingMission.Id);
                    progressingMission.Progress++;
                    if (progressingMission.Progress == mission.CompleteValue)
                    {
                        this.logger.LogDebug("Completed quest {questId}", progressingMission.Id);
                        progressingMission.State = MissionState.Completed;
                    }
                    else
                    {
                        this.logger.LogDebug(
                            "Progressed quest {questId} ({currentCount}/{totalCount}",
                            progressingMission.Id,
                            progressingMission.Progress,
                            mission.CompleteValue
                        );
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
        public readonly int Parameter4;

        public Event(
            MissionProgressType type,
            int parameter = -1,
            int parameter2 = -1,
            int parameter3 = -1,
            int parameter4 = -1
        )
        {
            Type = type;
            Parameter = parameter;
            Parameter2 = parameter2;
            Parameter3 = parameter3;
            Parameter4 = parameter4;
        }
    }
}
