using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Fort;

public interface IFortService
{
    Task<IEnumerable<BuildList>> GetBuildList();
    Task<FortDetail> AddCarpenter(PaymentTypes paymentType);
    Task<FortDetail> GetFortDetail();

    Task<FortGetMultiIncomeData> CollectIncome(IEnumerable<long> idsToCollect);

    Task BuildAtOnce(PaymentTypes paymentType, long buildId);
    Task LevelupAtOnce(PaymentTypes paymentType, long buildId);

    Task<DbFortBuild> CancelBuild(long buildId);
    Task<DbFortBuild> CancelLevelup(long buildId);

    Task EndBuild(long buildId);
    Task EndLevelup(long buildId);

    Task<DbFortBuild> BuildStart(FortPlants fortPlantId, int positionX, int positionZ);

    Task<DbFortBuild> LevelupStart(long buildId);
    Task<DbFortBuild> Move(long buildId, int afterPositionX, int afterPositionZ);

    Task ClearPlantNewStatuses(IEnumerable<FortPlants> plantIds);
    Task ClearPlantNewStatuses(IEnumerable<long> buildIds);

    Task<AtgenProductionRp> GetRupieProduction();
    Task<AtgenProductionRp> GetDragonfruitProduction();
    Task<AtgenProductionRp> GetStaminaProduction();
    Task<(int HalidomLevel, int SmithyLevel)> GetCoreLevels();
}
