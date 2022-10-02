using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Test.Unit.Services
{
    public class DbTestFixture : IDisposable
    {
        public ApiContext apiContext { get; init; }

        public DbTestFixture()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase("ApiRepositoryTest")
                .Options;
            apiContext = new ApiContext(options);
            apiContext.DeviceAccounts.Add(new DbDeviceAccount("id", "hashed password"));
            apiContext.SavefileUserData.Add(DbSavefileUserDataFactory.Create("id"));
            apiContext.SaveChanges();
        }

        public void Dispose()
        {
            apiContext.Dispose();
        }
    }
}
