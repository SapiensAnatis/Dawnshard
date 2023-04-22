using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Test.Integration.Dragalia;

[Collection("DragaliaIntegration")]
public class UserTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture fixture;
    private readonly HttpClient client;

    public UserTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        this.client = fixture.CreateClient();

        TestUtils.ApplyDateTimeAssertionOptions();
    }

    [Fact]
    public async Task LinkedNAccount_ReturnsExpectedResponse()
    {
        DbPlayerUserData dbUserData = this.fixture.ApiContext.PlayerUserData.Single(
            x => x.DeviceAccountId == IntegrationTestFixture.DeviceAccountIdConst
        );

        UserData expectedUserData = this.fixture.Mapper.Map<UserData>(dbUserData);

        (
            await this.client.PostMsgpack<UserLinkedNAccountData>(
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
            await this.client.PostMsgpack<UserGetNAccountInfoData>(
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
}
