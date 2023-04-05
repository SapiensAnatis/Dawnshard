using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Repositories;

public interface IFortRepository
{
    //Task<IEnumerable<DbFortBuild>> GetBuilds();
    IQueryable<DbFortBuild> Builds { get; }
    Task<DbFortDetail> GetFortDetails();
    Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel);
    Task GetFortPlantIdList(IEnumerable<int> fort_plant_id_list);
    Task UpdateFortMaximumCarpenter(int carpenter_num);
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
