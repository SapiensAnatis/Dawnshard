using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.SavefileUpdate;

public class V6Update : ISavefileUpdate
{
    private readonly IFortRepository fortRepository;

    public V6Update(IFortRepository fortRepository)
    {
        this.fortRepository = fortRepository;
    }

    public int SavefileVersion => 6;

    public async Task Apply()
    {
        foreach (
            DbFortBuild build in (await this.fortRepository.Builds.ToListAsync()).Where(
                x => x.BuildStatus == FortBuildStatus.LevelUp
            )
        )
        {
            if (!MasterAsset.FortPlant.TryGetValue(build.FortPlantDetailId, out _))
                build.Level--;
        }
    }
}
