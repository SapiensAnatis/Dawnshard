using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Fort;

public interface IFortRepository
{
    IQueryable<DbFortBuild> Builds { get; }

    Task InitializeFort();
    Task InitializeSmithy();
    Task AddDojos();
    Task AddDragontree();
    Task<DbFortDetail> GetFortDetail();
    Task<bool> CheckPlantLevel(FortPlants plant, int requiredLevel);
    Task UpdateFortMaximumCarpenter(int carpenterNum);
    Task<DbFortBuild> GetBuilding(long buildId);
    Task AddBuild(DbFortBuild build);
    Task AddToStorage(
        FortPlants plant,
        int? level = null,
        int quantity = 1,
        bool checkQuantity = false
    );
    void DeleteBuild(DbFortBuild build);
    Task<int> GetActiveCarpenters();
}
