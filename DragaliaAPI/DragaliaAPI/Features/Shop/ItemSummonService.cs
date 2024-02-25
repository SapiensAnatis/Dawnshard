using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Services.Exceptions;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Shop;

public class ItemSummonService : IItemSummonService
{
    private readonly ItemSummonConfig config;
    private readonly ILogger<ItemSummonService> logger;
    private readonly IShopRepository shopRepository;
    private readonly IPaymentService paymentService;
    private readonly IRewardService rewardService;
    private readonly IMissionProgressionService missionProgressionService;
    private readonly Random random;

    private readonly int[] summonWeights;

    public ItemSummonService(
        ILogger<ItemSummonService> logger,
        IOptionsMonitor<ItemSummonConfig> odds,
        IShopRepository shopRepository,
        IPaymentService paymentService,
        IRewardService rewardService,
        IMissionProgressionService missionProgressionService
    )
    {
        this.logger = logger;
        this.config = odds.CurrentValue;
        this.shopRepository = shopRepository;
        this.paymentService = paymentService;
        this.rewardService = rewardService;
        this.missionProgressionService = missionProgressionService;
        this.random = Random.Shared;

        this.summonWeights = new int[this.config.Odds.Count];

        int weight = 0;
        for (int i = 0; i < this.summonWeights.Length; i++)
        {
            this.summonWeights[i] = weight;
            weight += this.config.Odds[i].Rate;
        }
    }

    public async Task<AtgenUserItemSummon> GetItemSummon()
    {
        DbPlayerShopInfo info = await this.shopRepository.GetShopInfoAsync();
        return new(info.DailySummonCount, info.LastSummonTime);
    }

    public IEnumerable<AtgenItemSummonRateList> GetOdds()
    {
        return config.Odds.Select(x => new AtgenItemSummonRateList(
            x.Type,
            x.Id,
            x.Quantity,
            $"{x.Rate / 100000.0d:P}"
        ));
    }

    public async Task<IEnumerable<AtgenBuildEventRewardEntityList>> DoSummon(
        ShopItemSummonExecRequest request
    )
    {
        DbPlayerShopInfo info = await this.shopRepository.GetShopInfoAsync();

        int expectedPrice = info.DailySummonCount switch
        {
            0 => 0,
            1 => 30,
            2 => 50,
            3 => 100,
            4 => 200,
            5 => 300,
            _ => throw new DragaliaException(ResultCode.ItemSummonExecCountOver, "Too many summons")
        };

        await this.paymentService.ProcessPayment(
            request.PaymentType,
            request.PaymentTarget,
            expectedPrice
        );

        info.DailySummonCount++;
        info.LastSummonTime = DateTimeOffset.UtcNow;

        AtgenBuildEventRewardEntityList[] results = new AtgenBuildEventRewardEntityList[10];
        for (int i = 0; i < results.Length; i++)
        {
            int value = random.Next(100001);
            int index = Array.IndexOf(this.summonWeights, this.summonWeights.Last(x => x <= value));
            ItemSummonOddsEntry entity = this.config.Odds[index];
            await this.rewardService.GrantReward(new(entity.Type, entity.Id, entity.Quantity));
            results[i] = new AtgenBuildEventRewardEntityList(
                entity.Type,
                entity.Id,
                entity.Quantity
            );
        }

        this.missionProgressionService.OnItemSummon();

        this.logger.LogDebug("Item summon results: {@itemSummonResults}", (object)results);

        return results;
    }
}
