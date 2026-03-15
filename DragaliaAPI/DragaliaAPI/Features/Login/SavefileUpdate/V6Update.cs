using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

public partial class V6Update : ISavefileUpdate
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
                Log.FixingBuildingCurrentLevelDetailId(
                    this.logger,
                    build.BuildId,
                    build.Level,
                    build.FortPlantDetailId
                );

                build.Level--;
            }
        }
    }

    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Debug,
            "Fixing building {buildId}, current level {level}, detail id {detailId}"
        )]
        public static partial void FixingBuildingCurrentLevelDetailId(
            ILogger logger,
            long buildId,
            int level,
            int detailId
        );
    }
}
