using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Services;

public interface IUpdateDataService
{
    [Obsolete("Use the SaveChangesAsync overload that accepts a CancellationToken instead.")]
    Task<UpdateDataList> SaveChangesAsync();

    Task<UpdateDataList> SaveChangesAsync(CancellationToken cancellationToken);
}
