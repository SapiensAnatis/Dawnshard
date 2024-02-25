using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Fort;

public class FortTest : TestFixture
{
    public FortTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
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
                ViewerId = ViewerId,
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

        (await this.Client.PostMsgpack<FortGetDataData>("/fort/get_data", new FortGetDataRequest()))
            .data.BuildList.Should()
            .ContainEquivalentOf(
                new BuildList()
                {
                    PlantId = FortPlants.AxeDojo,
                    Level = 10,
                    PositionX = 10,
                    PositionZ = 10,
                    BuildStartDate = start,
                    BuildEndDate = end,
                    FortPlantDetailId = 10050411,
                    BuildStatus = FortBuildStatus.LevelUp,
                    IsNew = false
                },
                opts =>
                    opts.Excluding(x => x.BuildId)
                        .Excluding(x => x.LastIncomeTime)
                        .Excluding(x => x.RemainTime)
            );

        // Not much point asserting against the other properties since they're stubs
    }

    [Fact]
    public async Task GetData_ReportsFreeDragonGiftCount()
    {
        await this
            .ApiContext.PlayerDragonGifts.Where(x =>
                x.ViewerId == this.ViewerId && x.DragonGiftId == DragonGifts.FreshBread
            )
            .ExecuteUpdateAsync(e => e.SetProperty(p => p.Quantity, 1));

        (await this.Client.PostMsgpack<FortGetDataData>("/fort/get_data", new FortGetDataRequest()))
            .data.DragonContactFreeGiftCount.Should()
            .Be(1);

        await this
            .ApiContext.PlayerDragonGifts.Where(x =>
                x.ViewerId == this.ViewerId && x.DragonGiftId == DragonGifts.FreshBread
            )
            .ExecuteUpdateAsync(e => e.SetProperty(p => p.Quantity, 0));

        (await this.Client.PostMsgpack<FortGetDataData>("/fort/get_data", new FortGetDataRequest()))
            .data.DragonContactFreeGiftCount.Should()
            .Be(0);
    }

    [Fact]
    public async Task AddCarpenter_ReturnsValidResult()
    {
        DbPlayerUserData oldUserData = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId);

        FortAddCarpenterData response = (
            await this.Client.PostMsgpack<FortAddCarpenterData>(
                "/fort/add_carpenter",
                new FortAddCarpenterRequest(PaymentTypes.Wyrmite)
            )
        ).data;

        response.FortDetail.CarpenterNum.Should().Be(3);
        response.UpdateDataList.UserData.Crystal.Should().Be(oldUserData.Crystal - 250);
    }

    [Fact]
    public async Task BuildAtOnce_ReturnsValidResult()
    {
        DbFortBuild build = this
            .ApiContext.PlayerFortBuilds.Add(
                new()
                {
                    ViewerId = ViewerId,
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

        BuildList result = response.UpdateDataList.BuildList.First(x =>
            x.BuildId == (ulong)build.BuildId
        );
        // The level changes when building starts, not when it ends, so no need to check it here
        result.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        result.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task BuildCancel_ReturnsValidResult()
    {
        DbFortBuild build = this
            .ApiContext.PlayerFortBuilds.Add(
                new()
                {
                    ViewerId = ViewerId,
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

        await this.Client.PostMsgpack<FortBuildCancelData>(
            "/fort/build_cancel",
            new FortBuildCancelRequest(build.BuildId)
        );

        this.ApiContext.PlayerFortBuilds.AsNoTracking()
            .Should()
            .NotContain(x => x.BuildId == build.BuildId);
    }

    [Fact]
    public async Task BuildEnd_ReturnsValidResult()
    {
        DbFortBuild build =
            new()
            {
                ViewerId = ViewerId,
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

        BuildList result = response.UpdateDataList.BuildList.First(x =>
            x.BuildId == (ulong)build.BuildId
        );
        result.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        result.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);
        result.Level.Should().Be(1);
    }

    [Fact]
    public async Task BuildStart_ReturnsValidResult()
    {
        int expectedPositionX = 2;
        int expectedPositionZ = 2;

        FortBuildStartData response = (
            await this.Client.PostMsgpack<FortBuildStartData>(
                "/fort/build_start",
                new FortBuildStartRequest(
                    FortPlants.FlameAltar,
                    expectedPositionX,
                    expectedPositionZ
                )
            )
        ).data;

        BuildList result = response.UpdateDataList.BuildList.First();
        result.PositionX.Should().Be(expectedPositionX);
        result.PositionZ.Should().Be(expectedPositionZ);
        result.BuildStartDate.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.BuildEndDate.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.BuildEndDate.Should().BeAfter(result.BuildStartDate);
        response.FortDetail.WorkingCarpenterNum.Should().BeGreaterThan(0); // Requiring '1' can conflict with other building tests
    }

    [Fact]
    public async Task LevelupAtOnce_ReturnsValidResult()
    {
        DbFortBuild build = this
            .ApiContext.PlayerFortBuilds.Add(
                new()
                {
                    ViewerId = ViewerId,
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

        BuildList result = response.UpdateDataList.BuildList.First(x =>
            x.BuildId == (ulong)build.BuildId
        );
        result.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        result.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);
        result.Level.Should().Be(3);
    }

    [Fact]
    public async Task LevelupAtOnce_Halidom_ReturnsNewFortLevel()
    {
        DbFortBuild halidom = await this
            .ApiContext.PlayerFortBuilds.AsTracking()
            .SingleAsync(x => x.PlantId == FortPlants.TheHalidom);

        halidom.BuildStartDate = DateTimeOffset.FromUnixTimeSeconds(1287924543);
        halidom.BuildEndDate = DateTimeOffset.FromUnixTimeSeconds(1388924543);
        halidom.Level = 10;

        await this.ApiContext.SaveChangesAsync();

        FortLevelupAtOnceData response = (
            await this.Client.PostMsgpack<FortLevelupAtOnceData>(
                "/fort/levelup_at_once",
                new FortLevelupAtOnceRequest(halidom.BuildId, PaymentTypes.Wyrmite)
            )
        ).data;

        response.CurrentFortLevel.Should().Be(11);
    }

    [Fact]
    public async Task LevelupAtOnce_Smithy_ReturnsNewFortCraftLevel()
    {
        DbFortBuild smithy = await this
            .ApiContext.PlayerFortBuilds.AsTracking()
            .SingleAsync(x => x.PlantId == FortPlants.Smithy);

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

        response.CurrentFortCraftLevel.Should().Be(2);
    }

    [Fact]
    public async Task LevelUpCancel_ReturnsValidResult()
    {
        DbFortBuild build = this
            .ApiContext.PlayerFortBuilds.Add(
                new()
                {
                    ViewerId = ViewerId,
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

        BuildList result = response.UpdateDataList.BuildList.First(x =>
            x.BuildId == (ulong)build.BuildId
        );
        result.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        result.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);
        result.Level.Should().Be(1); // Level should have decreased
    }

    [Fact]
    public async Task LevelUpEnd_ReturnsValidResult()
    {
        DbFortBuild build = this
            .ApiContext.PlayerFortBuilds.Add(
                new()
                {
                    ViewerId = ViewerId,
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

        BuildList result = response.UpdateDataList.BuildList.First(x =>
            x.BuildId == (ulong)build.BuildId
        );
        result.BuildStartDate.Should().Be(DateTimeOffset.UnixEpoch);
        result.BuildEndDate.Should().Be(DateTimeOffset.UnixEpoch);
    }

    [Fact]
    public async Task LevelupEnd_Smithy_ReturnsNewFortCraftLevel()
    {
        DbFortBuild smithy = await this
            .ApiContext.PlayerFortBuilds.AsTracking()
            .SingleAsync(x => x.PlantId == FortPlants.Smithy);

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

        response.CurrentFortCraftLevel.Should().Be(2);
    }

    [Fact]
    public async Task LevelupEnd_Halidom_ReturnsNewFortLevel()
    {
        DbFortBuild halidom = await this
            .ApiContext.PlayerFortBuilds.AsTracking()
            .SingleAsync(x => x.PlantId == FortPlants.TheHalidom);

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

        response.CurrentFortLevel.Should().Be(11);
    }

    [Fact]
    public async Task LevelUpStart_ReturnsValidResult()
    {
        DbFortBuild build = this
            .ApiContext.PlayerFortBuilds.Add(
                new()
                {
                    ViewerId = ViewerId,
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

        BuildList result = response.UpdateDataList.BuildList.First(x =>
            x.BuildId == (ulong)build.BuildId
        );
        result.BuildStartDate.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.BuildEndDate.Should().NotBe(DateTimeOffset.UnixEpoch);
        result.BuildEndDate.Should().BeAfter(result.BuildStartDate);
        result.Level.Should().Be(1);
    }

    [Fact]
    public async Task Move_ReturnsValidResult()
    {
        DbFortBuild build = this
            .ApiContext.PlayerFortBuilds.Add(
                new()
                {
                    ViewerId = ViewerId,
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

        int expectedPositionX = 4;
        int expectedPositionZ = 4;
        FortMoveData response = (
            await this.Client.PostMsgpack<FortMoveData>(
                "/fort/move",
                new FortMoveRequest(build.BuildId, expectedPositionX, expectedPositionZ)
            )
        ).data;

        BuildList result = response.UpdateDataList.BuildList.First(x =>
            x.BuildId == (ulong)build.BuildId
        );
        result.PositionX.Should().Be(expectedPositionX);
        result.PositionZ.Should().Be(expectedPositionZ);
    }

    [Fact]
    public async Task GetMultiIncome_ReturnsExpectedResponse()
    {
        DateTimeOffset lastIncome = DateTimeOffset.UtcNow - TimeSpan.FromHours(6);
        long oldCoin = this
            .ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == ViewerId)
            .Coin;

        DbFortBuild rupieMine =
            new()
            {
                ViewerId = ViewerId,
                PlantId = FortPlants.RupieMine,
                LastIncomeDate = lastIncome,
                Level = 10
            };
        DbFortBuild dragonTree =
            new()
            {
                ViewerId = ViewerId,
                PlantId = FortPlants.Dragontree,
                LastIncomeDate = lastIncome,
                Level = 13
            };

        this.ApiContext.PlayerFortBuilds.Add(rupieMine);
        this.ApiContext.PlayerFortBuilds.Add(dragonTree);

        DbFortBuild halidom = this
            .ApiContext.PlayerFortBuilds.AsTracking()
            .First(x => x.PlantId == FortPlants.TheHalidom && x.ViewerId == ViewerId);
        halidom.LastIncomeDate = lastIncome;

        await this.ApiContext.SaveChangesAsync();

        DragaliaResponse<FortGetMultiIncomeData> response =
            await this.Client.PostMsgpack<FortGetMultiIncomeData>(
                "/fort/get_multi_income",
                new FortGetMultiIncomeRequest()
                {
                    BuildIdList = new[] { rupieMine.BuildId, dragonTree.BuildId, halidom.BuildId }
                }
            );

        response.data.AddCoinList.Should().NotBeEmpty();
        AtgenAddCoinList coinList = response.data.AddCoinList.First();
        coinList.BuildId.Should().Be(rupieMine.BuildId);
        coinList.AddCoin.Should().BeCloseTo(3098, 10);

        response.data.HarvestBuildList.Should().NotBeEmpty();
        AtgenHarvestBuildList harvestList = response.data.HarvestBuildList.First();
        harvestList.BuildId.Should().Be(dragonTree.BuildId);
        harvestList.AddHarvestList.Should().NotBeEmpty();

        response.data.AddStaminaList.Should().NotBeEmpty();
        AtgenAddStaminaList staminaList = response.data.AddStaminaList.First();
        staminaList.BuildId.Should().Be(halidom.BuildId);
        staminaList.AddStamina.Should().Be(12);

        response.data.UpdateDataList.UserData.Coin.Should().BeCloseTo(oldCoin + 3098, 10);
        response.data.UpdateDataList.MaterialList.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetMultiIncome_CompletesMission()
    {
        DbFortBuild rupieMine = await this.AddToDatabase(
            new DbFortBuild()
            {
                ViewerId = ViewerId,
                PlantId = FortPlants.RupieMine,
                LastIncomeDate = DateTimeOffset.UnixEpoch,
                Level = 10
            }
        );

        await this.AddToDatabase(
            new DbPlayerMission()
            {
                Id = 15070201, // Collect Rupies from a Facility
                ViewerId = ViewerId,
                State = MissionState.InProgress,
                Type = MissionType.Daily,
                Progress = 0,
            }
        );

        DragaliaResponse<FortGetMultiIncomeData> response =
            await this.Client.PostMsgpack<FortGetMultiIncomeData>(
                "/fort/get_multi_income",
                new FortGetMultiIncomeRequest() { BuildIdList = new[] { rupieMine.BuildId } }
            );

        response
            .data.UpdateDataList.MissionNotice.DailyMissionNotice.NewCompleteMissionIdList.Should()
            .Contain(15070201);
    }
}
