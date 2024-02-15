using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IUpdateDataService
{
    Task<UpdateDataList> SaveChangesAsync();
}
