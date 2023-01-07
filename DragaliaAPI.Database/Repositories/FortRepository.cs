using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Database.Repositories;

public class FortRepository : IFortRepository
{
    private readonly ApiContext ApiContext;

    public FortRepository(ApiContext apiContext)
    {
        this.ApiContext = apiContext;
    }

    public IQueryable<DbFortBuild> GetBuilds(string accountId) =>
        this.ApiContext.PlayerFortBuilds.Where(x => x.DeviceAccountId == accountId);
}
