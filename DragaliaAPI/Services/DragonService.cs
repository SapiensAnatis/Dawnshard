using System.Collections.Immutable;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

public class DragonService : IDragonService
{
    private readonly IUserDataRepository userDataRepository;
    private readonly IUnitRepository unitRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IStoryRepository storyRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IMapper mapper;

    public DragonService(
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService,
        IStoryRepository storyRepository,
        IMapper mapper
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.inventoryRepository = inventoryRepository;
        this.storyRepository = storyRepository;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
    }

    public async Task<DragonGetContactDataData> DoDragonGetContactData(
        DragonGetContactDataRequest request,
        string deviceAccountId
    )
    {
        DragonGifts rotatingGift = DragonConstants.rotatingGifts[
            (int)DateTimeOffset.UtcNow.DayOfWeek
        ];
        Dictionary<DragonGifts, DbPlayerDragonGift> gifts = await inventoryRepository
            .GetDragonGifts(deviceAccountId)
            .Where(
                x =>
                    x.DragonGiftId == DragonGifts.FreshBread
                    || x.DragonGiftId == DragonGifts.TastyMilk
                    || x.DragonGiftId == DragonGifts.StrawberryTart
                    || x.DragonGiftId == DragonGifts.HeartyStew
                    || x.DragonGiftId == rotatingGift
            )
            .ToDictionaryAsync(x => x.DragonGiftId);
        IEnumerable<AtgenShopGiftList> giftList = gifts.Values.Select(
            x =>
                new AtgenShopGiftList()
                {
                    dragon_gift_id = (int)x.DragonGiftId,
                    price = DragonConstants.buyGiftPrices.TryGetValue(
                        x.DragonGiftId,
                        out int giftPrice
                    )
                        ? giftPrice
                        : 0,
                    is_buy = x.Quantity
                }
        );
        return new DragonGetContactDataData(giftList);
    }

    private async Task<
        IEnumerable<Tuple<DragonGifts, List<RewardReliabilityList>>>
    > IncreaseDragonReliability(
        DbPlayerDragonReliability dragonReliability,
        IEnumerable<Tuple<DragonGifts, int>> giftsAndQuantity,
        string deviceAccountId
    )
    {
        List<Tuple<DragonGifts, List<RewardReliabilityList>>> levelGifts =
            new List<Tuple<DragonGifts, List<RewardReliabilityList>>>();
        int levelRewardIndex = (dragonReliability.Level / 5);
        DragonData dragonData = MasterAsset.DragonData.Get(dragonReliability.DragonId);
        IEnumerator<Tuple<DragonGifts, int>> enumerator = giftsAndQuantity.GetEnumerator();
        ImmutableArray<int> bondXpLimits =
            dragonReliability.DragonId == Dragons.Puppy
                ? DragonConstants.bondXpLimitsPuppy
                : DragonConstants.bondXpLimits;
        while (enumerator.MoveNext() && dragonReliability.Exp < bondXpLimits[^1])
        {
            dragonReliability.Exp = Math.Min(
                bondXpLimits[^1],
                dragonReliability.Exp
                    + (int)(
                        DragonConstants.favorVals[enumerator.Current.Item1]
                        * (
                            dragonData.FavoriteType != null
                            && DragonConstants.rotatingGifts[(int)dragonData.FavoriteType]
                                == enumerator.Current.Item1
                                ? DragonConstants.favMulti
                                : 1
                        )
                        * enumerator.Current.Item2
                    )
            );
            List<RewardReliabilityList> returnGifts = new List<RewardReliabilityList>();
            while (
                !(
                    dragonReliability.Level == DragonConstants.maxRelLevel
                    || bondXpLimits[dragonReliability.Level] > dragonReliability.Exp
                )
            )
            {
                dragonReliability.Level++;
                if (dragonReliability.Level / 5 > levelRewardIndex)
                {
                    RewardReliabilityList? reward = await GetRewardDataForLevel(
                        dragonReliability.Level,
                        dragonData.Rarity,
                        MasterAsset.DragonStories.Get((int)dragonReliability.DragonId).storyIds,
                        dragonReliability.DragonId == Dragons.Puppy,
                        deviceAccountId
                    );
                    if (reward != null)
                    {
                        returnGifts.Add(reward);
                    }
                    levelRewardIndex++;
                }
            }
            levelGifts.Add(
                new Tuple<DragonGifts, List<RewardReliabilityList>>(
                    enumerator.Current.Item1,
                    returnGifts
                )
            );
        }
        return levelGifts;
    }

