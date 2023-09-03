using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Fort;

public class FortTest : TestFixture
{
    public FortTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();

        this.ApiContext.PlayerFortBuilds
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdate(x => x.SetProperty(y => y.BuildStartDate, DateTimeOffset.UnixEpoch));

        this.ApiContext.PlayerFortBuilds
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .ExecuteUpdate(x => x.SetProperty(y => y.BuildEndDate, DateTimeOffset.UnixEpoch));

        this.ApiContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task GetData_ReturnsBuildingData()
    {
        // 2012-06-16 12:12:59
        DateTimeOffset start = DateTimeOffset.FromUnixTimeSeconds(1339848779);
        // 2029-11-09 13:15:43
        DateTimeOffset end = DateTimeOffset.FromUnixTimeSeconds(1888924543);
        // 2013-01-14 07:47:23
        DateTimeOffset income = DateTimeOffset.FromUnixTimeSeconds(1358149643);

        this.ApiContext.PlayerFortBuilds.Add(
            new DbFortBuild()
            {
                DeviceAccountId = DeviceAccountId,
                PlantId = FortPlants.AxeDojo,
                Level = 10,
                PositionX = 10,
                PositionZ = 10,
                BuildStartDate = start,
                BuildEndDate = end,
                IsNew = false,
                LastIncomeDate = income, // Axe dojos don't make you money but let's pretend they do
            }
        );
        await this.ApiContext.SaveChangesAsync();

        (
            await this.Client.PostMsgpack<FortGetDataData>(
                "/fort/get_data",
                new FortGetDataRequest()
            )
        ).data.build_list
            .Should()
            .ContainEquivalentOf(
                new BuildList()
                {
                    plant_id = FortPlants.AxeDojo,
                    level = 10,
                    position_x = 10,
                    position_z = 10,
                    build_start_date = start,
                    build_end_date = end,
                    fort_plant_detail_id = 10050411,
                    build_status = FortBuildStatus.LevelUp,
                    is_new = false
                },
                opts =>
                    opts.Excluding(x => x.build_id)
                        .Excluding(x => x.last_income_time)
                        .Excluding(x => x.remain_time)
            );

        // Not much point asserting against the other properties since they're stubs
    }

    [Fact]
    public async Task AddCarpenter_ReturnsValidResult()
    {
        DbPlayerUserData oldUserData = this.ApiContext.PlayerUserData
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId);

        FortAddCarpenterData response = (
            await this.Client.PostMsgpack<FortAddCarpenterData>(
                "/fort/add_carpenter",
                new FortAddCarpenterRequest(PaymentTypes.Wyrmite)
            )
        ).data;

