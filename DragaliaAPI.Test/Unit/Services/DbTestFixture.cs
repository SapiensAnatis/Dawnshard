using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Test.Unit.Services
{
    public class DbTestFixture : IDisposable
    {
        public ApiContext apiContext { get; init; }

        public DbTestFixture()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase($"ApiRepositoryTest-{Guid.NewGuid()}")
                .Options;

            Mock<IWebHostEnvironment> mockEnvironment = new(MockBehavior.Loose);

            apiContext = new ApiContext(options, mockEnvironment.Object);
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