    private static readonly ImmutableArray<Materials> dragonLevelReward = new Materials[]
    {
        Materials.Omnicite,
        Materials.Talonstone,
        Materials.Omnicite,
        Materials.SucculentDragonfruit,
        Materials.Talonstone,
        Materials.SunlightOre
    }.ToImmutableArray();

    private static readonly int[][] dragonLevelRewardQuantity = new int[][]
    {
        new int[] { 999999, 3, 999999, 2, 5, 1 },
        new int[] { 999999, 4, 999999, 3, 7, 1 },
        new int[] { 999999, 5, 999999, 4, 10, 1 }
    };

    private static readonly ImmutableArray<Materials> puppyLevelReward = new Materials[]
    {
        Materials.LightOrb,
        Materials.HalfEatenBread,
        Materials.HalfEatenBread,
        Materials.HalfEatenBread,
        Materials.HalfEatenBread,
        Materials.ImitationSquish
    }.ToImmutableArray();

    private static readonly int[] puppyLevelRewardQuantity = new int[] { 1, 21, 28, 34, 40, 1 };

    private async Task<RewardReliabilityList?> GetRewardDataForLevel(
        int level,
        int rarity,
        int[] dragonStories,
        bool isPuppy,
        string deviceAccountId
    )
    {
        RewardReliabilityList? reward = null;
        if (level > 4 && level % 5 == 0)
        {
            reward = new RewardReliabilityList()
            {
                level = level,
                levelup_entity_list = new DragonRewardEntityList[1]
            };
            int levelIndex = level / 5;
            ImmutableArray<Materials> rewardMats = isPuppy ? puppyLevelReward : dragonLevelReward;
            int[] rewardQuantity = isPuppy
                ? puppyLevelRewardQuantity
                : dragonLevelRewardQuantity[rarity - 3];
            if (!isPuppy)
            {
                if (levelIndex == 1 || levelIndex == 3)
                {
                    int nextStoryUnlockIndex = await storyRepository
                        .GetStoryList(deviceAccountId)
                        .Where(x => dragonStories.Contains(x.StoryId))
                        .CountAsync();
                    if (nextStoryUnlockIndex > dragonStories.Length - 1)
                    {
                        throw new DragaliaException(
                            ResultCode.StoryCountNotEnough,
                            "Too many story unlocks"
                        );
                    }
                    await storyRepository.GetOrCreateStory(
                        deviceAccountId,
                        StoryTypes.Dragon,
                        dragonStories[nextStoryUnlockIndex]
                    );
                    reward.is_release_story = 1;
                    if (levelIndex == 3)
                    {
                        //TODO: Add Epithet to account
                    }
                    return reward;
                }
                if (levelIndex == 6)
                {
                    //TODO: Add Notte's Notes Bonus
                }
            }
            DragonRewardEntityList rewardItem = new DragonRewardEntityList()
            {
                entity_type = EntityTypes.Material,
                entity_id = (int)rewardMats[levelIndex - 1],
                entity_quantity = rewardQuantity[levelIndex - 1],
                is_over = 0
            };
            DbPlayerMaterial mat =
                await inventoryRepository.GetMaterial(
                    deviceAccountId,
                    (Materials)rewardItem.entity_id
                )
                ?? inventoryRepository.AddMaterial(
                    deviceAccountId,
                    (Materials)rewardItem.entity_id
                );
            mat.Quantity += rewardItem.entity_quantity;
            ((DragonRewardEntityList[])reward.levelup_entity_list)[0] = rewardItem;
        }
        return reward;
    }

