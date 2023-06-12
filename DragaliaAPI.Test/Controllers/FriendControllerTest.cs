using DragaliaAPI.Controllers.Dragalia;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

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
        this.mockHelperService
            .Setup(x => x.GetHelpers())
            .ReturnsAsync(
                new QuestGetSupportUserListData()
                {
                    support_user_list = new List<UserSupportList>() { TestData.supportListEuden },
                    support_user_detail_list = new List<AtgenSupportUserDetailList>()
                    {
                        new() { viewer_id = 1000, is_friend = true, },
                    }
                }
            );

        ActionResult<DragaliaResponse<object>> response =
            await this.friendController.GetSupportCharaDetail(
                new FriendGetSupportCharaDetailRequest() { support_viewer_id = 1000 }
            );

        FriendGetSupportCharaDetailData? data = response.GetData<FriendGetSupportCharaDetailData>();
        data.Should().NotBeNull();

        data!.support_user_data_detail.user_support_data
            .Should()
            .BeEquivalentTo(TestData.supportListEuden);
        data!.support_user_data_detail.is_friend.Should().Be(true);

        this.mockHelperService.VerifyAll();
        this.mockBonusService.VerifyAll();
    }

    [Fact]
    public async Task GetSupportCharaDetailContainsDefaultInformationWhenNotFound()
    {
        this.mockHelperService
            .Setup(x => x.GetHelpers())
            .ReturnsAsync(
                new QuestGetSupportUserListData()
                {
                    support_user_list = new List<UserSupportList>() { TestData.supportListEuden },
                    support_user_detail_list = new List<AtgenSupportUserDetailList>()
                    {
                        new() { viewer_id = 1000, is_friend = true },
                    }
                }
            );

        ActionResult<DragaliaResponse<object>> response =
            await this.friendController.GetSupportCharaDetail(
                new FriendGetSupportCharaDetailRequest() { support_viewer_id = 0 }
            );

        FriendGetSupportCharaDetailData? data = response.GetData<FriendGetSupportCharaDetailData>();
        data.Should().NotBeNull();

        data!.support_user_data_detail.user_support_data
            .Should()
            .BeEquivalentTo(HelperService.StubData.SupportListData.support_user_list.First());

        data!.support_user_data_detail.is_friend.Should().Be(false);

        this.mockHelperService.VerifyAll();
        this.mockBonusService.VerifyAll();
    }
}
