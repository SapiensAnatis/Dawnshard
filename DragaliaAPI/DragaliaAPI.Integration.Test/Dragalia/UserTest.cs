using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class UserTest : TestFixture
{
    public UserTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task LinkedNAccount_ReturnsExpectedResponse()
    {
        DbPlayerUserData dbUserData = this.ApiContext.PlayerUserData.Single(x =>
            x.ViewerId == ViewerId
        );

        UserData expectedUserData = this.Mapper.Map<UserData>(dbUserData);

        (
            await this.Client.PostMsgpack<UserLinkedNAccountResponse>(
                "/user/linked_n_account",
                new UserLinkedNAccountRequest()
            )
        )
            .data.Should()
            .BeEquivalentTo(
                new UserLinkedNAccountData()
                {
                    UpdateDataList = new() { UserData = expectedUserData }
                },
                opts => opts.Excluding(x => x.UpdateDataList.UserData.Crystal)
            );
    }

    [Fact]
    public async Task GetNAccountInfo_ReturnsExpectedResponse()
    {
        (
            await this.Client.PostMsgpack<UserGetNAccountInfoResponse>(
                "/user/get_n_account_info",
                new UserGetNAccountInfoRequest()
            )
        )
            .data.Should()
            .BeEquivalentTo(
                new UserGetNAccountInfoData()
                {
                    NAccountInfo = new()
                    {
                        Email = "placeholder@email.com",
                        Nickname = "placeholder nickname"
                    },
                    UpdateDataList = new()
                },
                opts => opts.Excluding(x => x.UpdateDataList.UserData.Crystal)
            );
    }
}
