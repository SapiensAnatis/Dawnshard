using System.Collections;
using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test.Unit.Controllers;

public class FortControllerTest
{
    private readonly Mock<IFortService> mockFortService;
    private readonly Mock<IBonusService> mockBonusService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;

    private readonly FortController fortController;

    public FortControllerTest()
    {
        this.mockFortService = new(MockBehavior.Strict);
        this.mockBonusService = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);

        this.fortController = new(
            this.mockFortService.Object,
            this.mockBonusService.Object,
            this.mockUpdateDataService.Object
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

        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        this.mockFortService.Setup(x => x.GetBuildList()).ReturnsAsync(buildList);

        this.mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        FortGetDataData data = (await this.fortController.GetData()).GetData<FortGetDataData>()!;

        data.build_list.Should().BeEquivalentTo(buildList);
        data.fort_bonus_list.Should().BeEquivalentTo(bonusList);
        data.fort_detail.Should().BeEquivalentTo(detail);

        this.mockFortService.VerifyAll();
        this.mockBonusService.VerifyAll();
    }

    [Fact]
    public async Task AddCarpenter_AddsCarpenter()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };

        this.mockFortService
            .Setup(x => x.AddCarpenter(PaymentTypes.Diamantium))
            .ReturnsAsync(new FortDetail());
        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortAddCarpenterData data = (
            await this.fortController.AddCarpenter(
                new FortAddCarpenterRequest() { payment_type = PaymentTypes.Diamantium }
            )
        ).GetData<FortAddCarpenterData>()!;

        data.result.Should().Be(1);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        this.mockFortService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildAtOnce_CallsCompleteAtOnce()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };
        FortDetail detail = new() { working_carpenter_num = 4 };

        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        this.mockFortService
            .Setup(x => x.CompleteAtOnce(PaymentTypes.HalidomHustleHammer, 8))
            .Returns(Task.CompletedTask);

        this.mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortBuildAtOnceData data = (
            await this.fortController.BuildAtOnce(
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

        this.mockFortService.VerifyAll();
        this.mockBonusService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildCancel_CallsCancelUpgrade()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };

        this.mockFortService
            .Setup(x => x.CancelUpgrade(1))
            .ReturnsAsync(new DbFortBuild() { DeviceAccountId = "id", BuildId = 1 });
        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortBuildCancelData data = (
            await this.fortController.BuildCancel(new FortBuildCancelRequest() { build_id = 1 })
        ).GetData<FortBuildCancelData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(1);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        this.mockFortService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildEnd_CallsEndUpgrade()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };
        FortDetail detail = new() { working_carpenter_num = 4 };

        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        this.mockFortService.Setup(x => x.EndUpgrade(8)).Returns(Task.CompletedTask);

        this.mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortBuildEndData data = (
            await this.fortController.BuildEnd(new FortBuildEndRequest() { build_id = 8 })
        ).GetData<FortBuildEndData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(8);
        data.fort_bonus_list.Should().BeEquivalentTo(bonusList);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        this.mockFortService.VerifyAll();
        this.mockBonusService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task BuildStart_CallsBuildStart()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "id",
                BuildId = 10,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow,
            };

        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        this.mockFortService
            .Setup(x => x.BuildStart(FortPlants.BroadleafTree, 1, 2, 3))
            .ReturnsAsync(build);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortBuildStartData data = (
            await this.fortController.BuildStart(
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

        this.mockFortService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupAtOnce_CallsCompleteAtOnce()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };
        FortDetail detail = new() { working_carpenter_num = 4 };

        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        this.mockFortService
            .Setup(x => x.CompleteAtOnce(PaymentTypes.HalidomHustleHammer, 8))
            .Returns(Task.CompletedTask);
        this.mockFortService
            .Setup(x => x.GetBuildList())
            .ReturnsAsync(
                new List<BuildList>()
                {
                    new() { plant_id = FortPlants.Smithy, level = 2, },
                    new() { plant_id = FortPlants.TheHalidom, level = 3, }
                }
            );

        this.mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortLevelupAtOnceData data = (
            await this.fortController.LevelupAtOnce(
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

        this.mockFortService.VerifyAll();
        this.mockBonusService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupCancel_CallsCancelUpgrade()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };

        this.mockFortService
            .Setup(x => x.CancelUpgrade(1))
            .ReturnsAsync(new DbFortBuild() { DeviceAccountId = "id", BuildId = 1 });
        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortLevelupCancelData data = (
            await this.fortController.LevelupCancel(new FortLevelupCancelRequest() { build_id = 1 })
        ).GetData<FortLevelupCancelData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(1);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        this.mockFortService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupEnd_CallsEndUpgrade()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };
        FortDetail detail = new() { working_carpenter_num = 4 };

        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        this.mockFortService.Setup(x => x.EndUpgrade(8)).Returns(Task.CompletedTask);
        this.mockFortService
            .Setup(x => x.GetBuildList())
            .ReturnsAsync(
                new List<BuildList>()
                {
                    new() { plant_id = FortPlants.Smithy, level = 2, },
                    new() { plant_id = FortPlants.TheHalidom, level = 3, }
                }
            );

        this.mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortLevelupEndData data = (
            await this.fortController.LevelupEnd(new FortLevelupEndRequest() { build_id = 8 })
        ).GetData<FortLevelupEndData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(8);
        data.current_fort_level.Should().Be(3);
        data.current_fort_craft_level.Should().Be(2);
        data.fort_bonus_list.Should().BeEquivalentTo(bonusList);
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        this.mockFortService.VerifyAll();
        this.mockBonusService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task LevelupStart_CallsBuildStart()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortDetail detail = new() { working_carpenter_num = 4 };
        DbFortBuild build =
            new()
            {
                DeviceAccountId = "id",
                BuildId = 10,
                BuildStartDate = DateTimeOffset.UtcNow,
                BuildEndDate = DateTimeOffset.UtcNow + TimeSpan.FromHours(2),
            };

        this.mockFortService.Setup(x => x.GetFortDetail()).ReturnsAsync(detail);
        this.mockFortService.Setup(x => x.LevelupStart(1)).ReturnsAsync(build);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortLevelupStartData data = (
            await this.fortController.LevelupStart(new FortLevelupStartRequest() { build_id = 1 })
        ).GetData<FortLevelupStartData>()!;

        data.result.Should().Be(1);
        data.build_id.Should().Be(build.BuildId);
        data.levelup_start_date.Should().Be(build.BuildStartDate);
        data.levelup_end_date.Should().Be(build.BuildEndDate);
        data.remain_time.Should().BeCloseTo(build.RemainTime, TimeSpan.FromSeconds(1));
        data.fort_detail.Should().BeEquivalentTo(detail);
        data.update_data_list.Should().BeEquivalentTo(updateDataList);

        this.mockFortService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task Move_CallsMove()
    {
        UpdateDataList updateDataList = new() { build_list = new List<BuildList>() };
        FortBonusList bonusList = new() { all_bonus = new(2, 3) };

        this.mockFortService
            .Setup(x => x.Move(1, 2, 3))
            .ReturnsAsync(new DbFortBuild() { DeviceAccountId = "id", BuildId = 1 });

        this.mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(bonusList);

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync()).ReturnsAsync(updateDataList);

        FortMoveData data = (
            await this.fortController.Move(
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

        this.mockFortService.VerifyAll();
        this.mockUpdateDataService.VerifyAll();
    }
}
