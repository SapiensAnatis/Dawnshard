namespace DragaliaAPI.Database.Repositories;

public interface IBaseRepository
{
    Task<int> SaveChangesAsync();
}
