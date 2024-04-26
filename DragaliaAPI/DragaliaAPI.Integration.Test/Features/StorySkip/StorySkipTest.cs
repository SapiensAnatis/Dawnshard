namespace DragaliaAPI.Integration.Test.Features.StorySkip;

public class StorySkipTest : TestFixture
{
    private readonly ITestOutputHelper testOutputHelper;

    public StorySkipTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper
    )
        : base(factory, testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task StorySkip_Success()
    {
        StorySkipSkipResponse data = (
            await this.Client.PostMsgpack<StorySkipSkipResponse>(
                "story_skip/skip"
            )
        ).Data;

        data.Should().BeEquivalentTo(new StorySkipSkipResponse() { ResultState = 1 });
    }
}