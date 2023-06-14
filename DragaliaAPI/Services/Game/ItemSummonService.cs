using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Services.Game;

public class ItemSummonService : IItemSummonService
{
    private readonly ILogger<ItemSummonService> logger;
    private readonly ItemSummonOdds odds;

    public ItemSummonService(ILogger<ItemSummonService> logger, IOptions<ItemSummonOdds> odds)
    {
        this.logger = logger;
        this.odds = odds.Value;
    }

    public IEnumerable<AtgenItemSummonRateList> GetOdds()
    {
        return odds.Odds.Select(
            x => new AtgenItemSummonRateList(x.Type, x.Id, x.Quantity, $"{x.Rate:P}")
        );
    }

    public async Task<List<AtgenBuildEventRewardEntityList>> DoSummon(
        ShopItemSummonExecRequest request
    )
    {
        logger.LogDebug("Tried to start item summon - this is not yet implemented.");
        throw new NotImplementedException();
    }
}
