using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack.Resolvers;
using MessagePack;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using DragaliaAPI.Models.Data;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Xunit.Abstractions;
using Newtonsoft.Json;
using DragaliaAPI.Models.Data.Entity;
using DragaliaAPI.Models.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Models.Database.Savefile;
using System.Collections.Generic;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class SummonTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    public SummonTest(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
        _factory.SeedCache();
        _output = output;
    }

    [Fact]
    public async Task SummonExcludeGetList_ReturnsAnyData()
    {
        // Corresponds to JSON: "{}"
        byte[] payload = MessagePackSerializer.Serialize(1020203);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("summon_exclude/get_list", content);

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonExcludeGetListResponse data =
            MessagePackSerializer.Deserialize<SummonExcludeGetListResponse>(bytes);
        _output.WriteLine(TestUtils.MsgpackBytesToPrettyJson(bytes));
    }

    [Fact]
    public async Task SummonGetOddsData_ReturnsAnyData()
    {
        // Corresponds to JSON: "{}"
        byte[] payload = MessagePackSerializer.Serialize(1020203);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("summon/get_odds_data", content);

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonGetOddsDataResponse data =
            MessagePackSerializer.Deserialize<SummonGetOddsDataResponse>(bytes);
        _output.WriteLine(TestUtils.MsgpackBytesToPrettyJson(bytes));
    }

    [Fact]
    public async Task SummonGetSummonHistory_ReturnsAnyData()
    {
        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync(
            "summon/get_summon_history",
            content
        );

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonGetSummonHistoryResponse data =
            MessagePackSerializer.Deserialize<SummonGetSummonHistoryResponse>(bytes);
        _output.WriteLine(TestUtils.MsgpackBytesToPrettyJson(bytes));
    }

    [Fact]
    public async Task SummonGetSummonList_ReturnsAnyData()
    {
        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("summon/get_summon_list", content);

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonGetSummonListResponse data =
            MessagePackSerializer.Deserialize<SummonGetSummonListResponse>(bytes);
        _output.WriteLine(TestUtils.MsgpackBytesToPrettyJson(bytes));
    }

    [Fact]
    public async Task SummonRequest_GetSummonPointData_ReturnsAnyData()
    {
        HttpContent content = TestUtils.CreateMsgpackContent(
            MessagePackSerializer.Serialize(1020203)
        );

        HttpResponseMessage response = await _client.PostAsync(
            "summon/get_summon_point_trade",
            content
        );

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonGetSummonPointTradeResponse data =
            MessagePackSerializer.Deserialize<SummonGetSummonPointTradeResponse>(bytes);
        _output.WriteLine(TestUtils.MsgpackBytesToPrettyJson(bytes));
        _output.WriteLine(bytes.Length.ToString());
    }

    [Fact]
    public async Task SummonRequest_SingleSummonWyrmite_ReturnsValidResult()
    {
        SummonRequest request = new SummonRequest(
            1,
            SummonExecTypes.Single,
            1,
            PaymentTypes.Wyrmite,
            new PaymentTarget(1, 1)
        );
        SummonRequestResponseData data = await SummonRequest(request);
        data.result_unit_list.Count.Should().Be(1);
        _output.WriteLine(
            data.result_unit_list[0].entity_type == 7
                ? ((Dragons)data.result_unit_list[0].id).ToString()
                : ((Charas)data.result_unit_list[0].id).ToString()
        );
        using (IServiceScope scope = _factory.Services.CreateScope())
        {
            ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
            int size =
                data.result_unit_list[0].entity_type == 7
                    ? apiContext.PlayerDragonReliability.Count()
                        + apiContext.PlayerDragonData.Count()
                    : apiContext.PlayerCharaData.Count();
            _output.WriteLine(size.ToString());
        }
    }

    [Fact]
    public async Task SummonRequest_TenSummonWyrmite_ReturnsValidResult()
    {
        SummonRequest request = new SummonRequest(
            1020203,
            SummonExecTypes.Tenfold,
            0,
            PaymentTypes.Wyrmite,
            new PaymentTarget(1, 1)
        );
        SummonRequestResponseData data = await SummonRequest(request);
        data.result_unit_list.Count.Should().Be(10);
        _output.WriteLine("Summoned: ");
        foreach (SummonReward result in data.result_unit_list)
        {
            _output.WriteLine(
                result.entity_type == 7
                    ? ((Dragons)result.id).ToString()
                    : ((Charas)result.id).ToString()
            );
        }
        _output.WriteLine("\n");
        using (IServiceScope scope = _factory.Services.CreateScope())
        {
            ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();
            List<Charas> dbCharas = apiContext.PlayerCharaData.Select(x => x.CharaId).ToList();
            _output.WriteLine($"DB Charas:\n{string.Join(",\n", dbCharas)}\n");
            dbCharas.Should().Contain(data.update_data_list.chara_list!.Select(x => x.chara_id));
            List<Tuple<long, Dragons>> dbDragons = apiContext.PlayerDragonData
                .Select(x => new Tuple<long, Dragons>(x.DragonKeyId, x.DragonId))
                .ToList();
            _output.WriteLine($"DB Dragons:\n{string.Join(",\n", dbDragons)}\n");
            dbDragons
                .Should()
                .Contain(
                    data.update_data_list.dragon_list!.Select(
                        x => new Tuple<long, Dragons>((long)x.dragon_key_id, (Dragons)x.dragon_id)
                    )
                );
            List<Dragons> dbUniqueDragons = apiContext.PlayerDragonReliability
                .Select(x => x.DragonId)
                .ToList();
            _output.WriteLine($"DB Unique Dragons:\n{string.Join(",\n", dbUniqueDragons)}\n");
            dbUniqueDragons
                .Should()
                .Contain(data.update_data_list.dragon_reliability_list!.Select(x => x.dragon_id));
        }
    }

    protected async Task<SummonRequestResponseData> SummonRequest(SummonRequest request)
    {
        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await _client.PostAsync("summon/request", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
        SummonRequestResponse deserializedResponse =
            MessagePackSerializer.Deserialize<SummonRequestResponse>(
                responseBytes,
                ContractlessStandardResolver.Options
            );
        _output.WriteLine(TestUtils.MsgpackBytesToPrettyJson(responseBytes));
        return deserializedResponse.data;
    }
}
