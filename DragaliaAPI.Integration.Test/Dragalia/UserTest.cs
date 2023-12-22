using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class UserTest : TestFixture
{
    public UserTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task LinkedNAccount_ReturnsExpectedResponse()
    {
        DbPlayerUserData dbUserData = this.ApiContext.PlayerUserData.Single(
            x => x.ViewerId == ViewerId
        );

        UserData expectedUserData = this.Mapper.Map<UserData>(dbUserData);

        (
            await this.Client.PostMsgpack<UserLinkedNAccountData>(
                "/user/linked_n_account",
                new UserLinkedNAccountRequest()
            )
        )
            .data.Should()
            .BeEquivalentTo(
                new UserLinkedNAccountData()
                {
                    update_data_list = new() { user_data = expectedUserData }
                },
                opts => opts.Excluding(x => x.update_data_list.user_data.crystal)
            );
    }

    [Fact]
    public async Task GetNAccountInfo_ReturnsExpectedResponse()
    {
        (
            await this.Client.PostMsgpack<UserGetNAccountInfoData>(
                "/user/get_n_account_info",
                new UserGetNAccountInfoRequest()
            )
        )
            .data.Should()
            .BeEquivalentTo(
                new UserGetNAccountInfoData()
                {
                    n_account_info = new()
                    {
                        email = "placeholder@email.com",
                        nickname = "placeholder nickname"
                    },
                    update_data_list = new()
                },
                opts => opts.Excluding(x => x.update_data_list.user_data.crystal)
            );
    }
}
