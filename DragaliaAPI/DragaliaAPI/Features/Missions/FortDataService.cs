using DragaliaAPI.Features.Fort;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Missions;

public class FortDataService(IFortRepository fortRepository)
{
    private readonly IFortRepository fortRepository = fortRepository;

    public async Task<int> GetTotalFortLevel()
    {
        return await fortRepository
                .Builds.Where(x => x.PlantId != FortPlants.TheHalidom)
                .SumAsync(x =>
                    x.BuildEndDate == DateTimeOffset.UnixEpoch ? x.Level : (int?)x.Level - 1
                ) ?? 0;
    }

    public async Task<int> GetMaxFortPlantLevel(FortPlants plant)
    {
        return await fortRepository
                .Builds.Where(x => x.PlantId == plant)
                .Select(x => (int?)x.Level)
                .MaxAsync() ?? 0;
    }

    public async Task<int> GetFortPlantCount(FortPlants plant)
    {
        return await fortRepository.Builds.CountAsync(x =>
            x.PlantId == plant
            && x.PositionX != -1 // not in storage
            && x.PositionZ != -1
        );
    }
}
