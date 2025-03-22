using DragaliaAPI.Database.Entities;
using DragaliaAPI.Infrastructure.Results;

namespace DragaliaAPI.Integration.Test.Features.Configuration;

public class UpdateTest : TestFixture
{
    public UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task UpdateNamechange_UpdatesDB()
    {
        string newName = "Euden 2";

        await this.Client.PostMsgpack<UpdateNamechangeResponse>(
            "/update/namechange",
            new UpdateNamechangeRequest() { Name = newName },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DbPlayerUserData userData = this.ApiContext.PlayerUserData.Find(this.ViewerId)!;
        this.ApiContext.Entry(userData).Reload();
        userData.Name.Should().Be(newName);
    }

    [Fact]
    public async Task UpdateNamechange_ReturnsCorrectResponse()
    {
        string newName = "Euden 2";
        UpdateNamechangeResponse response = (
            await this.Client.PostMsgpack<UpdateNamechangeResponse>(
                "/update/namechange",
                new UpdateNamechangeRequest() { Name = newName },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.CheckedName.Should().Be(newName);
    }

    [Fact]
    public async Task UpdateResetNew_NullList_Handles()
    {
        DragaliaResponse<UpdateResetNewResponse> response = (
            await this.Client.PostMsgpack<UpdateResetNewResponse>(
                "/update/reset_new",
                new UpdateResetNewRequest()
                {
                    TargetList = new List<AtgenTargetList>()
                    {
                        new AtgenTargetList() { TargetName = "emblem", TargetIdList = null },
                    },
                },
                cancellationToken: TestContext.Current.CancellationToken
            )
        );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.Success);
    }

    [Fact]
    public async Task ResetNew_FriendRequest_Handles()
    {
        DbPlayer player1 = new() { AccountId = $"ResetNew_{Guid.NewGuid().ToString()}" };
        DbPlayer player2 = new() { AccountId = $"ResetNew_{Guid.NewGuid().ToString()}" };

        this.ApiContext.PlayerFriendRequests.AddRange(
            [
                new DbPlayerFriendRequest()
                {
                    FromPlayer = player1,
                    ToPlayerViewerId = this.ViewerId,
                    IsNew = true,
                },
                new DbPlayerFriendRequest()
                {
                    FromPlayer = player2,
                    ToPlayerViewerId = this.ViewerId,
                    IsNew = true,
                },
            ]
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<FriendFriendIndexResponse> indexResponse =
            await this.Client.PostMsgpack<FriendFriendIndexResponse>(
                "/friend/friend_index",
                cancellationToken: TestContext.Current.CancellationToken
            );

        indexResponse.Data.UpdateDataList.FriendNotice.ApplyNewCount.Should().Be(2);

        await this.Client.PostMsgpack<UpdateResetNewResponse>(
            "/update/reset_new",
            new UpdateResetNewRequest()
            {
                TargetList =
                [
                    new AtgenTargetList() { TargetName = "friend_apply", TargetIdList = null },
                ],
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        indexResponse = await this.Client.PostMsgpack<FriendFriendIndexResponse>(
            "/friend/friend_index",
            cancellationToken: TestContext.Current.CancellationToken
        );

        indexResponse.Data.UpdateDataList.FriendNotice.Should().BeNull();
    }

    [Fact]
    public async Task ResetNew_Friends_Handles()
    {
        DbPlayer player1 = new() { AccountId = $"ResetNew_{Guid.NewGuid().ToString()}" };
        DbPlayer player2 = new() { AccountId = $"ResetNew_{Guid.NewGuid().ToString()}" };

        this.ApiContext.PlayerFriendships.AddRange(
            [
                new DbPlayerFriendship()
                {
                    PlayerFriendshipPlayers =
                    [
                        new() { PlayerViewerId = this.ViewerId, IsNew = true },
                        new() { Player = player1 },
                    ],
                },
                new DbPlayerFriendship()
                {
                    PlayerFriendshipPlayers =
                    [
                        new() { PlayerViewerId = this.ViewerId, IsNew = true },
                        new() { Player = player2 },
                    ],
                },
            ]
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<FriendFriendIndexResponse> indexResponse =
            await this.Client.PostMsgpack<FriendFriendIndexResponse>(
                "/friend/friend_index",
                cancellationToken: TestContext.Current.CancellationToken
            );

        indexResponse.Data.UpdateDataList.FriendNotice.FriendNewCount.Should().Be(2);

        await this.Client.PostMsgpack<UpdateResetNewResponse>(
            "/update/reset_new",
            new UpdateResetNewRequest()
            {
                TargetList =
                [
                    new AtgenTargetList()
                    {
                        TargetName = "friend",
                        TargetIdList = [player1.ViewerId],
                    },
                ],
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        indexResponse = await this.Client.PostMsgpack<FriendFriendIndexResponse>(
            "/friend/friend_index",
            cancellationToken: TestContext.Current.CancellationToken
        );

        indexResponse.Data.UpdateDataList.FriendNotice.FriendNewCount.Should().Be(1);
    }
}
