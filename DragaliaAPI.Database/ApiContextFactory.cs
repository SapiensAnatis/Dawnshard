using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DragaliaAPI.Database;

public class ApiContextFactory : IDesignTimeDbContextFactory<ApiContext>
{
    public ApiContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApiContext>();

        optionsBuilder.UseSqlServer("Server=sqlserver;User ID=SA;Password=DragaliaLost123");

        return new ApiContext(optionsBuilder.Options);
    }
}
