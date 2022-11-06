using MessagePack.Resolvers;
using MessagePack;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DragaliaAPI.Models.Requests;
using DragaliaAPI.Models.Responses;
using DragaliaAPI.Models.Components;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Test.Integration.Dragalia;

public class SummonTest : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient client;
    private readonly IntegrationTestFixture fixture;
    private readonly ITestOutputHelper outputHelper;

    public SummonTest(IntegrationTestFixture fixture, ITestOutputHelper outputHelper)
    {
        this.fixture = fixture;
        this.outputHelper = outputHelper;
        client = fixture.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );
    }

    [Fact]
    public async Task SummonExcludeGetList_ReturnsAnyData()
    {
        byte[] payload = MessagePackSerializer.Serialize(new BannerIdRequest(1020203));
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/summon_exclude/get_list", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonExcludeGetListResponse deserialized =
            MessagePackSerializer.Deserialize<SummonExcludeGetListResponse>(bytes);

        deserialized.data.summon_exclude_unit_list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonGetOddsData_ReturnsAnyData()
    {
        byte[] payload = MessagePackSerializer.Serialize(new BannerIdRequest(1020203));
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("/summon/get_odds_data", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonGetOddsDataResponse deserialized =
            MessagePackSerializer.Deserialize<SummonGetOddsDataResponse>(bytes);

        deserialized.data.Should().NotBeNull();
    }

    [Fact]
    public async Task SummonGetSummonHistory_ReturnsAnyData()
    {
        DbPlayerSummonHistory historyEntry =
            new()
            {
                DeviceAccountId = fixture.DeviceAccountId,
                SummonId = 1,
                SummonExecType = SummonExecTypes.DailyDeal,
                ExecDate = DateTimeOffset.UtcNow,
                PaymentType = PaymentTypes.Diamantium,
                EntityType = EntityTypes.Dragon,
                EntityId = (int)Dragons.GalaRebornNidhogg,
                EntityQuantity = 1,
                EntityLevel = 1,
                EntityRarity = 5,
                EntityLimitBreakCount = 0,
                EntityHpPlusCount = 0,
                EntityAttackPlusCount = 0,
                SummonPrizeRank = SummonPrizeRanks.None,
                SummonPoint = 10,
                GetDewPointQuantity = 0,
            };

        using (IServiceScope scope = fixture.Services.CreateScope())
        {
            ApiContext context = scope.ServiceProvider.GetRequiredService<ApiContext>();

            await context.PlayerSummonHistory.AddAsync(historyEntry);
            await context.SaveChangesAsync();
        }

        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("summon/get_summon_history", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonGetSummonHistoryResponse deserialized =
            MessagePackSerializer.Deserialize<SummonGetSummonHistoryResponse>(bytes);

        // Too lazy to set up automapper to check exact result and it is covered more or less in SummonRepositoryTests.cs
        deserialized.data.summon_history_list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonGetSummonList_ReturnsAnyData()
    {
        // Corresponds to JSON: "{}"
        byte[] payload = new byte[] { 0x80 };
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("summon/get_summon_list", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonGetSummonListResponse deserialized =
            MessagePackSerializer.Deserialize<SummonGetSummonListResponse>(bytes);

        deserialized.data.Should().NotBeNull();
    }

    [Fact]
    public async Task SummonRequest_GetSummonPointData_ReturnsAnyData()
    {
        byte[] payload = MessagePackSerializer.Serialize(new BannerIdRequest(1020203));
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync(
            "summon/get_summon_point_trade",
            content
        );

        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        SummonGetSummonPointTradeResponse deserialized =
            MessagePackSerializer.Deserialize<SummonGetSummonPointTradeResponse>(bytes);

        deserialized.data.summon_point_list.Should().NotBeEmpty();
        deserialized.data.summon_point_trade_list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SummonRequest_SingleSummonWyrmite_ReturnsValidResult()
    {
        SummonRequest request =
            new(1, SummonExecTypes.Single, 1, PaymentTypes.Wyrmite, new PaymentTarget(1, 1));

        SummonRequestResponseData data = await this.SummonRequest(request);

        data.result_unit_list.Count().Should().Be(1);

        await this.CheckRewardInDb(data.result_unit_list.ElementAt(0));
    }

    /// Multisummon tests fail on testDB when saving 2+ new dragonData because sqlLite can't generate new Dragon_Key_Ids (always returns 0) via sequence
    /// TODO: Low priority since it works with the actual DB, but maybe figure out how to change the generation so it works in tests too
    [Fact]
    public async Task SummonRequest_TenSummonWyrmite_ReturnsValidResult()
    {
        SummonRequest request =
            new(1020203, SummonExecTypes.Tenfold, 0, PaymentTypes.Wyrmite, new PaymentTarget(1, 1));

        SummonRequestResponseData data = await this.SummonRequest(request);

        data.result_unit_list.Count().Should().Be(10);

        foreach (SummonReward reward in data.result_unit_list)
        {
            await this.CheckRewardInDb(reward);
        }
    }

    private async Task<SummonRequestResponseData> SummonRequest(SummonRequest request)
    {
        byte[] payload = MessagePackSerializer.Serialize(request);
        HttpContent content = TestUtils.CreateMsgpackContent(payload);

        HttpResponseMessage response = await client.PostAsync("summon/request", content);

        response.IsSuccessStatusCode.Should().BeTrue();

        byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();
        SummonRequestResponse deserializedResponse =
            MessagePackSerializer.Deserialize<SummonRequestResponse>(
                responseBytes,
                ContractlessStandardResolver.Options
            );

        return deserializedResponse.data;
    }

    private async Task CheckRewardInDb(SummonReward reward)
    {
        using IServiceScope scope = fixture.Services.CreateScope();
        ApiContext apiContext = scope.ServiceProvider.GetRequiredService<ApiContext>();

        if (reward.entity_type == (int)EntityTypes.Dragon)
        {
            List<DbPlayerDragonData> dragonData = await apiContext.PlayerDragonData
                .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
                .ToListAsync();

            dragonData.Where(x => (int)x.DragonId == reward.id).Should().NotBeEmpty();
        }
        else
        {
            List<DbPlayerCharaData> charaData = await apiContext.PlayerCharaData
                .Where(x => x.DeviceAccountId == fixture.DeviceAccountId)
                .ToListAsync();

            charaData.Where(x => (int)x.CharaId == reward.id).Should().NotBeEmpty();
        }
    }
}
