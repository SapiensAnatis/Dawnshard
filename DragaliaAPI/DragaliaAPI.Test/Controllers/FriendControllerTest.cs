using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;

namespace DragaliaAPI.Test.Controllers;

public class FriendControllerTest
{
    private readonly FriendController friendController;
    private readonly Mock<IHelperService> mockHelperService;
    private readonly Mock<IBonusService> mockBonusService;

    public FriendControllerTest()
    {
        this.mockHelperService = new(MockBehavior.Strict);
        this.mockBonusService = new(MockBehavior.Strict);

        this.mockBonusService.Setup(x => x.GetBonusList()).ReturnsAsync(new FortBonusList());

        this.friendController = new FriendController(
            mockHelperService.Object,
            mockBonusService.Object
        );

        this.friendController.SetupMockContext();
    }

    [Fact]
    public async Task GetSupportCharaDetailContainsCorrectInformationWhenFound()
    {
        this.mockHelperService.Setup(x => x.GetHelpers())
            .ReturnsAsync(
                new QuestGetSupportUserListResponse()
                {
                    SupportUserList = new List<UserSupportList>() { TestData.SupportListEuden },
                    SupportUserDetailList = new List<AtgenSupportUserDetailList>()
                    {
                        new() { ViewerId = 1000, IsFriend = true, },
                    }
                }
            );

        DragaliaResult response = await this.friendController.GetSupportCharaDetail(
            new FriendGetSupportCharaDetailRequest() { SupportViewerId = 1000 }
        );

        FriendGetSupportCharaDetailResponse? data =
            response.GetData<FriendGetSupportCharaDetailResponse>();
        data.Should().NotBeNull();

        data!
            .SupportUserDataDetail.UserSupportData.Should()
            .BeEquivalentTo(TestData.SupportListEuden);
        data!.SupportUserDataDetail.IsFriend.Should().Be(true);

        this.mockHelperService.VerifyAll();
        this.mockBonusService.VerifyAll();
    }

    [Fact]
    public async Task GetSupportCharaDetailContainsDefaultInformationWhenNotFound()
    {
        this.mockHelperService.Setup(x => x.GetHelpers())
            .ReturnsAsync(
                new QuestGetSupportUserListResponse()
                {
                    SupportUserList = new List<UserSupportList>() { TestData.SupportListEuden },
                    SupportUserDetailList = new List<AtgenSupportUserDetailList>()
                    {
                        new() { ViewerId = 1000, IsFriend = true },
                    }
                }
            );

        DragaliaResult response = await this.friendController.GetSupportCharaDetail(
            new FriendGetSupportCharaDetailRequest() { SupportViewerId = 0 }
        );

        FriendGetSupportCharaDetailResponse? data =
            response.GetData<FriendGetSupportCharaDetailResponse>();
        data.Should().NotBeNull();

        data!
            .SupportUserDataDetail.UserSupportData.Should()
            .BeEquivalentTo(HelperService.StubData.SupportListData.SupportUserList.First());

        data!.SupportUserDataDetail.IsFriend.Should().Be(false);

        this.mockHelperService.VerifyAll();
        this.mockBonusService.VerifyAll();
    }
}
