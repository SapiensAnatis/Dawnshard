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
                carpenter_num = 1,
                max_carpenter_count = 2,
                working_carpenter_num = 3
            };
        List<BuildList> buildList = new() { new() { fort_plant_detail_id = 4 } };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };

        mockFortService
            .Setup(x => x.GetRupieProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetDragonfruitProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockFortService
            .Setup(x => x.GetStaminaProduction())
            .ReturnsAsync(new AtgenProductionRp(0, 0));

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(new UpdateDataList());

        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        mockFortService.Setup(x => x.GetBuildList()).ReturnsAsync(buildList);

        mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        this.mockDragonService.Setup(x => x.GetFreeGiftCount()).ReturnsAsync(2);

        FortGetDataData data = (await fortController.GetData()).GetData<FortGetDataData>()!;

        data.build_list.Should().BeEquivalentTo(buildList);
        data.fort_bonus_list.Should().BeEquivalentTo(bonusList);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.dragon_contact_free_gift_count.Should().Be(2);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
    }

    [Fact]
    public async Task AddCarpenter_AddsCarpenter()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };

        mockFortService
            .Setup(x => x.AddCarpenter(PaymentTypes.Diamantium))
            .ReturnsAsync(new FortDetail());
        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortAddCarpenterData data = (
            await fortController.AddCarpenter(
                new FortAddCarpenterRequest() { payment_type = PaymentTypes.Diamantium }
            )
        ).GetData<FortAddCarpenterData>()!;

        data.result.Should().Be(1);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildAtOnce_CallsBuildAtOnce()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };
        FortDetail detail = new() { working_carpenter_num = 4 };

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

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortBuildAtOnceData data = (
            await fortController.BuildAtOnce(
                new FortBuildAtOnceRequest()
                {
                    payment_type = PaymentTypes.HalidomHustleHammer,
                    build_id = 8
                }
            )
        ).GetData<FortBuildAtOnceData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(8);
        data.fort_bonus_list.Should().BeEquivalentTo(bonusList);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildCancel_CallsCancelBuild()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };

        mockFortService
            .Setup(x => x.CancelBuild(1))
            .ReturnsAsync(new DbFortBuild() { ViewerId = 1, BuildId = 1 });
        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortBuildCancelData data = (
            await fortController.BuildCancel(new FortBuildCancelRequest() { build_id = 1 })
        ).GetData<FortBuildCancelData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(1);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildEnd_CallsEndBuild()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };
        FortDetail detail = new() { working_carpenter_num = 4 };

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

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortBuildEndData data = (
            await fortController.BuildEnd(new FortBuildEndRequest() { build_id = 8 })
        ).GetData<FortBuildEndData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(8);
        data.fort_bonus_list.Should().BeEquivalentTo(bonusList);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildStart_CallsBuildStart()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };
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

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortBuildStartData data = (
            await fortController.BuildStart(
                new FortBuildStartRequest()
                {
                    fort_plant_id = FortPlants.BroadleafTree,
                    position_x = 2,
                    position_z = 3
                }
            )
        ).GetData<FortBuildStartData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be((ulong)build.BuildId);
        data.build_start_date.Should().Be(build.BuildStartDate);
        data.build_end_date.Should().Be(build.BuildEndDate);
        data.remain_time.Should().Be(build.RemainTime);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupAtOnce_CallsLevelupAtOnce()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };
        FortDetail detail = new() { working_carpenter_num = 4 };

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

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortLevelupAtOnceData data = (
            await fortController.LevelupAtOnce(
                new FortLevelupAtOnceRequest()
                {
                    payment_type = PaymentTypes.HalidomHustleHammer,
                    build_id = 8
                }
            )
        ).GetData<FortLevelupAtOnceData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(8);
        data.current_fort_level.Should().Be(3);
        data.current_fort_craft_level.Should().Be(2);
        data.fort_bonus_list.Should().BeEquivalentTo(bonusList);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupCancel_CallsCancelLevelup()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };

        mockFortService
            .Setup(x => x.CancelLevelup(1))
            .ReturnsAsync(new DbFortBuild() { ViewerId = 1, BuildId = 1 });
        mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortLevelupCancelData data = (
            await fortController.LevelupCancel(new FortLevelupCancelRequest() { build_id = 1 })
        ).GetData<FortLevelupCancelData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(1);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupEnd_CallsEndLevelup()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };
        FortDetail detail = new() { working_carpenter_num = 4 };

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

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortLevelupEndData data = (
            await fortController.LevelupEnd(new FortLevelupEndRequest() { build_id = 8 })
        ).GetData<FortLevelupEndData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(8);
        data.current_fort_level.Should().Be(3);
        data.current_fort_craft_level.Should().Be(2);
        data.fort_bonus_list.Should().BeEquivalentTo(bonusList);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockBonusService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupStart_CallsBuildStart()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };
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

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortLevelupStartData data = (
            await fortController.LevelupStart(new FortLevelupStartRequest() { build_id = 1 })
        ).GetData<FortLevelupStartData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(build.BuildId);
        data.levelup_start_date.Should().Be(build.BuildStartDate);
        data.levelup_end_date.Should().Be(build.BuildEndDate);
        data.remain_time.Should().BeCloseTo(build.RemainTime, TimeSpan.FromSeconds(1));
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task Move_CallsMove()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };

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

        mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortMoveData data = (
            await fortController.Move(
                new FortMoveRequest()
                {
                    build_id = 1,
                    after_position_x = 2,
                    after_position_z = 3
                }
            )
        ).GetData<FortMoveData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(1);
        data.fort_bonus_list.Should().BeEquivalentTo(bonusList);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        mockFortService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }
}
