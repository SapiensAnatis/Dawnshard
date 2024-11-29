using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Features.Tutorial;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Tutorial;

/// <summary>
/// Tests <see cref="TutorialController"/>
/// </summary>
public class TutorialTest : TestFixture
{
    public TutorialTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper)
    {
        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task TutorialUpdateStep_UpdatesDB()
    {
        int step = 20000;

        TutorialUpdateStepResponse response = (
            await this.Client.PostMsgpack<TutorialUpdateStepResponse>(
                "/tutorial/update_step",
                new TutorialUpdateStepRequest(step, false, 0, 0),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        this.ApiContext.PlayerUserData.AsNoTracking()
            .First(x => x.ViewerId == this.ViewerId)
            .TutorialStatus.Should()
            .Be(step);
    }

    [Fact]
    public async Task TutorialUpdateStep_ReturnsCorrectResponse()
    {
        int step = 20000;

        DbPlayerUserData dbUserData = (
            await this.ApiContext.PlayerUserData.FindAsync(
                this.ViewerId,
                TestContext.Current.CancellationToken
            )
        )!;

        UserData expUserData = this.Mapper.Map<UserData>(dbUserData);
        expUserData.TutorialStatus = step;
        UpdateDataList expUpdateData = new() { UserData = expUserData };

        TutorialUpdateStepResponse response = (
            await this.Client.PostMsgpack<TutorialUpdateStepResponse>(
                "/tutorial/update_step",
                new TutorialUpdateStepRequest(step, false, 0, 0),
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.UpdateDataList.Should().BeEquivalentTo(expUpdateData);
    }

    [Fact]
    public async Task TutorialUpdateFlags_UpdatesDB()
    {
        int flag = 1020;

        TutorialUpdateFlagsResponse response = (
            await this.Client.PostMsgpack<TutorialUpdateFlagsResponse>(
                "/tutorial/update_flags",
                new TutorialUpdateFlagsRequest() { FlagId = flag },
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        TutorialFlagUtil
            .ConvertIntToFlagIntList(
                this.ApiContext.PlayerUserData.AsNoTracking()
                    .First(x => x.ViewerId == this.ViewerId)
                    .TutorialFlag
            )
            .Should()
            .BeEquivalentTo(new List<int>() { flag });
    }
}
