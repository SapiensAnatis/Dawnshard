namespace DragaliaAPI.Integration.Test.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.RedoableSummonController"/>
/// </summary>
public class RedoableSummonTest : TestFixture
{
    public RedoableSummonTest(CustomWebApplicationFactory factory, ITestOutputHelper outputHelper)
        : base(factory, outputHelper) { }

    [Fact]
    public async Task RedoableSummonGetData_ReturnsData()
    {
        RedoableSummonGetDataData response = (
            await this.Client.PostMsgpack<RedoableSummonGetDataData>(
                "redoable_summon/get_data",
                new RedoableSummonGetDataRequest()
            )
        ).data;

        response.Should().NotBeNull();
    }

    [Fact]
    public async Task RedoableSummonPreExec_ReturnsValidResult()
    {
        RedoableSummonPreExecData response = (
            await this.Client.PostMsgpack<RedoableSummonPreExecData>(
                "redoable_summon/pre_exec",
                new RedoableSummonPreExecRequest(0)
            )
        ).data;

        response.user_redoable_summon_data.redoable_summon_result_unit_list.Count().Should().Be(50);
    }

    [Fact]
    public async Task RedoableSummonFixExec_UpdatesDatabase()
    {
        // Set up cached summon result
        await this.Client.PostMsgpack<RedoableSummonPreExecData>(
            "redoable_summon/pre_exec",
            new RedoableSummonPreExecRequest()
        );

        RedoableSummonFixExecData response = (
            await this.Client.PostMsgpack<RedoableSummonFixExecData>(
                "redoable_summon/fix_exec",
                new RedoableSummonFixExecRequest()
            )
        ).data;

        IEnumerable<int> newCharaIds = response
            .update_data_list.chara_list!.Select(x => (int)x.chara_id)
            .OrderBy(x => x);

        IEnumerable<int> newDragonIds = response
            .update_data_list.dragon_list!.Select(x => (int)x.dragon_id)
            .OrderBy(x => x);

        IEnumerable<int> dbCharaIds = this.ApiContext.PlayerCharaData.Where(
            x => x.ViewerId == ViewerId
        )
            .Select(x => (int)x.CharaId)
            .OrderBy(x => x);
        IEnumerable<int> dbDragonIds = this.ApiContext.PlayerDragonData.Where(
            x => x.ViewerId == ViewerId
        )
            .Select(x => (int)x.DragonId)
            .OrderBy(x => x);

        newCharaIds.Should().BeSubsetOf(dbCharaIds);
        dbDragonIds.Should().BeSubsetOf(dbDragonIds);
    }
}
