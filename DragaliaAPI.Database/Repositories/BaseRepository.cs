using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
