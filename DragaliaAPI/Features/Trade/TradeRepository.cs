using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
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

    public IQueryable<DbPlayerTrade> Trades =>
        this.apiContext.PlayerTrades.Where(
            x => x.DeviceAccountId == this.playerIdentityService.AccountId
        );

    public async Task<ILookup<TradeType, DbPlayerTrade>> GetAllTradesAsync()
    {
        return (await this.Trades.ToListAsync()).ToLookup(x => x.Type);
    }

    public async Task<IEnumerable<DbPlayerTrade>> GetTradesByTypeAsync(TradeType type)
    {
        return await this.Trades.Where(x => x.Type == type).ToListAsync();
    }

    public async Task<bool> AddTrade(TradeType type, int id, int count, DateTimeOffset? time = null)
    {
        if (type == TradeType.None)
            throw new ArgumentNullException(nameof(type));

        if (type == TradeType.Treasure && time == null)
            throw new ArgumentNullException(nameof(time));

        DateTimeOffset actualTime = time ?? DateTimeOffset.UnixEpoch;

        DbPlayerTrade? existing = await this.apiContext.PlayerTrades.FindAsync(
            this.playerIdentityService.AccountId,
            id
        );

        if (existing == null)
        {
            this.apiContext.PlayerTrades.Add(
                new DbPlayerTrade
                {
                    DeviceAccountId = this.playerIdentityService.AccountId,
                    Type = type,
                    Id = id,
                    Count = count,
                    LastTradeTime = actualTime
                }
            );

            return true;
        }

        existing.Count += count;
        existing.LastTradeTime = actualTime;

        return false;
    }
}
