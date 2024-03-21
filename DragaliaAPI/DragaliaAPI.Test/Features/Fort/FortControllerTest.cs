using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test.Features.Fort;

public class FortControllerTest
{
    private readonly Mock<IFortService> mockFortService;
    private readonly Mock<IBonusService> mockBonusService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IDragonService> mockDragonService;

    private readonly FortController fortController;

    public FortControllerTest()
    {
        mockFortService = new(MockBehavior.Strict);
        mockBonusService = new(MockBehavior.Strict);
        mockUpdateDataService = new(MockBehavior.Strict);
        mockRewardService = new(MockBehavior.Strict);
        mockDragonService = new(MockBehavior.Strict);

        fortController = new(
            mockFortService.Object,
            mockBonusService.Object,
            mockUpdateDataService.Object,
            mockRewardService.Object,
            mockDragonService.Object
        );
    }

    [Fact]
    public async Task GetData_ReturnsData()
    {
        FortDetail detail =
            new()
            {
                CarpenterNum = 1,
                MaxCarpenterCount = 2,
                WorkingCarpenterNum = 3
            };
        List<BuildList> buildList = new() { new() { FortPlantDetailId = 4 } };
        FortBonusList bonusList = new() { AllBonus = new(2, 3) };

        mockFortService
            .Setup(x => x.GetRupieProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetDragonfruitProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetStaminaProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockUpdateDataService
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(new UpdateDataList());

        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        mockFortService.Setup(x => x.GetBuildList()).ReturnsAsync(buildList);

        mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        this.mockDragonService.Setup(x => x.GetFreeGiftCount()).ReturnsAsync(2);

        FortGetDataResponse data = (
            await fortController.GetData(default)
        ).GetData<FortGetDataResponse>()!;

        data.BuildList.Should().BeEquivalentTo(buildList);
        data.FortBonusList.Should().BeEquivalentTo(bonusList);
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.DragonContactFreeGiftCount.Should().Be(2);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
    }

    [Fact]
    public async Task AddCarpenter_AddsCarpenter()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortDetail detail = new() { WorkingCarpenterNum = 4 };

        mockFortService
            .Setup(x => x.AddCarpenter(PaymentTypes.Diamantium))
            .ReturnsAsync(new FortDetail());
        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortAddCarpenterResponse data = (
            await fortController.AddCarpenter(
                new FortAddCarpenterRequest() { PaymentType = PaymentTypes.Diamantium },
                default
            )
        ).GetData<FortAddCarpenterResponse>()!;

        data.Result.Should().Be(1);
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildAtOnce_CallsBuildAtOnce()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortBonusList bonusList = new() { AllBonus = new(2, 3) };
        FortDetail detail = new() { WorkingCarpenterNum = 4 };

        mockFortService
            .Setup(x => x.GetRupieProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetDragonfruitProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetStaminaProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        mockFortService
            .Setup(x => x.BuildAtOnce(PaymentTypes.HalidomHustleHammer, 8))
            .Returns(Task.CompletedTask);

        mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortBuildAtOnceResponse data = (
            await fortController.BuildAtOnce(
                new FortBuildAtOnceRequest()
                {
                    PaymentType = PaymentTypes.HalidomHustleHammer,
                    BuildId = 8
                },
                default
            )
        ).GetData<FortBuildAtOnceResponse>()!;

        data.Result.Should().Be(1);
        data.BuildId.Should().Be(8);
        data.FortBonusList.Should().BeEquivalentTo(bonusList);
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildCancel_CallsCancelBuild()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortDetail detail = new() { WorkingCarpenterNum = 4 };

        mockFortService
            .Setup(x => x.CancelBuild(1))
            .ReturnsAsync(new DbFortBuild() { ViewerId = 1, BuildId = 1 });
        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortBuildCancelResponse data = (
            await fortController.BuildCancel(new FortBuildCancelRequest() { BuildId = 1 }, default)
        ).GetData<FortBuildCancelResponse>()!;

        data.Result.Should().Be(1);
        data.BuildId.Should().Be(1);
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildEnd_CallsEndBuild()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortBonusList bonusList = new() { AllBonus = new(2, 3) };
        FortDetail detail = new() { WorkingCarpenterNum = 4 };

        mockFortService
            .Setup(x => x.GetRupieProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetDragonfruitProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetStaminaProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        mockFortService.Setup(x => x.EndBuild(8)).Returns(Task.CompletedTask);

        mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortBuildEndResponse data = (
            await fortController.BuildEnd(new FortBuildEndRequest() { BuildId = 8 }, default)
        ).GetData<FortBuildEndResponse>()!;

        data.Result.Should().Be(1);
        data.BuildId.Should().Be(8);
        data.FortBonusList.Should().BeEquivalentTo(bonusList);
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildStart_CallsBuildStart()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortDetail detail = new() { WorkingCarpenterNum = 4 };
        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                BuildId = 10,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow,
            };

        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        mockFortService
            .Setup(x => x.BuildStart(FortPlants.BroadleafTree, 2, 3))
            .ReturnsAsync(build);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortBuildStartResponse data = (
            await fortController.BuildStart(
                new FortBuildStartRequest()
                {
                    FortPlantId = FortPlants.BroadleafTree,
                    PositionX = 2,
                    PositionZ = 3
                },
                default
            )
        ).GetData<FortBuildStartResponse>()!;

        data.Result.Should().Be(1);
        data.BuildId.Should().Be(build.BuildId);
        data.BuildStartDate.Should().Be(build.BuildStartDate);
        data.BuildEndDate.Should().Be(build.BuildEndDate);
        data.RemainTime.Should().Be(build.RemainTime);
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupAtOnce_CallsLevelupAtOnce()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortBonusList bonusList = new() { AllBonus = new(2, 3) };
        FortDetail detail = new() { WorkingCarpenterNum = 4 };

        mockFortService
            .Setup(x => x.GetRupieProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetDragonfruitProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetStaminaProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        mockFortService
            .Setup(x => x.LevelupAtOnce(PaymentTypes.HalidomHustleHammer, 8))
            .Returns(Task.CompletedTask);
        mockFortService.Setup(x => x.GetCoreLevels()).ReturnsAsync((3, 2));

        mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortLevelupAtOnceResponse data = (
            await fortController.LevelupAtOnce(
                new FortLevelupAtOnceRequest()
                {
                    PaymentType = PaymentTypes.HalidomHustleHammer,
                    BuildId = 8
                },
                default
            )
        ).GetData<FortLevelupAtOnceResponse>()!;

        data.Result.Should().Be(1);
        data.BuildId.Should().Be(8);
        data.CurrentFortLevel.Should().Be(3);
        data.CurrentFortCraftLevel.Should().Be(2);
        data.FortBonusList.Should().BeEquivalentTo(bonusList);
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupCancel_CallsCancelLevelup()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortDetail detail = new() { WorkingCarpenterNum = 4 };

        mockFortService
            .Setup(x => x.CancelLevelup(1))
            .ReturnsAsync(new DbFortBuild() { ViewerId = 1, BuildId = 1 });
        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortLevelupCancelResponse data = (
            await fortController.LevelupCancel(
                new FortLevelupCancelRequest() { BuildId = 1 },
                default
            )
        ).GetData<FortLevelupCancelResponse>()!;

        data.Result.Should().Be(1);
        data.BuildId.Should().Be(1);
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupEnd_CallsEndLevelup()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortBonusList bonusList = new() { AllBonus = new(2, 3) };
        FortDetail detail = new() { WorkingCarpenterNum = 4 };

        mockFortService
            .Setup(x => x.GetRupieProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetDragonfruitProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetStaminaProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        mockFortService.Setup(x => x.EndLevelup(8)).Returns(Task.CompletedTask);
        mockFortService.Setup(x => x.GetCoreLevels()).ReturnsAsync((3, 2));

        mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortLevelupEndResponse data = (
            await fortController.LevelupEnd(new FortLevelupEndRequest() { BuildId = 8 }, default)
        ).GetData<FortLevelupEndResponse>()!;

        data.Result.Should().Be(1);
        data.BuildId.Should().Be(8);
        data.CurrentFortLevel.Should().Be(3);
        data.CurrentFortCraftLevel.Should().Be(2);
        data.FortBonusList.Should().BeEquivalentTo(bonusList);
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupStart_CallsBuildStart()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortDetail detail = new() { WorkingCarpenterNum = 4 };
        DbFortBuild build =
            new()
            {
                ViewerId = 1,
                BuildId = 10,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromHours(2),
            };

        mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());

        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        mockFortService.Setup(x => x.LevelupStart(1)).ReturnsAsync(build);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortLevelupStartResponse data = (
            await fortController.LevelupStart(
                new FortLevelupStartRequest() { BuildId = 1 },
                default
            )
        ).GetData<FortLevelupStartResponse>()!;

        data.Result.Should().Be(1);
        data.BuildId.Should().Be(build.BuildId);
        data.LevelupStartDate.Should().Be(build.BuildStartDate);
        data.LevelupEndDate.Should().Be(build.BuildEndDate);
        data.RemainTime.Should().BeCloseTo(build.RemainTime, TimeSpan.FromSeconds(1));
        data.FortDetail.Should().BeEquivalentTo(detail);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task Move_CallsMove()
    {
        UpdateDataList updateDataList = new() { BuildList = new List<BuildList>() };
        FortBonusList bonusList = new() { AllBonus = new(2, 3) };

        mockFortService
            .Setup(x => x.GetRupieProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetDragonfruitProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetStaminaProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.Move(1, 2, 3))
            .ReturnsAsync(new DbFortBuild() { ViewerId = 1, BuildId = 1 });

        mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(updateDataList);

        FortMoveResponse data = (
            await fortController.Move(
                new FortMoveRequest()
                {
                    BuildId = 1,
                    AfterPositionX = 2,
                    AfterPositionZ = 3
                },
                default
            )
        ).GetData<FortMoveResponse>()!;

        data.Result.Should().Be(1);
        data.BuildId.Should().Be(1);
        data.FortBonusList.Should().BeEquivalentTo(bonusList);
        data.UpdateDataList.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }
}
