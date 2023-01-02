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
            x => x.DeviceAccountId == fixture.DeviceAccountId
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
}
