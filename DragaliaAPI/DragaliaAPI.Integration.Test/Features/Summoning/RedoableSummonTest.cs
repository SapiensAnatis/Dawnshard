using DragaliaAPI.Features.Summoning;

namespace DragaliaAPI.Integration.Test.Features.Summoning;

/// <summary>
/// Tests <see cref="RedoableSummonController"/>
/// </summary>
public class RedoableSummonTest : TestFixture
{
    public RedoableSummonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task RedoableSummonGetData_ReturnsData()
    {
        RedoableSummonGetDataResponse response = (
            await this.Client.PostMsgpack<RedoableSummonGetDataResponse>("redoable_summon/get_data")
        ).Data;

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task RedoableSummonPreExec_ReturnsValidResult()
    {
        RedoableSummonPreExecResponse response = (
            await this.Client.PostMsgpack<RedoableSummonPreExecResponse>(
                "redoable_summon/pre_exec",
                new RedoableSummonPreExecRequest(0)
            )
        ).Data;

        response.UserRedoableSummonData.RedoableSummonResultUnitList.Count().Should().Be(50);
    }

    [Fact]
    public async Task RedoableSummonFixExec_UpdatesDatabase()
    {
        // Set up cached summon result
        await this.Client.PostMsgpack<RedoableSummonPreExecResponse>(
            "redoable_summon/pre_exec",
            new RedoableSummonPreExecRequest()
        );

        RedoableSummonFixExecResponse response = (
            await this.Client.PostMsgpack<RedoableSummonFixExecResponse>("redoable_summon/fix_exec")
        ).Data;

        IEnumerable<int> newCharaIds = response
            .UpdateDataList.CharaList!.Select(x => (int)x.CharaId)
            .OrderBy(x => x);

        IEnumerable<int> newDragonIds = response
            .UpdateDataList.DragonList!.Select(x => (int)x.DragonId)
            .OrderBy(x => x);

        IEnumerable<int> dbCharaIds = this
            .ApiContext.PlayerCharaData.Where(x => x.ViewerId == this.ViewerId)
            .Select(x => (int)x.CharaId)
            .OrderBy(x => x);
        IEnumerable<int> dbDragonIds = this
            .ApiContext.PlayerDragonData.Where(x => x.ViewerId == this.ViewerId)
            .Select(x => (int)x.DragonId)
            .OrderBy(x => x);

        newCharaIds.Should().BeSubsetOf(dbCharaIds);
        newDragonIds.Should().BeSubsetOf(dbDragonIds);
    }
}
