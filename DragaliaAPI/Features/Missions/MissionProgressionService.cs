using System.Numerics;
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

    public void OnItemSummon()
    {
        this.eventQueue.Enqueue(new Event(MissionProgressType.ItemSummon));
    }

    public async Task ProcessMissionEvents()
    {
        if (this.eventQueue.Count == 0)
            return;

        List<DbPlayerMission>? missionList = null;

        while (this.eventQueue.TryDequeue(out Event evt))
        {
            this.logger.LogDebug("Processing mission progression event {@event}", evt);

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
                        (x.Parameter is null || x.Parameter == evt.Parameter)
                        && (x.Parameter2 is null || x.Parameter2 == evt.Parameter2)
                        && (x.Parameter3 is null || x.Parameter3 == evt.Parameter3)
                        && (x.Parameter4 is null || x.Parameter4 == evt.Parameter4)
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

    private readonly record struct Event(
        MissionProgressType Type,
        int? Parameter = null,
        int? Parameter2 = null,
        int? Parameter3 = null,
        int? Parameter4 = null
    );
}
