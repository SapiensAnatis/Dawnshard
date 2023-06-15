using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IFortRepository
{
    IQueryable<DbFortBuild> Builds { get; }

    Task InitializeFort();
    Task<DbFortDetail> GetFortDetail();
    Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel);
    Task GetFortPlantIdList(IEnumerable<int> fortPlantIdList);
    Task UpdateFortMaximumCarpenter(int carpenterNum);
    Task<DbFortBuild> GetBuilding(long buildId);
    Task AddBuild(DbFortBuild build);
    void DeleteBuild(DbFortBuild build);
    Task<DbFortBuild> UpgradeAtOnce(
        DbPlayerUserData userData,
        long buildId,
        PaymentTypes paymentType
    );
    void ConsumePaymentCost(DbPlayerUserData userData, PaymentTypes paymentType, int paymentCost);
    Task<int> GetActiveCarpenters();
}
