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
        int tradeId,
        AbilityCrests expectedCrestId,
        int expectedDewpointCost
    )
    {
        int oldDewpoint = GetDewpoint();

        AbilityCrestTradeTradeResponse data = (
            await Client.PostMsgpack<AbilityCrestTradeTradeResponse>(
                "ability_crest_trade/trade",
                new AbilityCrestTradeTradeRequest()
                {
                    AbilityCrestTradeId = tradeId,
                    TradeCount = 1
                }
            )
        ).Data;

        AbilityCrests abilityCrestId;
        abilityCrestId = data.UpdateDataList.AbilityCrestList!.First().AbilityCrestId;
        int dewpoint = data.UpdateDataList.UserData.DewPoint;

        abilityCrestId.Should().Be(expectedCrestId);
        dewpoint.Should().Be(oldDewpoint - expectedDewpointCost);
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