    private async Task<
        IEnumerable<Tuple<DragonGifts, DragonRewardEntityList>>
    > RollPuppyThankYouRewards(IEnumerable<DragonGifts> gifts, string deviceAccountId)
    {
        List<Tuple<DragonGifts, DragonRewardEntityList>> giftPerGift =
            new List<Tuple<DragonGifts, DragonRewardEntityList>>();
        Materials[] rewards = new Materials[]
        {
            Materials.FiendsClaw,
            Materials.FiendsEye,
            Materials.Granite,
            Materials.OldCloth,
            Materials.SolidFungus
        };
        int rewardQuantity = 5;
        foreach (DragonGifts gift in gifts)
        {
            int rIndex = new Random().Next(rewards.Length);
            (
                await inventoryRepository.GetMaterial(deviceAccountId, rewards[rIndex])
                ?? inventoryRepository.AddMaterial(deviceAccountId, rewards[rIndex])
            ).Quantity += rewardQuantity;
            DragonRewardEntityList reward = new DragonRewardEntityList()
            {
                entity_type = EntityTypes.Material,
                entity_id = (int)rewards[rIndex],
                entity_quantity = rewardQuantity,
                is_over = 0
            };
            giftPerGift.Add(new(gift, reward));
        }
        return giftPerGift;
    }

    private async Task<
        IEnumerable<Tuple<DragonGifts, List<DragonRewardEntityList>>>
    > RollDragonThankYouRewards(
        IEnumerable<Tuple<DragonGifts, int>> gifts,
        UnitElement dragonElement,
        string deviceAccountId
    )
    {
        Dictionary<UnitElement, Materials>[] orbs = new Dictionary<UnitElement, Materials>[]
        {
            UpgradeMaterials.t1Orbs,
            UpgradeMaterials.t2Orbs,
            UpgradeMaterials.t3Orbs
        };
        int[][] rarityQuantities = new int[][]
        {
            new int[] { 3, 4, 5 },
            new int[] { 1, 2, 3 },
            new int[] { 1 }
        };
        Materials[] fruits = new Materials[]
        {
            Materials.Dragonfruit,
            Materials.RipeDragonfruit,
            Materials.SucculentDragonfruit
        };
        List<Tuple<DragonGifts, List<DragonRewardEntityList>>> giftsPerGift =
            new List<Tuple<DragonGifts, List<DragonRewardEntityList>>>();
        Random r = new Random();
        foreach (Tuple<DragonGifts, int> gift in gifts)
        {
            List<DragonRewardEntityList> tyGiftsPerItem = new List<DragonRewardEntityList>();
            for (int i = 0; i < gift.Item2; i++)
            {
                List<DragonRewardEntityList> tyGifts = new List<DragonRewardEntityList>();
                while (r.NextDouble() < 1 - (tyGifts.Count * 0.34))
                {
                    DragonRewardEntityList reward = new DragonRewardEntityList();
                    switch (r.Next(4))
                    {
                        case 0:
                            reward.entity_type = EntityTypes.Mana;
                            reward.entity_id = 0;
                            reward.entity_quantity =
                                rarityQuantities[1][r.Next(rarityQuantities[1].Length)] * 500;
                            (
                                await userDataRepository.GetUserData(deviceAccountId).SingleAsync()
                            ).ManaPoint += reward.entity_quantity;
                            break;
                        case 1:
                            int rIndex = r.Next(fruits.Length);
                            Materials fruitMat = fruits[rIndex];
                            reward.entity_type = EntityTypes.Material;
                            reward.entity_id = (int)fruitMat;
                            reward.entity_quantity = rarityQuantities[rIndex][
                                r.Next(rarityQuantities[rIndex].Length)
                            ];
                            (
                                await inventoryRepository.GetMaterial(deviceAccountId, fruitMat)
                                ?? inventoryRepository.AddMaterial(deviceAccountId, fruitMat)
                            ).Quantity += reward.entity_quantity;
                            break;
                        case 2:
                            rIndex = r.Next(orbs.Length);
                            reward.entity_type = EntityTypes.Material;
                            reward.entity_id = (int)orbs[rIndex][dragonElement];
                            reward.entity_quantity = rarityQuantities[rIndex][
                                r.Next(rarityQuantities[rIndex].Length)
                            ];
                            (
                                await inventoryRepository.GetMaterial(
                                    deviceAccountId,
                                    (Materials)reward.entity_id
                                )
                                ?? inventoryRepository.AddMaterial(
                                    deviceAccountId,
                                    (Materials)reward.entity_id
                                )
                            ).Quantity += reward.entity_quantity;
                            break;
                        case 3:
                            reward.entity_type = EntityTypes.Material;
                            reward.entity_id = (int)Materials.Talonstone;
                            reward.entity_quantity = rarityQuantities[1][
                                r.Next(rarityQuantities[1].Length)
                            ];
                            (
                                await inventoryRepository.GetMaterial(
                                    deviceAccountId,
                                    Materials.Talonstone
                                )
                                ?? inventoryRepository.AddMaterial(
                                    deviceAccountId,
                                    Materials.Talonstone
                                )
                            ).Quantity += reward.entity_quantity;
                            break;
                    }
                    reward.is_over = 0;
                    tyGifts.Add(reward);
                }
                tyGiftsPerItem.AddRange(tyGifts);
            }
            giftsPerGift.Add(new(gift.Item1, tyGiftsPerItem));
        }
        return giftsPerGift;
    }

