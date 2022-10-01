using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;

namespace DragaliaAPI.Test.Unit.Services;

public class ApiRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly ApiRepository apiRepository;
    private readonly ApiContext apiContext;

    private readonly DbDeviceAccount account = new("id", "hashed password");
    private readonly DbSavefileUserData playerInfo = DbSavefileUserDataFactory.Create("id");

    public ApiRepositoryTest(DbTestFixture data)
    {
        apiContext = data.apiContext;

        apiRepository = new(apiContext);
    }


    [Fact]
    public async Task AddNewDeviceAccount_CanGetAfterwards()
    {
        await apiRepository.AddNewDeviceAccount("id 2", "hashed password 2");
        DbDeviceAccount? result = await apiRepository.GetDeviceAccountById("id 2");

        result.Should().NotBeNull();
        result!.Id.Should().Be("id 2");
        result!.HashedPassword.Should().Be("hashed password 2");
    }

    [Fact]
    public async Task GetDeviceAccountById_ValidId_ReturnsDeviceAccount()
    {
        DbDeviceAccount? result = await apiRepository.GetDeviceAccountById("id");

        result.Should().BeEquivalentTo(account);
    }

    [Fact]
    public async Task GetDeviceAccountById_InvalidId_ReturnsNull()
    {
        DbDeviceAccount? result = await apiRepository.GetDeviceAccountById("wrong id");

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddNewPlayerInfo_CanGetAfterward()
    {
        await apiRepository.AddNewPlayerInfo("id 2");
        IQueryable<DbSavefileUserData> result = apiRepository.GetPlayerInfo("id 2");

        result.Count().Should().Be(1);
    }

    [Fact]
    public void GetPlayerInfo_ValidId_ReturnsInfo()
    {
        IQueryable<DbSavefileUserData> result = apiRepository.GetPlayerInfo("id");

        result.Count().Should().Be(1);
    }

    [Fact]
    public void GetPlayerInfo_InvalidId_ThrowsException()
    {
        apiRepository.Invoking(x => x.GetPlayerInfo("wrong id")).Should().Throw<InvalidOperationException>();
    }
}
