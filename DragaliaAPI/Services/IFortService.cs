using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Services;

public interface IFortService
{
    IEnumerable<BuildList> GetBuildList();
    Task<FortDetail> GetFortDetails();
    Task<FortDetail> AddCarpenter(string accountId, PaymentTypes paymentType);
    Task<FortDetail> CompleteAtOnce(string accountId, PaymentTypes paymentType, long buildId);
    Task<DbFortBuild> CancelUpgrade(long buildId);
    Task<FortDetail> EndUpgrade(long buildId);
    Task<DbFortBuild> BuildStart(
        string accountId,
        FortPlants fortPlantId,
        int level,
        int positionX,
        int positionZ
    );
    Task<DbFortBuild> LevelupStart(string accountId, long buildId);
    Task<DbFortBuild> Move(long buildId, int afterPositionX, int afterPositionZ);
    Task GetFortPlantIdList(IEnumerable<int> fortPlantIdList);
}