    public async Task<DragonBuyGiftToSendMultipleData> DoDragonBuyGiftToSendMultiple(
        DragonBuyGiftToSendMultipleRequest request,
        string deviceAccountId
    )
    {
        DbPlayerUserData userData = await userDataRepository
            .GetUserData(deviceAccountId)
            .FirstAsync();
        //DbPlayerCurrency rupies = await inventoryRepository.GetCurrency(deviceAccountId, CurrencyTypes.Rupies) ?? inventoryRepository.AddCurrency(deviceAccountId, CurrencyTypes.Rupies);

        int totalCost = request.dragon_gift_id_list
            .Select(x => DragonConstants.buyGiftPrices[x])
            .Sum();
        if (userData.Coin < totalCost)
        //if (rupies.Quantity < totalCost)
        {
            throw new DragaliaException(ResultCode.CommonMaterialShort, "Insufficient Rupies");
        }

        Dictionary<DragonGifts, DbPlayerDragonGift> gifts = await inventoryRepository
            .GetDragonGifts(deviceAccountId)
            .Where(x => request.dragon_gift_id_list.Contains(x.DragonGiftId))
            .ToDictionaryAsync(x => x.DragonGiftId);

        DbPlayerDragonReliability dragonReliability = await unitRepository
            .GetAllDragonReliabilityData(deviceAccountId)
            .Where(x => x.DragonId == request.dragon_id)
            .FirstAsync();

        if (dragonReliability == null)
        {
            throw new DragaliaException(Models.ResultCode.EntityNotFoundError);
        }

        DragonData dragonData = MasterAsset.DragonData.Get(dragonReliability.DragonId);

        IEnumerable<Tuple<DragonGifts, List<DragonRewardEntityList>>> rewards =
            dragonReliability.DragonId == Dragons.Puppy
                ? (
                    await RollPuppyThankYouRewards(request.dragon_gift_id_list, deviceAccountId)
                ).Select(
                    x =>
                        new Tuple<DragonGifts, List<DragonRewardEntityList>>(
                            x.Item1,
                            new List<DragonRewardEntityList>() { x.Item2 }
                        )
                )
                : await RollDragonThankYouRewards(
                    request.dragon_gift_id_list.Select(x => new Tuple<DragonGifts, int>(x, 1)),
                    dragonData.ElementalType,
                    deviceAccountId
                );
        ;

        IEnumerable<Tuple<DragonGifts, List<RewardReliabilityList>>> levelGifts =
            await IncreaseDragonReliability(
                dragonReliability,
                request.dragon_gift_id_list.Select(
                    x => new Tuple<DragonGifts, int>((DragonGifts)x, 1)
                ),
                deviceAccountId
            );

        List<AtgenDragonGiftRewardList> rewardObjList = new List<AtgenDragonGiftRewardList>();
        foreach (DragonGifts gift in request.dragon_gift_id_list)
        {
            rewardObjList.Add(
                new AtgenDragonGiftRewardList()
                {
                    return_gift_list = rewards.Where(x => x.Item1 == gift).First().Item2,
                    dragon_gift_id = gift,
                    is_favorite =
                        dragonData.FavoriteType != null
                        && DragonConstants.rotatingGifts[(int)dragonData.FavoriteType] == gift,
                    reward_reliability_list = levelGifts.Where(x => x.Item1 == gift).First().Item2
                }
            );
        }

        foreach (DbPlayerDragonGift gift in gifts.Values)
        {
            gift.Quantity -= 1;
        }

        userData.Coin -= totalCost;
        //rupies.Quantity -= totalCost;

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(deviceAccountId);

        await unitRepository.SaveChangesAsync();

        IEnumerable<AtgenShopGiftList> giftList = (
            await DoDragonGetContactData(new(), deviceAccountId)
        ).shop_gift_list;

        return new DragonBuyGiftToSendMultipleData()
        {
            dragon_contact_free_gift_count = 0,
            dragon_gift_reward_list = rewardObjList,
            entity_result = null,
            shop_gift_list = giftList,
            update_data_list = updateDataList
        };
    }

