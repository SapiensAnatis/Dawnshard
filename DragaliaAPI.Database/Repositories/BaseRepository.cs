namespace DragaliaAPI.Database.Repositories;

public abstract class BaseRepository : IBaseRepository
{
    private readonly ApiContext apiContext;

    public BaseRepository(ApiContext apiContext)
    {
        this.apiContext = apiContext;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await this.apiContext.SaveChangesAsync();
    }
}
