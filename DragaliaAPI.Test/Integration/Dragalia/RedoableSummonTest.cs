using MessagePack.Resolvers;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Database;

namespace DragaliaAPI.Test.Integration.Dragalia;

/// <summary>
/// Tests <see cref="Controllers.Dragalia.RedoableSummonController"/>
/// </summary>
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
        RedoableSummonGetDataResponse expectedResponse =
            new(RedoableSummonGetDataFactory.CreateData());

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("redoable_summon/get_data", content);

        await TestUtils.CheckMsgpackResponse(response, expectedResponse);
    }

    [Fact]
    public async Task RedoableSummonPreExec_ReturnsValidResult()
    {
        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("redoable_summon/pre_exec", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
        RedoableSummonPreExecResponse deserializedResponse =
            MessagePackSerializer.Deserialize<RedoableSummonPreExecResponse>(
                responseBytes,
                ContractlessStandardResolver.Options
            );

        List<SimpleSummonReward> summonResult = deserializedResponse
            .data
            .user_redoable_summon_data
            .redoable_summon_result_unit_list;

        summonResult.Count.Should().Be(50);
    }

    [Fact]
    public async Task RedoableSummonFixExec_UpdatesDatabase()
    {
        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        // Set up cached summon result
        await client.PostAsync("redoable_summon/pre_exec", content);

        HttpResponseMessage fixResponse = await client.PostAsync(
            "redoable_summon/fix_exec",
            content
        );

        fixResponse.IsSuccessStatusCode.Should().BeTrue();

        byte[] fixBytes = await fixResponse.Content.ReadAsByteArrayAsync();

        var fixDeserialized = MessagePackSerializer.Deserialize<RedoableSummonPreExecResponse>(
            fixBytes,
            ContractlessStandardResolver.Options
        );

        IEnumerable<int> newCharaIds = fixDeserialized.data.update_data_list.chara_list!
            .Select(x => (int)x.chara_id)
            .OrderBy(x => x);

        IEnumerable<int> newDragonIds = fixDeserialized.data.update_data_list.dragon_list!
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
