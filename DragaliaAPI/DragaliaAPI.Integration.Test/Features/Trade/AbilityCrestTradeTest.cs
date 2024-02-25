using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Integration.Test.Features.Trade;

/// <summary>
/// Tests <see cref="DragaliaAPI.Features.Trade.AbilityCrestTradeController"/>
/// </summary>
public class AbilityCrestTradeTest : TestFixture
{
    public AbilityCrestTradeTest(CustomWebApplicationFactory factory, ITestOutputHelper output)
        : base(factory, output) { }

    [Theory]
    [InlineData(104, AbilityCrests.WorthyRivals, 4000)]
    [InlineData(2902, AbilityCrests.HisCleverBrother, 2000)]
    [InlineData(167, AbilityCrests.DragonsNest, 200)]
    public async Task Trade_AddsAbilityCrestAndDecreasesDewpoint(
        int trade_id,
        AbilityCrests expected_crest_id,
        int expected_dewpoint_cost
    )
    {
        int old_dewpoint = GetDewpoint();

        AbilityCrestTradeTradeData data = (
            await Client.PostMsgpack<AbilityCrestTradeTradeResponse>(
                "ability_crest_trade/trade",
                new AbilityCrestTradeTradeRequest()
                {
                    AbilityCrestTradeId = trade_id,
                    TradeCount = 1
                }
            )
        ).data;

        AbilityCrests ability_crest_id = data
            .UpdateDataList.AbilityCrestList.First()
            .AbilityCrestId;
        int dewpoint = data.UpdateDataList.UserData.DewPoint;

        ability_crest_id.Should().Be(expected_crest_id);
        dewpoint.Should().Be(old_dewpoint - expected_dewpoint_cost);
    }

    private int GetDewpoint()
    {
        return ApiContext
            .PlayerUserData.AsNoTracking()
            .Where(x => x.ViewerId == ViewerId)
            .Select(x => x.DewPoint)
            .First();
    }
}
