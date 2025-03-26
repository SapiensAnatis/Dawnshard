using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Login.Savefile;
using DragaliaAPI.Infrastructure.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DragaliaAPI.Integration.Test.Features.Friends;

public class FriendTest : TestFixture
{
    public FriendTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task FriendIndex_ReturnsFriendInformation()
    {
        DbPlayer other1 = await this.CreateOtherPlayer();
        DbPlayer other2 = await this.CreateOtherPlayer();

        this.ApiContext.AddRange(
            [
                new DbPlayerFriendRequest()
                {
                    FromPlayerViewerId = other1.ViewerId,
                    ToPlayerViewerId = this.ViewerId,
                    IsNew = true,
                },
                new DbPlayerFriendship()
                {
                    PlayerFriendshipPlayers =
                    [
                        new DbPlayerFriendshipPlayer()
                        {
                            PlayerViewerId = this.ViewerId,
                            IsNew = true,
                        },
                        new DbPlayerFriendshipPlayer() { PlayerViewerId = other2.ViewerId },
                    ],
                },
            ]
        );
        this.ApiContext.SaveChanges();

        DragaliaResponse<FriendFriendIndexResponse> response =
            await this.Client.PostMsgpack<FriendFriendIndexResponse>(
                "/friend/friend_index",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.Should()
            .BeEquivalentTo(
                new FriendFriendIndexResponse()
                {
                    FriendCount = 1,
                    UpdateDataList = new()
                    {
                        FriendNotice = new() { FriendNewCount = 1, ApplyNewCount = 1 },
                    },
                }
            );
    }

    [Fact]
    public async Task FriendList_ReturnsFriendList()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();
        await this.CreateFriendship(otherPlayer);

        DragaliaResponse<FriendFriendListResponse> response =
            await this.Client.PostMsgpack<FriendFriendListResponse>(
                "/friend/friend_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        // lazy
        response
            .Data.FriendList.Should()
            .ContainSingle()
            .Which.ViewerId.Should()
            .Be((ulong)otherPlayer.ViewerId);
    }

    [Fact]
    public async Task AutoSearch_ReturnsSuggestedFriends()
    {
        DbPlayer existingFriend = await this.CreateOtherPlayer();
        DbPlayer pendingRequest = await this.CreateOtherPlayer();
        DbPlayer suggested = await this.CreateOtherPlayer();

        // Set last login time to be high so suggested player always shows up in top 10
        int rowsUpdated = await this
            .ApiContext.PlayerUserData.IgnoreQueryFilters()
            .Where(x => x.ViewerId == suggested.ViewerId)
            .ExecuteUpdateAsync(
                e => e.SetProperty(x => x.LastLoginTime, DateTimeOffset.MaxValue),
                TestContext.Current.CancellationToken
            );

        rowsUpdated.Should().Be(1);

        await this.CreateFriendship(existingFriend);
        await this.CreateFriendRequestTo(pendingRequest);

        DragaliaResponse<FriendAutoSearchResponse> response =
            await this.Client.PostMsgpack<FriendAutoSearchResponse>(
                "/friend/auto_search",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.SearchList.Should().Contain(x => x.ViewerId == (ulong)suggested.ViewerId);
        response
            .Data.SearchList.Should()
            .NotContain(x => x.ViewerId == (ulong)existingFriend.ViewerId);
        response
            .Data.SearchList.Should()
            .NotContain(x => x.ViewerId == (ulong)pendingRequest.ViewerId);

        // test with other players in the db that probably exist at this point
        response.Data.SearchList.Should().BeInDescendingOrder(x => x.LastLoginDate);
    }

    [Fact]
    public async Task RequestList_ReturnsSentRequests()
    {
        DbPlayer sentRequest = await this.CreateOtherPlayer();

        await this.CreateFriendRequestTo(sentRequest);

        DragaliaResponse<FriendRequestListResponse> response =
            await this.Client.PostMsgpack<FriendRequestListResponse>(
                "/friend/request_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.RequestList.Should()
            .ContainSingle()
            .Which.ViewerId.Should()
            .Be((ulong)sentRequest.ViewerId);
    }

    [Fact]
    public async Task ApplyList_ReturnsReceivedRequests()
    {
        DbPlayer receivedRequest = await this.CreateOtherPlayer();

        await this.CreateFriendRequestFrom(receivedRequest);

        DragaliaResponse<FriendApplyListResponse> response =
            await this.Client.PostMsgpack<FriendApplyListResponse>(
                "/friend/apply_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.FriendApply.Should()
            .ContainSingle()
            .Which.ViewerId.Should()
            .Be((ulong)receivedRequest.ViewerId);

        // Request is not new
        response.Data.NewApplyViewerIdList.Should().BeEmpty();
    }

    [Fact]
    public async Task IdSearch_ReturnsUser()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();

        DragaliaResponse<FriendIdSearchResponse> response =
            await this.Client.PostMsgpack<FriendIdSearchResponse>(
                "/friend/id_search",
                new FriendIdSearchRequest() { SearchId = (ulong)otherPlayer.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.SearchUser.ViewerId.Should().Be((ulong)otherPlayer.ViewerId);
    }

    [Fact]
    public async Task IdSearch_OwnId_ReturnsIdSearchError()
    {
        DragaliaResponse<FriendIdSearchResponse> response =
            await this.Client.PostMsgpack<FriendIdSearchResponse>(
                "/friend/id_search",
                new FriendIdSearchRequest() { SearchId = (ulong)this.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendIdsearchError);
    }

    [Fact]
    public async Task IdSearch_AlreadyFriends_ReturnsFriendTargetAlready()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();

        await this.CreateFriendship(otherPlayer);

        DragaliaResponse<FriendIdSearchResponse> response =
            await this.Client.PostMsgpack<FriendIdSearchResponse>(
                "/friend/id_search",
                new FriendIdSearchRequest() { SearchId = (ulong)otherPlayer.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendTargetAlready);
    }

    [Fact]
    public async Task IdSearch_NonExistentUser_ReturnsFriendTargetNone()
    {
        DragaliaResponse<FriendIdSearchResponse> response =
            await this.Client.PostMsgpack<FriendIdSearchResponse>(
                "/friend/id_search",
                new FriendIdSearchRequest() { SearchId = ulong.MaxValue },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendTargetNone);
    }

    [Fact]
    public async Task Request_SendsRequest_IsVisibleByOtherPlayer()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();

        await this.Client.PostMsgpack<FriendRequestResponse>(
            "/friend/request",
            new FriendRequestRequest() { FriendId = (ulong)otherPlayer.ViewerId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        HttpClient otherPlayerClient = this.CreateClientForOtherPlayer(otherPlayer);

        DragaliaResponse<FriendFriendIndexResponse> indexResponse =
            await otherPlayerClient.PostMsgpack<FriendFriendIndexResponse>(
                "/friend/friend_index",
                cancellationToken: TestContext.Current.CancellationToken
            );

        indexResponse.Data.UpdateDataList.FriendNotice.ApplyNewCount.Should().Be(1);

        DragaliaResponse<FriendApplyListResponse> applyListResponse =
            await otherPlayerClient.PostMsgpack<FriendApplyListResponse>(
                "/friend/apply_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        applyListResponse
            .Data.FriendApply.Should()
            .ContainSingle()
            .Which.ViewerId.Should()
            .Be((ulong)this.ViewerId);

        applyListResponse
            .Data.NewApplyViewerIdList.Should()
            .ContainSingle()
            .Which.Should()
            .Be((ulong)this.ViewerId);
    }

    [Fact]
    public async Task Request_ToSelf_ReturnsFriendApplyError()
    {
        DragaliaResponse<FriendRequestResponse> response =
            await this.Client.PostMsgpack<FriendRequestResponse>(
                "/friend/request",
                new FriendRequestRequest() { FriendId = (ulong)this.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendApplyError);
    }

    [Fact]
    public async Task Request_ToExistingFriend_ReturnsFriendTargetAlready()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();

        await this.CreateFriendship(otherPlayer);

        DragaliaResponse<FriendRequestResponse> response =
            await this.Client.PostMsgpack<FriendRequestResponse>(
                "/friend/request",
                new FriendRequestRequest() { FriendId = (ulong)otherPlayer.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendTargetAlready);
    }

    [Fact]
    public async Task Request_RequestAlreadyExists_ReturnsFriendApplyExists()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();

        await this.CreateFriendRequestTo(otherPlayer);

        DragaliaResponse<FriendRequestResponse> response =
            await this.Client.PostMsgpack<FriendRequestResponse>(
                "/friend/request",
                new FriendRequestRequest() { FriendId = (ulong)otherPlayer.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendApplyExists);
    }

    [Fact]
    public async Task Request_OwnFriendsListFull_ReturnsFriendCountLimit()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();
        await this.CreateFriends(this.ViewerId, 175);

        DragaliaResponse<FriendRequestResponse> response =
            await this.Client.PostMsgpack<FriendRequestResponse>(
                "/friend/request",
                new FriendRequestRequest() { FriendId = (ulong)otherPlayer.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendCountLimit);
    }

    [Fact]
    public async Task Request_OtherFriendsListFull_ReturnsFriendCountOtherLimit()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();
        await this.CreateFriends(otherPlayer.ViewerId, 175);

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<FriendRequestResponse> response =
            await this.Client.PostMsgpack<FriendRequestResponse>(
                "/friend/request",
                new FriendRequestRequest() { FriendId = (ulong)otherPlayer.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendCountOtherLimit);
    }

    [Fact]
    public async Task Request_OwnFriendRequestLimitReached_ReturnsFriendApplyCountLimit()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();
        await this.CreateFriends(this.ViewerId, 173);

        this.ApiContext.PlayerFriendRequests.AddRange(
            [
                new()
                {
                    FromPlayerViewerId = this.ViewerId,
                    ToPlayer = new DbPlayer() { AccountId = $"ApplyLimit_{Guid.NewGuid()}" },
                },
                new()
                {
                    FromPlayer = new DbPlayer() { AccountId = $"ApplyLimit_{Guid.NewGuid()}" },
                    ToPlayerViewerId = this.ViewerId,
                },
            ]
        );
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<FriendRequestResponse> response =
            await this.Client.PostMsgpack<FriendRequestResponse>(
                "/friend/request",
                new FriendRequestRequest() { FriendId = (ulong)otherPlayer.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendApplyCountLimit);
    }

    [Fact]
    public async Task Request_OtherFriendRequestLimitReached_ReturnsFriendApplyCountOtherLimit()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();
        await this.CreateFriends(otherPlayer.ViewerId, 23); // Fresh player has a limit of 25 friends

        this.ApiContext.PlayerFriendRequests.AddRange(
            [
                new()
                {
                    FromPlayerViewerId = otherPlayer.ViewerId,
                    ToPlayer = new DbPlayer() { AccountId = $"ApplyLimit_{Guid.NewGuid()}" },
                },
                new()
                {
                    FromPlayer = new DbPlayer() { AccountId = $"ApplyLimit_{Guid.NewGuid()}" },
                    ToPlayerViewerId = otherPlayer.ViewerId,
                },
            ]
        );
        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DragaliaResponse<FriendRequestResponse> response =
            await this.Client.PostMsgpack<FriendRequestResponse>(
                "/friend/request",
                new FriendRequestRequest() { FriendId = (ulong)otherPlayer.ViewerId },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendApplyCountOtherLimit);
    }

    [Fact]
    public async Task Request_NonExistentUser_ReturnsFriendApplyError()
    {
        DragaliaResponse<FriendRequestResponse> response =
            await this.Client.PostMsgpack<FriendRequestResponse>(
                "/friend/request",
                new FriendRequestRequest() { FriendId = ulong.MaxValue },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendApplyError);
    }

    [Fact]
    public async Task RequestCancel_CancelsRequest()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();

        await this.Client.PostMsgpack<FriendRequestResponse>(
            "/friend/request",
            new FriendRequestRequest() { FriendId = (ulong)otherPlayer.ViewerId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        HttpClient otherPlayerClient = this.CreateClientForOtherPlayer(otherPlayer);

        DragaliaResponse<FriendFriendIndexResponse> indexResponse =
            await otherPlayerClient.PostMsgpack<FriendFriendIndexResponse>(
                "/friend/friend_index",
                cancellationToken: TestContext.Current.CancellationToken
            );

        indexResponse.Data.UpdateDataList.FriendNotice.ApplyNewCount.Should().Be(1);

        await this.Client.PostMsgpack<FriendRequestCancelResponse>(
            "/friend/request_cancel",
            new FriendRequestCancelRequest() { FriendId = (ulong)otherPlayer.ViewerId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<FriendFriendIndexResponse> secondIndexResponse =
            await otherPlayerClient.PostMsgpack<FriendFriendIndexResponse>(
                "/friend/friend_index",
                cancellationToken: TestContext.Current.CancellationToken
            );

        secondIndexResponse.Data.UpdateDataList.FriendNotice.Should().BeNull();
    }

    [Fact]
    public async Task RequestCancel_NonExistentRequest_ReturnsFriendApplyError()
    {
        DragaliaResponse<FriendRequestCancelResponse> response =
            await this.Client.PostMsgpack<FriendRequestCancelResponse>(
                "/friend/request_cancel",
                new FriendRequestCancelRequest() { FriendId = ulong.MaxValue },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendApplyError);
    }

    [Fact]
    public async Task Reply_Accept_CreatesFriendship()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();
        await this.CreateFriendRequestFrom(otherPlayer);

        await this.Client.PostMsgpack<FriendReplyResponse>(
            "/friend/reply",
            new FriendReplyRequest()
            {
                FriendId = (ulong)otherPlayer.ViewerId,
                Reply = FriendReplyType.Accept,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<FriendApplyListResponse> applyListResponse =
            await this.Client.PostMsgpack<FriendApplyListResponse>(
                "/friend/apply_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        applyListResponse.Data.FriendApply.Should().BeEmpty();

        DragaliaResponse<FriendFriendListResponse> response =
            await this.Client.PostMsgpack<FriendFriendListResponse>(
                "/friend/friend_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response
            .Data.FriendList.Should()
            .ContainSingle()
            .Which.ViewerId.Should()
            .Be((ulong)otherPlayer.ViewerId);
    }

    [Fact]
    public async Task Reply_Decline_DoesNotCreateFriendship()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();
        await this.CreateFriendRequestFrom(otherPlayer);

        await this.Client.PostMsgpack<FriendReplyResponse>(
            "/friend/reply",
            new FriendReplyRequest()
            {
                FriendId = (ulong)otherPlayer.ViewerId,
                Reply = FriendReplyType.Decline,
            },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<FriendApplyListResponse> applyListResponse =
            await this.Client.PostMsgpack<FriendApplyListResponse>(
                "/friend/apply_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        applyListResponse.Data.FriendApply.Should().BeEmpty();

        DragaliaResponse<FriendFriendListResponse> response =
            await this.Client.PostMsgpack<FriendFriendListResponse>(
                "/friend/friend_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.FriendList.Should().BeEmpty();
    }

    [Fact]
    public async Task Reply_NonExistentRequest_ReturnsCommonInvalidArgument()
    {
        DragaliaResponse<FriendReplyResponse> response =
            await this.Client.PostMsgpack<FriendReplyResponse>(
                "/friend/reply",
                new FriendReplyRequest()
                {
                    FriendId = ulong.MaxValue,
                    Reply = FriendReplyType.Decline,
                },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.CommonInvalidArgument);
    }

    [Fact]
    public async Task Delete_DeletesFriend()
    {
        DbPlayer otherPlayer = await this.CreateOtherPlayer();
        await this.CreateFriendship(otherPlayer);

        await this.Client.PostMsgpack<FriendDeleteResponse>(
            "/friend/delete",
            new FriendDeleteRequest() { FriendId = (ulong)otherPlayer.ViewerId },
            cancellationToken: TestContext.Current.CancellationToken
        );

        DragaliaResponse<FriendFriendListResponse> response =
            await this.Client.PostMsgpack<FriendFriendListResponse>(
                "/friend/friend_list",
                cancellationToken: TestContext.Current.CancellationToken
            );

        response.Data.FriendList.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_NonExistentFriend_ReturnsFriendDeleteError()
    {
        DragaliaResponse<FriendDeleteResponse> response =
            await this.Client.PostMsgpack<FriendDeleteResponse>(
                "/friend/delete",
                new FriendDeleteRequest() { FriendId = ulong.MaxValue },
                cancellationToken: TestContext.Current.CancellationToken,
                ensureSuccessHeader: false
            );

        response.DataHeaders.ResultCode.Should().Be(ResultCode.FriendDeleteError);
    }

    private async Task<DbPlayer> CreateOtherPlayer()
    {
        return await this
            .Services.GetRequiredService<ISavefileService>()
            .Create($"friend {Guid.NewGuid()}");
    }

    private async Task CreateFriendship(DbPlayer other)
    {
        this.ApiContext.PlayerFriendships.Add(
            new DbPlayerFriendship()
            {
                PlayerFriendshipPlayers =
                [
                    new DbPlayerFriendshipPlayer() { PlayerViewerId = this.ViewerId },
                    new DbPlayerFriendshipPlayer() { PlayerViewerId = other.ViewerId },
                ],
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private async Task CreateFriendRequestTo(DbPlayer other)
    {
        this.ApiContext.PlayerFriendRequests.Add(
            new DbPlayerFriendRequest()
            {
                FromPlayerViewerId = this.ViewerId,
                ToPlayerViewerId = other.ViewerId,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private async Task CreateFriendRequestFrom(DbPlayer other)
    {
        this.ApiContext.PlayerFriendRequests.Add(
            new DbPlayerFriendRequest()
            {
                FromPlayerViewerId = other.ViewerId,
                ToPlayerViewerId = this.ViewerId,
            }
        );

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private async Task CreateFriends(long viewerId, int count)
    {
        IEnumerable<DbPlayerFriendship> friendships = Enumerable
            .Range(0, count)
            .Select(x => new DbPlayerFriendship()
            {
                PlayerFriendshipPlayers =
                [
                    new DbPlayerFriendshipPlayer() { PlayerViewerId = viewerId },
                    new DbPlayerFriendshipPlayer()
                    {
                        Player = new() { AccountId = $"CreateFriends_{viewerId}_{x}" },
                    },
                ],
            });

        this.ApiContext.PlayerFriendships.AddRange(friendships);

        await this.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
