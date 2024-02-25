using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Utils;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.TutorialController"/>
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
                new TutorialUpdateStepRequest(step, false, 0, 0)
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
            await this.ApiContext.PlayerUserData.FindAsync(this.ViewerId)
        )!;

        UserData expUserData = this.Mapper.Map<UserData>(dbUserData);
        expUserData.TutorialStatus = step;
        UpdateDataList expUpdateData = new() { UserData = expUserData };

        TutorialUpdateStepResponse response = (
            await this.Client.PostMsgpack<TutorialUpdateStepResponse>(
                "/tutorial/update_step",
                new TutorialUpdateStepRequest(step, false, 0, 0)
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
                new TutorialUpdateFlagsRequest() { FlagId = flag }
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
