using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Features.Presents;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using Microsoft.EntityFrameworkCore;
using Renci.SshNet;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V26UpdateTest : SavefileUpdateTestFixture
{
    protected override int StartingVersion => 25;

    public V26UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V26Update_GrantsExpectedPresent()
    {
        int playerLevel = 30;
        int playerXp = MasterAsset.UserLevel[playerLevel].TotalExp;

        await this.ApiContext.PlayerUserData.ExecuteUpdateAsync(
            u => u.SetProperty(e => e.Level, playerLevel).SetProperty(e => e.Exp, playerXp),
            cancellationToken: TestContext.Current.CancellationToken
        );

        await this.LoadIndex();

        PresentGetPresentListResponse getPresentListResponse = (
            await this.Client.PostMsgpack<PresentGetPresentListResponse>(
                "/present/get_present_list",
                new PresentGetPresentListRequest() { IsLimit = false },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        int expectedQuantity = 50 * (playerLevel - 1); // 29 level ups

        getPresentListResponse
            .PresentList.Should()
            .ContainSingle(x => x.MessageId == PresentMessage.PlayerLevelUp)
            .Which.Should()
            .BeEquivalentTo(
                new PresentDetailList()
                {
                    EntityType = EntityTypes.Wyrmite,
                    EntityQuantity = expectedQuantity,
                    EntityLevel = 1,
                    MessageId = PresentMessage.PlayerLevelUp,
                    MessageParamValue1 = playerLevel,
                    ReceiveLimitTime = DateTimeOffset.UnixEpoch,
                    CreateTime = DateTimeOffset.UtcNow,
                },
                opts => opts.Excluding(x => x.PresentId).WithDateTimeTolerance()
            );
    }
}
