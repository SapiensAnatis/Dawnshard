using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Trade;

public class TradeRepository : ITradeRepository
{
    private readonly ApiContext apiContext;
    private readonly IPlayerIdentityService playerIdentityService;

    public TradeRepository(ApiContext apiContext, IPlayerIdentityService playerIdentityService)
    {
        this.apiContext = apiContext;
        this.playerIdentityService = playerIdentityService;
    }

    public IQueryable<DbPlayerTreasureTrade> TreasureTrades =>
        this.apiContext.PlayerTreasureTrades.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public async Task<bool> AddTrade(int id, int count, DateTimeOffset time)
    {
        DbPlayerTreasureTrade? existing = await this.TreasureTrades.FirstOrDefaultAsync(
            x => x.Id == id
        );

        if (existing == null)
        {
            this.apiContext.PlayerTreasureTrades.Add(
                new DbPlayerTreasureTrade()
                {
                    DeviceAccountId = this.playerIdentityService.AccountId,
                    Id = id,
                    Count = count,
                    LastTradeTime = time
                }
            );

            return true;
        }
        else
        {
            existing.Count += count;
            existing.LastTradeTime = time;
        }

        return false;
    }
}
