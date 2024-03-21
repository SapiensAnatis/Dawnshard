using DragaliaAPI.Database.Entities;
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
                NormalMissionNotice = new AtgenNormalMissionNotice()
                {
                    IsUpdate = true,
                    AllMissionCount = 420
                }
            };

        CurrentMainStoryMission mainStoryMission =
            new(1337, new List<AtgenMainStoryMissionStateList>());

        this.mockMissionService.Setup(x => x.GetMissionNotice(null)).ReturnsAsync(notice);

        this.mockMissionService.Setup(x => x.GetCurrentMainStoryMission())
            .ReturnsAsync(mainStoryMission);
        this.mockMissionService.Setup(x => x.BuildNormalResponse<MissionGetMissionListResponse>())
            .ReturnsAsync(
                new MissionGetMissionListResponse()
                {
                    NormalMissionList = [],
                    MissionNotice = notice,
                    CurrentMainStoryMission = mainStoryMission
                }
            );

        DragaliaResult<MissionGetMissionListResponse> resp =
            await this.missionController.GetMissionList();

        MissionGetMissionListResponse? response = resp.Value;
        response.Should().NotBeNull();

        response!.MissionNotice.Should().Be(notice);
        response.CurrentMainStoryMission.Should().Be(mainStoryMission);
        response.NormalMissionList.Should().BeEmpty();

        mockMissionService.VerifyAll();
        mockUpdateDataService.VerifyAll();
    }

    [Fact]
    public async Task GetDrillMissionList_ReturnsDrillMissionList()
    {
        MissionNotice notice =
            new()
            {
                NormalMissionNotice = new AtgenNormalMissionNotice()
                {
                    IsUpdate = true,
                    AllMissionCount = 420
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

        DragaliaResult<MissionGetDrillMissionListResponse> resp =
            await this.missionController.GetDrillMissionList();

        MissionGetDrillMissionListResponse? response = resp.Value;
        response.Should().NotBeNull();

        response!.MissionNotice.Should().Be(notice);
        response
            .DrillMissionList.Should()
            .ContainEquivalentOf(
                new DrillMissionList(500, 0, 0, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch)
            );
        response.DrillMissionGroupList.Should().BeEquivalentTo([new DrillMissionGroupList(1)]);

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

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(new UpdateDataList());

        DragaliaResult<MissionUnlockDrillMissionGroupResponse> resp =
            await this.missionController.UnlockDrillMissionGroup(
                new MissionUnlockDrillMissionGroupRequest(100),
                default
            );

        MissionUnlockDrillMissionGroupResponse? response = resp.Value;
        response!
            .DrillMissionList.Should()
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

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(new UpdateDataList());

        DragaliaResult<MissionUnlockMainStoryGroupResponse> resp =
            await this.missionController.UnlockMainStoryMissionGroup(
                new MissionUnlockMainStoryGroupRequest(100),
                default
            );

        MissionUnlockMainStoryGroupResponse? response = resp.Value;
        response!
            .MainStoryMissionList.Should()
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
            .MainStoryMissionUnlockBonusList.Should()
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

        this.mockMissionService.Setup(x =>
            x.TryRedeemDrillMissionGroups(It.IsAny<IEnumerable<int>>())
        )
            .ReturnsAsync(new List<AtgenBuildEventRewardEntityList>());

        this.mockMissionService.Setup(x => x.GetCompletedDrillGroups())
            .ReturnsAsync(new List<DrillMissionGroupList>() { new(1) });

        this.mockUpdateDataService.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(new UpdateDataList());

        this.mockMissionRepository.Setup(x => x.GetMissionsByType(MissionType.Drill))
            .Returns(Enumerable.Empty<DbPlayerMission>().AsQueryable().BuildMock());

        this.mockRewardService.Setup(x => x.GetEntityResult()).Returns(new EntityResult());

        DragaliaResult<MissionReceiveDrillRewardResponse> resp =
            await this.missionController.ReceiveDrillStoryReward(
                new MissionReceiveDrillRewardRequest(fakeIdList, Enumerable.Empty<int>()),
                default
            );

        MissionReceiveDrillRewardResponse? response = resp.Value;

        mockRewardService.VerifyAll();
        mockMissionService.VerifyAll();
        mockUpdateDataService.VerifyAll();
        mockMissionRepository.VerifyAll();
    }
}
