using MessagePack.Resolvers;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using DragaliaAPI.Database;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.RedoableSummonController"/>
/// </summary>
[Collection("DragaliaIntegration")]
public class RedoableSummonTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;

    public RedoableSummonTest(IntegrationTestFixture fixture)
    {
        this.fixture = fixture;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task RedoableSummonGetData_ReturnsData()
    {
        RedoableSummonGetDataData response = (
            await client.PostMsgpack<RedoableSummonGetDataData>(
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
            await client.PostMsgpack<RedoableSummonPreExecData>(
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
        await client.PostAsync("redoable_summon/pre_exec", TestUtils.CreateMsgpackContent(new { }));

        RedoableSummonFixExecData response = (
            await client.PostMsgpack<RedoableSummonFixExecData>(
                "redoable_summon/fix_exec",
                new RedoableSummonFixExecRequest()
            )
        ).data;

        IEnumerable<int> newCharaIds = response.update_data_list.chara_list!
            .Select(x => (int)x.chara_id)
            .OrderBy(x => x);

        IEnumerable<int> newDragonIds = response.update_data_list.dragon_list!
            .Select(x => (int)x.dragon_id)
            .OrderBy(x => x);

        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();

        IEnumerable<int> dbCharaIds = apiContext.PlayerCharaData
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .Select(x => (int)x.CharaId)
            .OrderBy(x => x);
        IEnumerable<int> dbDragonIds = apiContext.PlayerDragonData
            .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
            .Select(x => (int)x.DragonId)
            .OrderBy(x => x);

        newCharaIds.Should().BeSubsetOf(dbCharaIds);
        dbDragonIds.Should().BeSubsetOf(dbDragonIds);
    }
}
