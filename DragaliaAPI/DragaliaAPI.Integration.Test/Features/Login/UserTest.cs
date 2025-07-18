﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Mapping.Mapperly;

namespace DragaliaAPI.Integration.Test.Features.Login;

public class UserTest : TestFixture
{
    public UserTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task LinkedNAccount_ReturnsExpectedResponse()
    {
        DbPlayerUserData dbUserData = this.ApiContext.PlayerUserData.Single(x =>
            x.ViewerId == this.ViewerId
        );

        UserData expectedUserData = dbUserData.MapToUserData();

        (
            await this.Client.PostMsgpack<UserLinkedNAccountResponse>(
                "/user/linked_n_account",
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .Data.Should()
            .BeEquivalentTo(
                new UserLinkedNAccountResponse()
                {
                    UpdateDataList = new() { UserData = expectedUserData },
                },
                opts =>
                    opts.Excluding(x => x.UpdateDataList.UserData.Crystal).WithDateTimeTolerance()
            );
    }

    [Fact]
    public async Task GetNAccountInfo_ReturnsExpectedResponse()
    {
        this.MockBaasApi.Setup(x => x.GetUsername(It.IsAny<string>())).ReturnsAsync("okada");

        (
            await this.Client.PostMsgpack<UserGetNAccountInfoResponse>(
                "/user/get_n_account_info",
                cancellationToken: TestContext.Current.CancellationToken
            )
        )
            .Data.Should()
            .BeEquivalentTo(
                new UserGetNAccountInfoResponse()
                {
                    NAccountInfo = new() { Email = "", Nickname = "okada" },
                    UpdateDataList = new(),
                }
            );
    }
}
