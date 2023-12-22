using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models.Missions;
using DragaliaAPI.Test.Utils;
using MockQueryable.Moq;

namespace DragaliaAPI.Test.Features.Missions;

public class MissionControllerTest
{
    private readonly MissionController missionController;
    private readonly Mock<IMissionService> mockMissionService;
    private readonly Mock<IMissionRepository> mockMissionRepository;
    private readonly Mock<IRewardService> mockRewardService;
    private readonly Mock<IUpdateDataService> mockUpdateDataService;

    public MissionControllerTest()
    {
        this.mockMissionService = new(MockBehavior.Strict);
        this.mockMissionRepository = new(MockBehavior.Strict);
        this.mockRewardService = new(MockBehavior.Strict);
        this.mockUpdateDataService = new(MockBehavior.Strict);

        this.missionController = new MissionController(
            this.mockMissionService.Object,
            this.mockMissionRepository.Object,
            this.mockRewardService.Object,
            this.mockUpdateDataService.Object
        );

        this.missionController.SetupMockContext();
    }

    [Fact]
    public async Task GetMissionList_ReturnsMissionList()
    {
        MissionNotice notice =
            new()
            {
                normal_mission_notice = new AtgenNormalMissionNotice()
                {
                    is_update = 1,
                    all_mission_count = 420
                }
            };

        CurrentMainStoryMission mainStoryMission =
            new(1337, new List<AtgenMainStoryMissionStateList>());

        this.mockMissionService.Setup(x => x.GetMissionNotice(null)).ReturnsAsync(notice);

        this.mockMissionService.Setup(x => x.GetCurrentMainStoryMission())
            .ReturnsAsync(mainStoryMission);
        this.mockMissionService.Setup(x => x.BuildNormalResponse<MissionGetMissionListData>())
            .ReturnsAsync(
                new MissionGetMissionListData()
                {
                    normal_mission_list = [],
                    mission_notice = notice,
                    current_main_story_mission = mainStoryMission
                }
            );

        DragaliaResult<MissionGetMissionListData> resp =
            await this.missionController.GetMissionList();

        MissionGetMissionListData? response = resp.Value;
        response.Should().NotBeNull();

        response!.mission_notice.Should().Be(notice);
        response.current_main_story_mission.Should().Be(mainStoryMission);
        response.normal_mission_list.Should().BeEmpty();

        mockMissionService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task GetDrillMissionList_ReturnsDrillMissionList()
    {
        MissionNotice notice =
            new()
            {
                normal_mission_notice = new AtgenNormalMissionNotice()
                {
                    is_update = 1,
                    all_mission_count = 420
                }
            };

        this.mockMissionService.Setup(x => x.GetMissionNotice(null)).ReturnsAsync(notice);
        this.mockMissionService.Setup(x => x.GetCompletedDrillGroups())
            .ReturnsAsync([new DrillMissionGroupList(1)]);

        this.mockMissionRepository.Setup(x => x.GetMissionsByType(MissionType.Drill))
            .Returns(
                new List<DbPlayerMission>()
                {
                    new()
                    {
                        ViewerId = IdentityTestUtils.ViewerId,
                        Type = MissionType.Drill,
                        Id = 500,
                        State = MissionState.InProgress,
                        Start = DateTimeOffset.UnixEpoch,
                        End = DateTimeOffset.UnixEpoch
                    }
                }
                    .AsQueryable()
                    .BuildMock()
            );

        DragaliaResult<MissionGetDrillMissionListData> resp =
            await this.missionController.GetDrillMissionList();

        MissionGetDrillMissionListData? response = resp.Value;
        response.Should().NotBeNull();

        response!.mission_notice.Should().Be(notice);
        response
            .drill_mission_list.Should()
            .ContainEquivalentOf(
                new DrillMissionList(500, 0, 0, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch)
            );
        response.drill_mission_group_list.Should().BeEquivalentTo([new DrillMissionGroupList(1)]);

        mockMissionService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockMissionRepository.VerifyAll();
    }

    [Fact]
    public async Task UnlockDrillMissionGroup_UnlocksGroup()
    {
        this.mockMissionService.Setup(x => x.UnlockDrillMissionGroup(100))
            .ReturnsAsync(
                new List<DbPlayerMission>()
                {
                    new()
                    {
                        ViewerId = IdentityTestUtils.ViewerId,
                        Id = 5000,
                        State = MissionState.Completed,
                        Type = MissionType.Drill,
                        Start = DateTimeOffset.UnixEpoch,
                        End = DateTimeOffset.UnixEpoch
                    }
                }
            );

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        DragaliaResult<MissionUnlockDrillMissionGroupData> resp =
            await this.missionController.UnlockDrillMissionGroup(
                new MissionUnlockDrillMissionGroupRequest(100)
            );

        MissionUnlockDrillMissionGroupData? response = resp.Value;
        response!
            .drill_mission_list.Should()
            .ContainEquivalentOf(
                new DrillMissionList(5000, 0, 1, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch)
            );

        mockMissionService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockMissionRepository.VerifyAll();
    }

    [Fact]
    public async Task UnlockMainMissionGroup_UnlocksGroupAndRewards()
    {
        DbPlayerMission fakeMission =
            new()
            {
                ViewerId = IdentityTestUtils.ViewerId,
                Id = 5000,
                State = MissionState.Completed,
                Type = MissionType.MainStory,
                Start = DateTimeOffset.UnixEpoch,
                End = DateTimeOffset.UnixEpoch
            };

        MainStoryMissionGroupReward fakeReward = new(EntityTypes.FortPlant, 10, 500);

        this.mockMissionService.Setup(x => x.UnlockMainMissionGroup(100))
            .ReturnsAsync((new[] { fakeReward }, new[] { fakeMission }));

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        DragaliaResult<MissionUnlockMainStoryGroupData> resp =
            await this.missionController.UnlockMainStoryMissionGroup(
                new MissionUnlockMainStoryGroupRequest(100)
            );

        MissionUnlockMainStoryGroupData? response = resp.Value;
        response!
            .main_story_mission_list.Should()
            .ContainEquivalentOf(
                new MainStoryMissionList(
                    5000,
                    0,
                    1,
                    DateTimeOffset.UnixEpoch,
                    DateTimeOffset.UnixEpoch
                )
            );
        response
            .main_story_mission_unlock_bonus_list.Should()
            .ContainEquivalentOf(
                new AtgenBuildEventRewardEntityList(EntityTypes.FortPlant, 10, 500)
            );

        mockMissionService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockMissionRepository.VerifyAll();
    }

    [Fact]
    public async Task ReceiveDrillReward_ReceivesReward()
    {
        AtgenBuildEventRewardEntityList reward = new(EntityTypes.FortPlant, 10, 420);

        List<int> fakeIdList = new() { 50, 56, 58, 19 };

        this.mockMissionService.Setup(x => x.RedeemMissions(MissionType.Drill, fakeIdList))
            .Returns(Task.CompletedTask);

        this.mockMissionService.Setup(
            x => x.TryRedeemDrillMissionGroups(It.IsAny<IEnumerable<int>>())
        )
            .ReturnsAsync(new List<AtgenBuildEventRewardEntityList>());

        this.mockMissionService.Setup(x => x.GetCompletedDrillGroups())
            .ReturnsAsync(new List<DrillMissionGroupList>() { new(1) });

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(new UpdateDataList());

        this.mockMissionRepository.Setup(x => x.GetMissionsByType(MissionType.Drill))
            .Returns(Enumerable.Empty<DbPlayerMission>().AsQueryable().BuildMock());

        this.mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());

        DragaliaResult<MissionReceiveDrillRewardData> resp =
            await this.missionController.ReceiveDrillStoryReward(
                new MissionReceiveDrillRewardRequest(fakeIdList, Enumerable.Empty<int>())
            );

        MissionReceiveDrillRewardData? response = resp.Value;

        mockRewardService.VerifyAll();
        mockMissionService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockMissionRepository.VerifyAll();
    }
}