        response.fort_detail.carpenter_num.Should().Be(3);
        response.update_data_list.user_data.crystal.Should().Be(oldUserData.Crystal - 250);
    }

    [Fact]
    public async Task BuildAtOnce_ReturnsValidResult()
    {
        DbFortBuild build = this.ApiContext.PlayerFortBuilds
            .Add(
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    PlantId = FortPlants.StaffDojo,
                    Level = 0,
                    PositionX = 2,
                    PositionZ = 2,
                    BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                    BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                }
            )
            .Entity;
        await this.ApiContext.SaveChangesAsync();

        FortBuildAtOnceData response = (
            await this.Client.PostMsgpack<FortBuildAtOnceData>(
                "/fort/build_at_once",
                new FortBuildAtOnceRequest(build.BuildId, PaymentTypes.Wyrmite)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        // The level changes when building starts, not when it ends, so no need to check it here
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task BuildCancel_ReturnsValidResult()
    {
        DbFortBuild build = this.ApiContext.PlayerFortBuilds
            .Add(
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    PlantId = FortPlants.StaffDojo,
                    Level = 0,
                    PositionX = 2,
                    PositionZ = 2,
                    BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                    BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                }
            )
            .Entity;
        await this.ApiContext.SaveChangesAsync();

        FortBuildCancelData response = (
            await this.Client.PostMsgpack<FortBuildCancelData>(
                "/fort/build_cancel",
                new FortBuildCancelRequest(build.BuildId)
            )
        ).data;

        // this removes it from the player
    }

    [Fact]
    public async Task BuildEnd_ReturnsValidResult()
    {
        DbFortBuild build =
            new()
            {
                DeviceAccountId = DeviceAccountId,
                PlantId = FortPlants.StaffDojo,
                Level = 0,
                PositionX = 2,
                PositionZ = 2,
                BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1682110410),
                BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1682110411),
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            };

        await this.AddToDatabase(build);

        FortBuildEndData response = (
            await this.Client.PostMsgpack<FortBuildEndData>(
                "/fort/build_end",
                new FortBuildEndRequest(build.BuildId)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.level.Should().Be(1);
    }

    [Fact]
    public async Task BuildStart_ReturnsValidResult()
    {
        int ExpectedPositionX = 2;
        int ExpectedPositionZ = 2;

        FortBuildStartData response = (
            await this.Client.PostMsgpack<FortBuildStartData>(
                "/fort/build_start",
                new FortBuildStartRequest(
                    FortPlants.FlameAltar,
                    ExpectedPositionX,
                    ExpectedPositionZ
                )
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First();
        result.position_x.Should().Be(ExpectedPositionX);
        result.position_z.Should().Be(ExpectedPositionZ);
        result.build_start_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().BeAfter(result.build_start_date);
        response.fort_detail.working_carpenter_num.Should().BeGreaterThan(0); // Requiring '1' can conflict with other building tests
    }

    [Fact]
    public async Task LevelupAtOnce_ReturnsValidResult()
    {
        DbFortBuild build = this.ApiContext.PlayerFortBuilds
            .Add(
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    PlantId = FortPlants.StaffDojo,
                    Level = 2,
                    PositionX = 2,
                    PositionZ = 2,
                    BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                    BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                }
            )
            .Entity;
        await this.ApiContext.SaveChangesAsync();

        FortLevelupAtOnceData response = (
            await this.Client.PostMsgpack<FortLevelupAtOnceData>(
                "/fort/levelup_at_once",
                new FortLevelupAtOnceRequest(build.BuildId, PaymentTypes.Wyrmite)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.level.Should().Be(3);
    }

    [Fact]
    public async Task LevelupAtOnce_Halidom_ReturnsNewFortLevel()
    {
        DbFortBuild halidom = await this.ApiContext.PlayerFortBuilds.SingleAsync(
            x => x.PlantId == FortPlants.TheHalidom
        );

        halidom.BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1287924543);
        halidom.BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1388924543);
        halidom.Level = 10;

        int rows = await this.ApiContext.SaveChangesAsync();

        FortLevelupAtOnceData response = (
            await this.Client.PostMsgpack<FortLevelupAtOnceData>(
                "/fort/levelup_at_once",
                new FortLevelupAtOnceRequest(halidom.BuildId, PaymentTypes.Wyrmite)
            )
        ).data;

        response.current_fort_level.Should().Be(11);
    }

    [Fact]
    public async Task LevelupAtOnce_Smithy_ReturnsNewFortCraftLevel()
    {
        DbFortBuild smithy = await this.ApiContext.PlayerFortBuilds.SingleAsync(
            x => x.PlantId == FortPlants.Smithy
        );

        smithy.BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1287924543);
        smithy.BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1388924543);
        smithy.Level = 1;

        await this.ApiContext.SaveChangesAsync();

        FortLevelupAtOnceData response = (
            await this.Client.PostMsgpack<FortLevelupAtOnceData>(
                "/fort/levelup_at_once",
                new FortLevelupAtOnceRequest(smithy.BuildId, PaymentTypes.Wyrmite)
            )
        ).data;

        response.current_fort_craft_level.Should().Be(2);
    }

    [Fact]
    public async Task LevelUpCancel_ReturnsValidResult()
    {
        DbFortBuild build = this.ApiContext.PlayerFortBuilds
            .Add(
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    PlantId = FortPlants.StaffDojo,
                    Level = 1,
                    PositionX = 2,
                    PositionZ = 2,
                    BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                    BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                }
            )
            .Entity;
        await this.ApiContext.SaveChangesAsync();

        FortLevelupCancelData response = (
            await this.Client.PostMsgpack<FortLevelupCancelData>(
                "/fort/levelup_cancel",
                new FortLevelupCancelRequest(build.BuildId)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.level.Should().Be(1); // Level should have decreased
    }

    [Fact]
    public async Task LevelUpEnd_ReturnsValidResult()
    {
        DbFortBuild build = this.ApiContext.PlayerFortBuilds
            .Add(
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    PlantId = FortPlants.StaffDojo,
                    Level = 1,
                    PositionX = 2,
                    PositionZ = 2,
                    BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1287924543),
                    BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1388924543),
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                }
            )
            .Entity;

        await this.ApiContext.SaveChangesAsync();

        FortLevelupEndData response = (
            await this.Client.PostMsgpack<FortLevelupEndData>(
                "/fort/levelup_end",
                new FortLevelupEndRequest(build.BuildId)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().Be(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task LevelupEnd_Smithy_ReturnsNewFortCraftLevel()
    {
        DbFortBuild smithy = await this.ApiContext.PlayerFortBuilds.SingleAsync(
            x => x.PlantId == FortPlants.Smithy
        );

        smithy.BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1287924543);
        smithy.BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1388924543);
        smithy.Level = 1;

        await this.ApiContext.SaveChangesAsync();

        FortLevelupEndData response = (
            await this.Client.PostMsgpack<FortLevelupEndData>(
                "/fort/levelup_end",
                new FortLevelupEndRequest(smithy.BuildId)
            )
        ).data;

        response.current_fort_craft_level.Should().Be(2);
    }

    [Fact]
    public async Task LevelupEnd_Halidom_ReturnsNewFortLevel()
    {
        DbFortBuild halidom = await this.ApiContext.PlayerFortBuilds.SingleAsync(
            x => x.PlantId == FortPlants.TheHalidom
        );

        halidom.BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1287924543);
        halidom.BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1388924543);
        halidom.Level = 10;

        await this.ApiContext.SaveChangesAsync();

        FortLevelupEndData response = (
            await this.Client.PostMsgpack<FortLevelupEndData>(
                "/fort/levelup_end",
                new FortLevelupEndRequest(halidom.BuildId)
            )
        ).data;

        response.current_fort_level.Should().Be(11);
    }

    [Fact]
    public async Task LevelUpStart_ReturnsValidResult()
    {
        DbFortBuild build = this.ApiContext.PlayerFortBuilds
            .Add(
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    PlantId = FortPlants.StaffDojo,
                    Level = 1,
                    PositionX = 2,
                    PositionZ = 2,
                    BuildStartDate = DateTimeOffset.UnixEpoch,
                    BuildEndDate = DateTimeOffset.UnixEpoch,
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                }
            )
            .Entity;
        await this.ApiContext.SaveChangesAsync();

        FortLevelupStartData response = (
            await this.Client.PostMsgpack<FortLevelupStartData>(
                "/fort/levelup_start",
                new FortLevelupStartRequest(build.BuildId)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.build_start_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.build_end_date.Should().BeAfter(result.build_start_date);
        result.level.Should().Be(1);
    }

    [Fact]
    public async Task Move_ReturnsValidResult()
    {
        DbFortBuild build = this.ApiContext.PlayerFortBuilds
            .Add(
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    PlantId = FortPlants.StaffDojo,
                    Level = 1,
                    PositionX = 2,
                    PositionZ = 2,
                    BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1887924543),
                    BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1888924543),
                    IsNew = true,
                    LastIncomeDate = DateTimeOffset.UnixEpoch
                }
            )
            .Entity;
        await this.ApiContext.SaveChangesAsync();

        int ExpectedPositionX = 4;
        int ExpectedPositionZ = 4;
        FortMoveData response = (
            await this.Client.PostMsgpack<FortMoveData>(
                "/fort/move",
                new FortMoveRequest(build.BuildId, ExpectedPositionX, ExpectedPositionZ)
            )
        ).data;

        BuildList result = response.update_data_list.build_list.First(
            x => x.build_id == (ulong)build.BuildId
        );
        result.position_x.Should().Be(ExpectedPositionX);
        result.position_z.Should().Be(ExpectedPositionZ);
    }

    [Fact]
    public async Task GetMultiIncome_ReturnsExpectedResponse()
    {
        DateTimeOffset lastIncome = DateTimeOffset.UtcNow - TimeSpan.FromHours(6);
        long oldCoin = this.ApiContext.PlayerUserData
            .AsNoTracking()
            .First(x => x.DeviceAccountId == DeviceAccountId)
            .Coin;

        DbFortBuild rupieMine =
            new()
            {
                DeviceAccountId = DeviceAccountId,
                PlantId = FortPlants.RupieMine,
                LastIncomeDate = lastIncome,
                Level = 10
            };
        DbFortBuild dragonTree =
            new()
            {
                DeviceAccountId = DeviceAccountId,
                PlantId = FortPlants.Dragontree,
                LastIncomeDate = lastIncome,
                Level = 13
            };

        this.ApiContext.PlayerFortBuilds.Add(rupieMine);
        this.ApiContext.PlayerFortBuilds.Add(dragonTree);

        DbFortBuild halidom = this.ApiContext.PlayerFortBuilds.First(
            x => x.PlantId == FortPlants.TheHalidom && x.DeviceAccountId == DeviceAccountId
        );
        halidom.LastIncomeDate = lastIncome;

        await this.ApiContext.SaveChangesAsync();

        DragaliaResponse<FortGetMultiIncomeData> response =
            await this.Client.PostMsgpack<FortGetMultiIncomeData>(
                "/fort/get_multi_income",
                new FortGetMultiIncomeRequest()
                {
                    build_id_list = new[] { rupieMine.BuildId, dragonTree.BuildId, halidom.BuildId }
                }
            );

        response.data.add_coin_list.Should().NotBeEmpty();
        AtgenAddCoinList coinList = response.data.add_coin_list.First();
        coinList.build_id.Should().Be(rupieMine.BuildId);
        coinList.add_coin.Should().BeCloseTo(3098, 10);

        response.data.harvest_build_list.Should().NotBeEmpty();
        AtgenHarvestBuildList harvestList = response.data.harvest_build_list.First();
        harvestList.build_id.Should().Be(dragonTree.BuildId);
        harvestList.add_harvest_list.Should().NotBeEmpty();

        response.data.add_stamina_list.Should().NotBeEmpty();
        AtgenAddStaminaList staminaList = response.data.add_stamina_list.First();
        staminaList.build_id.Should().Be(halidom.BuildId);
        staminaList.add_stamina.Should().Be(12);

        response.data.update_data_list.user_data.coin.Should().BeCloseTo(oldCoin + 3098, 10);
        response.data.update_data_list.material_list.Should().NotBeEmpty();
    }
}
