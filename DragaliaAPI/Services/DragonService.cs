using System.Collections.Immutable;
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
    private readonly ILogger<DragonService> logger;
    private readonly IUpdateDataService updateDataService;

    public DragonService(
        IUserDataRepository userDataRepository,
        IUnitRepository unitRepository,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService,
        IStoryRepository storyRepository,
        ILogger<DragonService> logger
    )
    {
        this.userDataRepository = userDataRepository;
        this.unitRepository = unitRepository;
        this.inventoryRepository = inventoryRepository;
        this.storyRepository = storyRepository;
        this.logger = logger;
        this.updateDataService = updateDataService;
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
            .OrderBy(x => x.DragonGiftId)
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
                            DragonConstants.rotatingGifts[(int)dragonData.FavoriteType]
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
                        dragonReliability.DragonId == Dragons.Puppy
                            ? Array.Empty<int>()
                            : MasterAsset.DragonStories
                                .Get((int)dragonReliability.DragonId)
                                .storyIds,
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
                    return reward;
                }
                if (levelIndex == 6)
                {
                    //TODO: Add Notte's Nottes Bonus
                }
            }
            DragonRewardEntityList rewardItem = new DragonRewardEntityList()
            {
                entity_type = EntityTypes.Material,
                entity_id = (int)rewardMats[levelIndex - 1],
                entity_quantity = rewardQuantity[levelIndex - 1],
                //TODO: check for mat limit
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
                //TODO: check for mat limit
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
                    //TODO: check for mat limit
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
        DbPlayerUserData userData = await userDataRepository.LookupUserData();
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
            throw new DragaliaException(
                Models.ResultCode.EntityNotFoundError,
                $"DragonReliability {request.dragon_id} not found for DeviceAccountId {deviceAccountId}"
            );
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
        logger.LogDebug(
            "Creating GiftRewardList from rewards {@rewards} and levelGifts: {@levelGifts}",
            rewards,
            levelGifts
        );
        foreach (DragonGifts gift in request.dragon_gift_id_list)
        {
            rewardObjList.Add(
                new AtgenDragonGiftRewardList()
                {
                    return_gift_list = rewards.Where(x => x.Item1 == gift).First().Item2,
                    dragon_gift_id = gift,
                    is_favorite =
                        DragonConstants.rotatingGifts[(int)dragonData.FavoriteType] == gift,
                    reward_reliability_list =
                        levelGifts.FirstOrDefault(x => x.Item1 == gift)?.Item2
                        ?? Enumerable.Empty<RewardReliabilityList>()
                }
            );
        }
        logger.LogDebug("GiftRewardList: {@list}", rewardObjList);

        foreach (DbPlayerDragonGift gift in gifts.Values)
        {
            gift.Quantity -= 1;
        }

        userData.Coin -= totalCost;
        //rupies.Quantity -= totalCost;

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(deviceAccountId);

        await unitRepository.SaveChangesAsync();

        return new DragonBuyGiftToSendMultipleData()
        {
            dragon_contact_free_gift_count = 0,
            dragon_gift_reward_list = rewardObjList,
            entity_result = null,
            shop_gift_list = (await DoDragonGetContactData(new(), deviceAccountId)).shop_gift_list,
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

        logger.LogDebug(
            "Creating response from rewards {@rewards} and levelGifts: {@levelGifts}",
            rewards,
            levelGifts
        );
        return new DragonSendGiftMultipleData()
        {
            is_favorite = true,
            reward_reliability_list =
                levelGifts.FirstOrDefault()?.Item2 ?? Enumerable.Empty<RewardReliabilityList>(),
            return_gift_list = rewards.First().Item2,
            update_data_list = updateDataList
        };
    }

    public async Task<DragonBuildupData> DoBuildup(
        DragonBuildupRequest request,
        string deviceAccountId
    )
    {
        IEnumerable<Materials> matIds = request.grow_material_list
            .Where(x => x.type == EntityTypes.Material)
            .Select(x => x.id)
            .Cast<Materials>();

        Dictionary<Materials, DbPlayerMaterial> dbMats = await this.inventoryRepository
            .GetMaterials(deviceAccountId)
            .Where(
                dbMat =>
                    matIds.Contains(dbMat.MaterialId)
                    || dbMat.MaterialId == Materials.FortifyingDragonscale
                    || dbMat.MaterialId == Materials.AmplifyingDragonscale
            )
            .ToDictionaryAsync(dbMat => dbMat.MaterialId);
        foreach (GrowMaterialList mat in request.grow_material_list)
        {
            if (mat.quantity < 0)
            {
                throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Invalid quantity for MaterialList"
                );
            }
            if (
                mat.type == EntityTypes.Material
                && (
                    !dbMats.ContainsKey((Materials)mat.id)
                    || dbMats[(Materials)mat.id].Quantity < mat.quantity
                )
            )
            {
                throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Invalid quantity for MaterialList"
                );
            }
        }
        DbPlayerDragonData playerDragonData = await unitRepository
            .GetAllDragonData(deviceAccountId)
            .FirstAsync(dragon => (ulong)dragon.DragonKeyId == request.base_dragon_key_id);

        Dictionary<int, int> usedMaterials = new();
        await DragonLevelUp(
            request.grow_material_list,
            playerDragonData,
            usedMaterials,
            deviceAccountId
        );
        foreach (KeyValuePair<int, int> mat in usedMaterials)
        {
            dbMats[(Materials)mat.Key].Quantity -= mat.Value;
        }
        await unitRepository.RemoveDragons(
            deviceAccountId,
            request.grow_material_list
                .Where(x => x.type == EntityTypes.Dragon)
                .Select(x => (long)x.id)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(deviceAccountId);

        await unitRepository.SaveChangesAsync();

        return new DragonBuildupData(
            updateDataList,
            new DeleteDataList(
                request.grow_material_list
                    .Where(x => x.type == EntityTypes.Dragon)
                    .Select(x => new AtgenDeleteDragonList() { dragon_key_id = (ulong)x.id }),
                new List<AtgenDeleteTalismanList>(),
                new List<AtgenDeleteWeaponList>(),
                new List<AtgenDeleteAmuletList>()
            ),
            new()
        );
    }

    private async Task DragonLevelUp(
        IEnumerable<GrowMaterialList> materials,
        DbPlayerDragonData playerDragonData,
        Dictionary<int, int> usedMaterials,
        string deviceAccountId
    )
    {
        //TODO: For now we'll trust the client to not allow leveling up/enhancing beyond allowed limits
        byte maxLevel = DragonConstants.GetMaxLevelFor(
            MasterAsset.DragonData.Get(playerDragonData.DragonId).Rarity,
            playerDragonData.LimitBreakCount
        );
        int gainedHpAugs = 0;
        int gainedAtkAugs = 0;
        foreach (GrowMaterialList materialList in materials)
        {
            if (materialList.type == EntityTypes.Material)
            {
                switch ((Materials)materialList.id)
                {
                    case Materials.Dragonfruit:
                    case Materials.RipeDragonfruit:
                    case Materials.SucculentDragonfruit:
                        playerDragonData.Exp = Math.Min(
                            playerDragonData.Exp
                                + (
                                    UpgradeMaterials.buildupXpValues[(Materials)materialList.id]
                                    * materialList.quantity
                                ),
                            DragonConstants.XpLimits[maxLevel - 1]
                        );
                        break;
                    case Materials.AmplifyingDragonscale:
                        playerDragonData.AttackPlusCount = (byte)
                            Math.Min(
                                playerDragonData.AttackPlusCount + materialList.quantity,
                                DragonConstants.MaxAtkEnhance
                            );
                        break;
                    case Materials.FortifyingDragonscale:
                        playerDragonData.HpPlusCount = (byte)
                            Math.Min(
                                playerDragonData.HpPlusCount + materialList.quantity,
                                DragonConstants.MaxHpEnhance
                            );
                        break;
                    default:
                        throw new DragaliaException(
                            ResultCode.DragonGrowNotBoostMaterial,
                            "Invalid MaterialList"
                        );
                }
                if (!usedMaterials.ContainsKey((int)materialList.id))
                {
                    usedMaterials.Add((int)materialList.id, 0);
                }
                usedMaterials[(int)materialList.id] += materialList.quantity;
            }
            else if (materialList.type == EntityTypes.Dragon)
            {
                playerDragonData.Exp = Math.Min(
                    playerDragonData.Exp
                        + UpgradeMaterials.dragonBuildupXp[
                            MasterAsset.DragonData.Get(playerDragonData.DragonId).Rarity
                        ],
                    DragonConstants.XpLimits[maxLevel - 1]
                );
                DbPlayerDragonData dragonSacrifice = await unitRepository
                    .GetAllDragonData(deviceAccountId)
                    .Where(x => x.DragonKeyId == materialList.id)
                    .FirstAsync();
                gainedHpAugs += dragonSacrifice.HpPlusCount;
                gainedAtkAugs += dragonSacrifice.AttackPlusCount;
            }
        }
        while (
            playerDragonData.Level < maxLevel
            && playerDragonData.Level < DragonConstants.XpLimits.Length
            && !(playerDragonData.Exp < DragonConstants.XpLimits[playerDragonData.Level])
        )
        {
            playerDragonData.Level++;
        }

        int gainedDragonHpAugs = Math.Min(
            DragonConstants.MaxHpEnhance - playerDragonData.HpPlusCount,
            gainedHpAugs
        );
        playerDragonData.HpPlusCount += (byte)gainedDragonHpAugs;
        gainedHpAugs -= gainedDragonHpAugs;
        if (gainedHpAugs > 0)
        {
            if (!usedMaterials.TryAdd((int)Materials.FortifyingDragonscale, -gainedHpAugs))
            {
                usedMaterials[(int)Materials.FortifyingDragonscale] -= gainedHpAugs;
            }
        }

        int gainedDragonAtkAugs = Math.Min(
            DragonConstants.MaxAtkEnhance - playerDragonData.AttackPlusCount,
            gainedAtkAugs
        );
        playerDragonData.AttackPlusCount += (byte)gainedDragonAtkAugs;
        gainedAtkAugs -= gainedDragonAtkAugs;
        if (gainedAtkAugs > 0)
        {
            if (!usedMaterials.TryAdd((int)Materials.AmplifyingDragonscale, -gainedAtkAugs))
            {
                usedMaterials[(int)Materials.AmplifyingDragonscale] -= gainedAtkAugs;
            }
        }
    }

    public async Task<DragonResetPlusCountData> DoDragonResetPlusCount(
        DragonResetPlusCountRequest request,
        string deviceAccountId
    )
    {
        DbPlayerDragonData playerDragonData = await this.unitRepository
            .GetAllDragonData(deviceAccountId)
            .FirstAsync(dragon => (ulong)dragon.DragonKeyId == request.dragon_key_id);
        Materials mat =
            (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                ? Materials.AmplifyingDragonscale
                : Materials.FortifyingDragonscale;

        DbPlayerMaterial upgradeMat =
            await inventoryRepository.GetMaterial(deviceAccountId, mat)
            ?? inventoryRepository.AddMaterial(deviceAccountId, mat);

        //DbPlayerCurrency playerCurrency =
        //    await inventoryRepository.GetCurrency(deviceAccountId, CurrencyTypes.Rupies)
        //    ?? throw new DragaliaException(
        //        ResultCode.CommonMaterialShort,
        //        "Insufficient Rupies for reset"
        //    );
        DbPlayerUserData userData = await userDataRepository.LookupUserData();
        int cost =
            DragonConstants.AugmentResetCost
            * (
                (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                    ? playerDragonData.AttackPlusCount
                    : playerDragonData.HpPlusCount
            );
        //if (playerCurrency.Quantity < cost)
        if (userData.Coin < cost)
        {
            throw new DragaliaException(
                ResultCode.CommonMaterialShort,
                "Insufficient Rupies for reset"
            );
        }
        //playerCurrency.Quantity -= cost;
        userData.Coin -= cost;

        upgradeMat.Quantity +=
            (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                ? playerDragonData.AttackPlusCount
                : playerDragonData.HpPlusCount;
        _ =
            (UpgradeEnhanceTypes)request.plus_count_type == UpgradeEnhanceTypes.AtkPlus
                ? playerDragonData.AttackPlusCount = 0
                : playerDragonData.HpPlusCount = 0;

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(deviceAccountId);

        await userDataRepository.SaveChangesAsync();

        return new DragonResetPlusCountData(updateDataList, new());
    }

    public async Task<DragonLimitBreakData> DoDragonLimitBreak(
        DragonLimitBreakRequest request,
        string deviceAccountId
    )
    {
        DbPlayerDragonData playerDragonData =
            await this.unitRepository
                .GetAllDragonData(deviceAccountId)
                .Where(x => x.DragonKeyId == (long)request.base_dragon_key_id)
                .FirstAsync()
            ?? throw new DragaliaException(
                ResultCode.EntityNotFoundError,
                "No such dragon in inventory"
            );
        ;

        DragonData dragonData = MasterAsset.DragonData.Get(playerDragonData.DragonId);

        logger.LogDebug("Pre-LimitBreak Dragon: {@dragon}", playerDragonData);

        playerDragonData.LimitBreakCount = (byte)
            request.limit_break_grow_list.Last().limit_break_count;
        playerDragonData.Skill1Level = (byte)(1 + (playerDragonData.LimitBreakCount / 4));
        playerDragonData.Ability1Level = (byte)(playerDragonData.LimitBreakCount + 1);
        playerDragonData.Ability2Level = (byte)(playerDragonData.LimitBreakCount + 1);

        logger.LogDebug("Post-LimitBreak Dragon: {@dragon}", playerDragonData);

        IEnumerable<LimitBreakGrowList> deleteDragons = request.limit_break_grow_list.Where(
            x => (DragonLimitBreakMatTypes)x.limit_break_item_type == DragonLimitBreakMatTypes.Dupe
        );

        if (deleteDragons.Any())
        {
            await unitRepository.RemoveDragons(
                deviceAccountId,
                deleteDragons.Select(x => (long)x.target_id)
            );
        }

        int stonesToSpend = request.limit_break_grow_list
            .Where(
                x =>
                    (DragonLimitBreakMatTypes)x.limit_break_item_type
                    == DragonLimitBreakMatTypes.Stone
            )
            .Count();
        if (stonesToSpend > 0)
        {
            DbPlayerMaterial stones =
                await inventoryRepository.GetMaterial(
                    deviceAccountId,
                    dragonData.Rarity == 4 ? Materials.MoonlightStone : Materials.SunlightStone
                )
                ?? throw new DragaliaException(
                    ResultCode.CommonStoneShort,
                    $"Not enough stones to spend"
                );
            if (stones.Quantity < stonesToSpend)
            {
                throw new DragaliaException(
                    ResultCode.CommonStoneShort,
                    $"Not enough stones to spend"
                );
            }
            stones.Quantity -= stonesToSpend;
        }

        int spheresConsumed =
            request.limit_break_grow_list
                .Where(
                    x =>
                        (DragonLimitBreakMatTypes)x.limit_break_item_type
                        == DragonLimitBreakMatTypes.Spheres
                )
                .Count() * 50;

        int lb5SpheresConsumed =
            request.limit_break_grow_list
                .Where(
                    x =>
                        (DragonLimitBreakMatTypes)x.limit_break_item_type
                        == DragonLimitBreakMatTypes.SpheresLB5
                )
                .Count() * (dragonData.LimitBreakId == DragonLimitBreakTypes.Normal ? 50 : 120);
        if (spheresConsumed + lb5SpheresConsumed > 0)
        {
            DbPlayerMaterial spheres =
                await inventoryRepository.GetMaterial(
                    deviceAccountId,
                    dragonData.LimitBreakMaterialId
                )
                ?? throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Not enough spheres to spend"
                );
            if (spheres.Quantity < spheresConsumed + lb5SpheresConsumed)
            {
                throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Not enough spheres to spend"
                );
            }
            spheres.Quantity -= spheresConsumed + lb5SpheresConsumed;
        }

        UpdateDataList udl = updateDataService.GetUpdateDataList(deviceAccountId);

        await unitRepository.SaveChangesAsync();

        return new DragonLimitBreakData()
        {
            delete_data_list = new DeleteDataList()
            {
                delete_dragon_list = deleteDragons.Select(
                    x => new AtgenDeleteDragonList() { dragon_key_id = x.target_id }
                )
            },
            update_data_list = udl,
            entity_result = null
        };
    }

    public async Task<DragonSetLockData> DoDragonSetLock(
        DragonSetLockRequest request,
        string deviceAccountId
    )
    {
        (
            await this.unitRepository
                .GetAllDragonData(deviceAccountId)
                .SingleOrDefaultAsync(dragon => (ulong)dragon.DragonKeyId == request.dragon_key_id)
            ?? throw new DragaliaException(
                ResultCode.EntityNotFoundError,
                $"No dragon with KeyId: {request.dragon_key_id}"
            )
        ).IsLock = request.is_lock;

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(deviceAccountId);

        await userDataRepository.SaveChangesAsync();
        return new DragonSetLockData(updateDataList, new());
    }

    public async Task<DragonSellData> DoDragonSell(
        DragonSellRequest request,
        string deviceAccountId
    )
    {
        List<DbPlayerDragonData> selectedPlayerDragons = await unitRepository
            .GetAllDragonData(deviceAccountId)
            .Where(
                x =>
                    x.DeviceAccountId == deviceAccountId
                    && request.dragon_key_id_list.Select(x => (long)x).Contains(x.DragonKeyId)
            )
            .ToListAsync();
        if (selectedPlayerDragons.Count < request.dragon_key_id_list.Count())
        {
            throw new DragaliaException(
                Models.ResultCode.DragonSellNotFound,
                "Could not find all received dragonKeyIds to sell"
            );
        }

        if (selectedPlayerDragons.Where(x => x.DragonId == Dragons.Puppy).Any())
        {
            throw new DragaliaException(
                Models.ResultCode.DragonSellLocked,
                "Invalid sale attempt of the puppy"
            );
        }

        this.logger.LogInformation(
            "Requested sale of {count} dragons: {@list}",
            selectedPlayerDragons.Count,
            selectedPlayerDragons
        );

        //DbPlayerCurrency rupies = await inventoryRepository.GetCurrency(deviceAccountId, CurrencyTypes.Rupies) ?? inventoryRepository.AddCurrency(deviceAccountId, CurrencyTypes.Rupies);
        //DbPlayerCurrency dew = await inventoryRepository.GetCurrency(deviceAccountId, CurrencyTypes.Dew) ?? inventoryRepository.AddCurrency(deviceAccountId, CurrencyTypes.Dew);
        DbPlayerUserData userData = await userDataRepository.LookupUserData();
        this.logger.LogDebug(
            "Pre-sale: rupies {rupies}, eldwater {eldwater}",
            userData.Coin,
            userData.DewPoint
        );

        foreach (DbPlayerDragonData dd in selectedPlayerDragons)
        {
            //rupies.Quantity += dd.SellCoin;
            //dew.Quantity += dd.SellDewPoint;
            userData.Coin +=
                MasterAsset.DragonData.Get(dd.DragonId).SellCoin * (dd.LimitBreakCount + 1);
            userData.DewPoint +=
                MasterAsset.DragonData.Get(dd.DragonId).SellDewPoint * (dd.LimitBreakCount + 1);
        }

        this.logger.LogDebug(
            "Post-sale: rupies {rupies}, eldwater {eldwater}",
            userData.Coin,
            userData.DewPoint
        );

        await unitRepository.RemoveDragons(
            deviceAccountId,
            request.dragon_key_id_list.Select(x => (long)x)
        );

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(deviceAccountId);

        await userDataRepository.SaveChangesAsync();
        return new DragonSellData(
            new DeleteDataList(
                request.dragon_key_id_list.Select(
                    x => new AtgenDeleteDragonList() { dragon_key_id = x }
                ),
                new List<AtgenDeleteTalismanList>(),
                new List<AtgenDeleteWeaponList>(),
                new List<AtgenDeleteAmuletList>()
            ),
            updateDataList,
            new()
        );
    }
}
