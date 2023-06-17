using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IFortRepository
{
    IQueryable<DbFortBuild> Builds { get; }

    Task InitializeFort();
    Task InitializeSmithy();
    Task AddDojos();
    Task<DbFortDetail> GetFortDetail();
    Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel);
    Task GetFortPlantIdList(IEnumerable<int> fortPlantIdList);
    Task UpdateFortMaximumCarpenter(int carpenterNum);
    Task<DbFortBuild> GetBuilding(long buildId);
    Task AddBuild(DbFortBuild build);
    Task AddToStorage(FortPlants plant, int level);
    void DeleteBuild(DbFortBuild build);
    void ConsumeUpgradeAtOnceCost(
        DbPlayerUserData userData,
        DbFortBuild build,
        PaymentTypes paymentType
    );
    void ConsumePaymentCost(DbPlayerUserData userData, PaymentTypes paymentType, int paymentCost);
    Task<int> GetActiveCarpenters();
}
