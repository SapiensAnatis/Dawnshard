using DragaliaAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Dragalia;

public class QuestOpenTreasureTest : TestFixture
{
    public QuestOpenTreasureTest(
        CustomWebApplicationFactory factory,
        ITestOutputHelper outputHelper
    )
        : base(factory, outputHelper) { }

    [Fact]
    public async Task OpenTreasure_ReturnCorrectResponse()
    {
        QuestOpenTreasureData response = (
            await this.Client.PostMsgpack<QuestOpenTreasureData>(
                "/quest/open_treasure",
                new QuestOpenTreasureRequest() { QuestTreasureId = 104101 }
            )
        ).data;

        response
            .UpdateDataList.QuestTreasureList.Should()
            .ContainEquivalentOf(new QuestTreasureList() { QuestTreasureId = 104101 });
    }

    [Fact]
    public async Task OpenTreasure_UpdatesDatabase()
    {
        QuestOpenTreasureData response = (
            await this.Client.PostMsgpack<QuestOpenTreasureData>(
                "/quest/open_treasure",
                new QuestOpenTreasureRequest() { QuestTreasureId = 126201 }
            )
        ).data;

        List<DbQuestTreasureList> questTreasureList = await this
            .ApiContext.QuestTreasureList.Where(x => x.ViewerId == ViewerId)
            .ToListAsync();

        questTreasureList.Should().Contain(x => x.QuestTreasureId == 126201);
    }
}
