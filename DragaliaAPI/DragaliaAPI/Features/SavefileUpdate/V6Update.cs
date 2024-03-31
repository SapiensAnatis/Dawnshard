using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V6Update : ISavefileUpdate
{
    private readonly IFortRepository fortRepository;
    private readonly ILogger<V6Update> logger;

    public V6Update(IFortRepository fortRepository, ILogger<V6Update> logger)
    {
        this.fortRepository = fortRepository;
        this.logger = logger;
    }

    public int SavefileVersion => 6;

    public async Task Apply()
    {
        foreach (
            DbFortBuild build in (await this.fortRepository.Builds.ToListAsync()).Where(x =>
                x.BuildStatus == FortBuildStatus.LevelUp
            )
        )
        {
            if (!MasterAsset.FortPlantDetail.TryGetValue(build.FortPlantDetailId, out _))
            {
                logger.LogDebug(
                    "Fixing building {buildId}, current level {level}, detail id {detailId}",
                    build.BuildId,
                    build.Level,
                    build.FortPlantDetailId
                );

                build.Level--;
            }
        }
    }
}
