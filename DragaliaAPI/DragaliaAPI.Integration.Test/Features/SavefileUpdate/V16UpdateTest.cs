using DragaliaAPI.Database.Entities;

namespace DragaliaAPI.Integration.Test.Features.SavefileUpdate;

public class V16UpdateTest : SavefileUpdateTestFixture
{
    public V16UpdateTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task V16Update_ConvertsEventItems()
    {
        await this.AddToDatabase(
            [
                new DbPlayerEventItem()
                {
                    EventId = 20844,
                    Id = 2084401,
                    Type = 10003,
                },
                new DbPlayerEventItem()
                {
                    EventId = 20844,
                    Id = 2084402,
                    Type = 10001,
                },
                new DbPlayerEventItem()
                {
                    EventId = 20844,
                    Id = 2084403,
                    Type = 10002,
                },
            ]
        );

        await this.LoadIndex();

        this.ApiContext.PlayerEventItems.Should()
            .BeEquivalentTo(
                [
                    new DbPlayerEventItem()
                    {
                        EventId = 20844,
                        Id = 2084401,
                        Type = 10001,
                    },
                    new DbPlayerEventItem()
                    {
                        EventId = 20844,
                        Id = 2084402,
                        Type = 10002,
                    },
                    new DbPlayerEventItem()
                    {
                        EventId = 20844,
                        Id = 2084403,
                        Type = 10003,
                    },
                ],
                opts => opts.Excluding(x => x.ViewerId).Excluding(x => x.Owner)
            );
    }
}
