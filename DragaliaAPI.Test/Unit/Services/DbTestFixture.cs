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

namespace DragaliaAPI.Test.Unit.Services;

public class DbTestFixture : IDisposable
{
    public ApiContext ApiContext { get; init; }

    public Mock<IWebHostEnvironment> MockEnvironment { get; init; }

    public DbTestFixture()
    {
        var options = new DbContextOptionsBuilder<ApiContext>()
            .UseInMemoryDatabase($"ApiRepositoryTest-{Guid.NewGuid()}")
            .Options;

        MockEnvironment = new(MockBehavior.Loose);

        ApiContext = new ApiContext(options, MockEnvironment.Object);
        ApiContext.DeviceAccounts.Add(new DbDeviceAccount("id", "hashed password"));
        ApiContext.PlayerUserData.Add(DbSavefileUserDataFactory.Create("id"));
        ApiContext.SaveChanges();
    }

    public void Dispose()
    {
        ApiContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
