using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IFortRepository
{
    IQueryable<DbFortBuild> GetBuilds(string accountId);
    IQueryable<DbFortBuild> Builds { get; }
    Task<DbFortDetail> GetFortDetails();
    Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel);
    Task GetFortPlantIdList(IEnumerable<int> fort_plant_id_list);
    Task UpdateFortMaximumCarpenter(string accountId, int carpenter_num);
    Task<DbFortBuild> GetBuilding(string deviceAccountId, long buildId);
    Task AddBuild(DbFortBuild build);
    void DeleteBuild(DbFortBuild build);
    Task<DbFortBuild> UpgradeAtOnce(
        DbPlayerUserData userData,
        string accountId,
        long buildId,
        PaymentTypes paymentType
    );
    Task<DbFortBuild> CancelUpgrade(string accountId, long buildId);
    Task<DbFortDetail> UpdateCarpenterUsage(string accountId);
    void ConsumePaymentCost(DbPlayerUserData userData, PaymentTypes paymentType, int paymentCost);
}