    public async Task<DragonSendGiftMultipleData> DoDragonSendGiftMultiple(
        DragonSendGiftMultipleRequest request,
        string deviceAccountId
    )
    {
        DbPlayerDragonGift? gift = await inventoryRepository
            .GetDragonGifts(deviceAccountId)
            .Where(x => x.DragonGiftId == request.dragon_gift_id)
            .FirstOrDefaultAsync();

        if (gift == null || gift.Quantity < request.quantity)
        {
            throw new DragaliaException(
                Models.ResultCode.CommonMaterialShort,
                $"Insufficient gift quantity for: {request.dragon_gift_id}"
            );
        }

        DbPlayerDragonReliability? dragonReliability = await unitRepository
            .GetAllDragonReliabilityData(deviceAccountId)
            .Where(x => x.DragonId == request.dragon_id)
            .FirstOrDefaultAsync();

        if (dragonReliability == null)
        {
            throw new DragaliaException(
                Models.ResultCode.EntityNotFoundError,
                $"No such dragon in inventory: {request.dragon_id}"
            );
        }

        List<Tuple<DragonGifts, int>> requestGift = new List<Tuple<DragonGifts, int>>()
        {
            Tuple.Create(request.dragon_gift_id, request.quantity)
        };

        DragonData dragonData = MasterAsset.DragonData.Get(dragonReliability.DragonId);

        IEnumerable<Tuple<DragonGifts, List<DragonRewardEntityList>>> rewards =
            dragonReliability.DragonId == Dragons.Puppy
                ? (
                    await RollPuppyThankYouRewards(
                        new List<DragonGifts>() { request.dragon_gift_id },
                        deviceAccountId
                    )
                ).Select(
                    x =>
                        new Tuple<DragonGifts, List<DragonRewardEntityList>>(
                            x.Item1,
                            new List<DragonRewardEntityList>() { x.Item2 }
                        )
                )
                : await RollDragonThankYouRewards(
                    requestGift,
                    dragonData.ElementalType,
                    deviceAccountId
                );
        ;

        IEnumerable<Tuple<DragonGifts, List<RewardReliabilityList>>> levelGifts =
            await IncreaseDragonReliability(dragonReliability, requestGift, deviceAccountId);

        IEnumerable<AtgenShopGiftList> giftList = (
            await DoDragonGetContactData(new(), deviceAccountId)
        ).shop_gift_list;

        gift.Quantity -= request.quantity;

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(deviceAccountId);

        await unitRepository.SaveChangesAsync();

        return new DragonSendGiftMultipleData()
        {
            is_favorite = true,
            reward_reliability_list = levelGifts.First().Item2,
            return_gift_list = rewards.First().Item2,
            update_data_list = updateDataList
        };
    }
}
