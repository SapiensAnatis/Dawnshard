using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Models.Database;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;

namespace DragaliaAPI.Test.Unit.Services;

public class ApiRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly ApiRepository apiRepository;
    private readonly ApiContext apiContext;
    private readonly IUnitDataService unitDataService;
    private readonly IDragonDataService dragonDataService;

    private readonly DbDeviceAccount account = new("id", "hashed password");
    private readonly DbPlayerUserData playerInfo = DbSavefileUserDataFactory.Create("id");

    public ApiRepositoryTest(DbTestFixture data)
    {
        apiContext = data.apiContext;
        this.unitDataService = new UnitDataService();
        this.dragonDataService = new DragonDataService();

        apiRepository = new(apiContext, unitDataService, dragonDataService);
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
    public async Task AddNewPlayerInfo_CanGetAfterwards()
    {
        await apiRepository.AddNewPlayerInfo("id 2");
        IQueryable<DbPlayerUserData> result = apiRepository.GetPlayerInfo("id 2");

        result.Count().Should().Be(1);
    }

    [Fact]
    public void GetPlayerInfo_ValidId_ReturnsInfo()
    {
        apiRepository.GetPlayerInfo("id").Should().NotBeEmpty();
    }

    [Fact]
    public void GetPlayerInfo_InvalidId_ReturnsEmptyQueryable()
    {
        apiRepository.GetPlayerInfo("wrong id").Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateTutorialStatus_UpdatesTutorialStatus()
    {
        await apiRepository.UpdateTutorialStatus("id", 200);

        this.apiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == "id")
            .TutorialStatus.Should()
            .Be(200);
    }

    [Fact]
    public async Task UpdateName_UpdatesName()
    {
        await apiRepository.UpdateName("id", "Euden 2");

        this.apiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == "id")
            .Name.Should()
            .Be("Euden 2");
    }
}
