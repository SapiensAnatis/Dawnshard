using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using Xunit.Abstractions;

namespace DragaliaAPI.Integration.Test.Dragalia;

[Collection("DragaliaIntegration")]
public class UserTest : TestFixture
{
    [Fact]
    public async Task LinkedNAccount_ReturnsExpectedResponse()
    {
<<<<<<< HEAD:DragaliaAPI.Test/Integration/Dragalia/UserTest.cs
        DbPlayerUserData dbUserData = this.fixture.ApiContext.PlayerUserData.Single(
            x => x.DeviceAccountId == IntegrationTestFixture.DeviceAccountIdConst
=======
        DbPlayerUserData dbUserData = this.ApiContext.PlayerUserData.Single(
            x => x.DeviceAccountId == DeviceAccountId
>>>>>>> bb29ecf (Attempt to use containers for tests):DragaliaAPI.Integration.Test/Dragalia/UserTest.cs
        );

        UserData expectedUserData = this.Mapper.Map<UserData>(dbUserData);

        (
            await this.Client.PostMsgpack<UserLinkedNAccountData>(
                "/user/linked_n_account",
                new UserLinkedNAccountRequest()
            )
        ).data
            .Should()
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
        ).data
            .Should()
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

    public UserTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }
}
