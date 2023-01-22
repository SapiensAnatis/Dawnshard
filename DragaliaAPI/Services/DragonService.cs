using System.Collections.Immutable;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Nintendo;
using DragaliaAPI.Shared.Definitions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;

namespace DragaliaAPI.Services;

public class DragonService : IDragonService
{
    private readonly IDragonDataService _dragonDataService;
    private readonly IDragonService dragonService;
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
        IMapper mapper,
        IDragonDataService charaDataService
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.inventoryRepository = inventoryRepository;
        this.storyRepository = storyRepository;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
        _dragonDataService = charaDataService;
    }

    private async Task<DragonGetContactDataData> DoDragonGetContactData(
        DragonGetContactDataRequest request
    )
    {
        DragonGifts rotatingGift = DragonConstants.rotatingGifts[
            (int)DateTimeOffset.UtcNow.DayOfWeek
        ];
        Dictionary<DragonGifts, DbPlayerDragonGift> gifts = await inventoryRepository
            .GetDragonGifts(DeviceAccountId)
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
        IEnumerable<Tuple<DragonGifts, int>> giftsAndQuantity
    )
    {
        List<Tuple<DragonGifts, List<RewardReliabilityList>>> levelGifts =
            new List<Tuple<DragonGifts, List<RewardReliabilityList>>>();
        int levelRewardIndex = (dragonReliability.Level / 5) - 1;
        DataDragon dragonData = _dragonDataService.GetData(dragonReliability.DragonId);
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
                            && rotatingGifts[(int)dragonData.FavoriteType]
                                == enumerator.Current.Item1
                                ? favMulti
                                : 1
                        )
                        * enumerator.Current.Item2
                    )
            );
            List<RewardReliabilityList> returnGifts = new List<RewardReliabilityList>();
            while (
                !(bondXpLimits[dragonReliability.Level] > dragonReliability.Exp)
                || dragonReliability.Level == DragonConstants.maxRelLevel
            )
            {
                dragonReliability.Level++;
                if (!(dragonReliability.Level / 5 < levelRewardIndex))
                {
                    RewardReliabilityList? reward = await GetRewardDataForLevel(
                        dragonReliability.Level,
                        dragonData.Rarity,
                        _dragonDataService.GetStoryData(dragonReliability.DragonId),
                        dragonReliability.DragonId == Dragons.Puppy
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
        bool isPuppy
    )
    {
        RewardReliabilityList? reward = null;
        if (!(level > 4 && level % 5 > 0))
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
                : dragonLevelRewardQuantity[rarity];
            if (!isPuppy && (levelIndex == 1 || levelIndex == 3))
            {
                int nextStoryUnlockIndex = await storyRepository
                    .GetStoryList(DeviceAccountId)
                    .Where(x => dragonStories.Contains(x.StoryId))
                    .CountAsync();
                if (dragonStories.Length - 1 > nextStoryUnlockIndex)
                {
                    throw new ArgumentException("Too many story unlocks");
                }
                await storyRepository.GetOrCreateStory(
                    DeviceAccountId,
                    StoryTypes.Dragon,
                    dragonStories[nextStoryUnlockIndex]
                );
                reward.is_release_story = 1;
                return reward;
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
                    DeviceAccountId,
                    (Materials)rewardItem.entity_id
                )
                ?? inventoryRepository.AddMaterial(
                    DeviceAccountId,
                    (Materials)rewardItem.entity_id
                );
            mat.Quantity += rewardItem.entity_quantity;
            ((DragonRewardEntityList[])reward.levelup_entity_list)[0] = rewardItem;
        }
        return reward;
    }

    private async Task<
        IEnumerable<Tuple<DragonGifts, DragonRewardEntityList>>
    > RollPuppyThankYouRewards(IEnumerable<DragonGifts> gifts)
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
                await inventoryRepository.GetMaterial(DeviceAccountId, rewards[rIndex])
                ?? inventoryRepository.AddMaterial(DeviceAccountId, rewards[rIndex])
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
        UnitElement dragonElement
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
            List<DragonRewardEntityList> tyGifts = new List<DragonRewardEntityList>();
            for (int i = 0; i < gift.Item2; i++)
            {
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
                                await userDataRepository.GetUserData(DeviceAccountId).SingleAsync()
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
                                await inventoryRepository.GetMaterial(DeviceAccountId, fruitMat)
                                ?? inventoryRepository.AddMaterial(DeviceAccountId, fruitMat)
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
                                    DeviceAccountId,
                                    (Materials)reward.entity_id
                                )
                                ?? inventoryRepository.AddMaterial(
                                    DeviceAccountId,
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
                                    DeviceAccountId,
                                    Materials.Talonstone
                                )
                                ?? inventoryRepository.AddMaterial(
                                    DeviceAccountId,
                                    Materials.Talonstone
                                )
                            ).Quantity += reward.entity_quantity;
                            break;
                    }
                    reward.is_over = 0;
                    tyGifts.Add(reward);
                }
            }
            giftsPerGift.Add(new(gift.Item1, tyGifts));
        }
        return giftsPerGift;
    }

    private async Task<DragonBuyGiftToSendMultipleData> DoDragonBuyGiftToSendMultiple(
        DragonBuyGiftToSendMultipleRequest request
    )
    {
        Dictionary<DragonGifts, DbPlayerDragonGift> gifts = await inventoryRepository
            .GetDragonGifts(DeviceAccountId)
            .Where(x => request.dragon_gift_id_list.Contains(x.DragonGiftId))
            .ToDictionaryAsync(x => x.DragonGiftId);

        DbPlayerDragonReliability dragonReliability = await unitRepository
            .GetAllDragonReliabilityData(DeviceAccountId)
            .Where(x => x.DragonId == request.dragon_id)
            .FirstAsync();

        DataDragon dragonData = _dragonDataService.GetData(dragonReliability.DragonId);

        IEnumerable<Tuple<DragonGifts, List<DragonRewardEntityList>>> rewards =
            dragonReliability.DragonId == Dragons.Puppy
                ? (await RollPuppyThankYouRewards(request.dragon_gift_id_list)).Select(
                    x =>
                        new Tuple<DragonGifts, List<DragonRewardEntityList>>(
                            x.Item1,
                            new List<DragonRewardEntityList>() { x.Item2 }
                        )
                )
                : await RollDragonThankYouRewards(
                    request.dragon_gift_id_list.Select(x => new Tuple<DragonGifts, int>(x, 1)),
                    dragonData.ElementalType
                );
        ;

        IEnumerable<Tuple<DragonGifts, List<RewardReliabilityList>>> levelGifts =
            await IncreaseDragonReliability(
                dragonReliability,
                request.dragon_gift_id_list.Select(
                    x => new Tuple<DragonGifts, int>((DragonGifts)x, 1)
                )
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
                        && rotatingGifts[(int)dragonData.FavoriteType] == gift,
                    reward_reliability_list = levelGifts
                        .Where(x => x.Item1 == gift)
                        .FirstOrDefault()
                        .Item2
                }
            );
        }

        IEnumerable<AtgenShopGiftList> giftList = (
            await DoDragonGetContactData(new())
        ).shop_gift_list;
        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(DeviceAccountId);

        await unitRepository.SaveChangesAsync();

        return new DragonBuyGiftToSendMultipleData()
        {
            dragon_contact_free_gift_count = 0,
            dragon_gift_reward_list = rewardObjList,
            entity_result = null,
            shop_gift_list = giftList,
            update_data_list = updateDataList
        };
    }
}
