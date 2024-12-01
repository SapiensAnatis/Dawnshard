using DragaliaAPI.Infrastructure;

namespace DragaliaAPI.Integration.Test.Features.Tool;

public class TransitionTest : TestFixture
{
    public TransitionTest(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper) { }

    [Fact]
    public async Task Transition_CorrectIdToken_ReturnsOKResponse()
    {
        string token = TokenHelper.GetToken(
            DeviceAccountId,
            DateTime.UtcNow + TimeSpan.FromMinutes(5)
        );
        this.Client.DefaultRequestHeaders.Add(DragaliaHttpConstants.Headers.IdToken, token);

        TransitionTransitionByNAccountResponse response = (
            await this.Client.PostMsgpack<TransitionTransitionByNAccountResponse>(
                "/transition/transition_by_n_account",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.TransitionResultData.LinkedViewerId.Should().Be((ulong)this.ViewerId);
        response.TransitionResultData.AbolishedViewerId.Should().Be(0);
    }

    [Fact]
    public async Task Transition_NewUser_CorrectIdToken_CreatesAccount_ReturnsOKResponse()
    {
        string token = TokenHelper.GetToken(
            $"new account {Guid.NewGuid()}",
            DateTime.UtcNow + TimeSpan.FromMinutes(5)
        );
        this.Client.DefaultRequestHeaders.Add(DragaliaHttpConstants.Headers.IdToken, token);

        TransitionTransitionByNAccountResponse response = (
            await this.Client.PostMsgpack<TransitionTransitionByNAccountResponse>(
                "/transition/transition_by_n_account",
                cancellationToken: TestContext.Current.CancellationToken
            )
        ).Data;

        response.TransitionResultData.LinkedViewerId.Should().BeGreaterThan((ulong)this.ViewerId);
        response.TransitionResultData.AbolishedViewerId.Should().Be(0);
    }
}
