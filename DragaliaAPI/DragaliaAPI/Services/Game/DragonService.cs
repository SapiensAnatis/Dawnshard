using System.Collections.Immutable;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Utils;
using DragaliaAPI.Extensions;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Helpers;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services.Game;

public class DragonService(
    IUserDataRepository userDataRepository,
    IUnitRepository unitRepository,
    IInventoryRepository inventoryRepository,
    IUpdateDataService updateDataService,
    IStoryRepository storyRepository,
    ILogger<DragonService> logger,
    IPaymentService paymentService,
    IRewardService rewardService,
    IMissionProgressionService missionProgressionService,
    IResetHelper resetHelper,
    ApiContext apiContext
) : IDragonService
{
    public async Task<DragonGetContactDataResponse> DoDragonGetContactData()
    {
        DragonGifts rotatingGift = DragonConstants.RotatingGifts[
            (int)resetHelper.LastDailyReset.DayOfWeek
        ];

        Dictionary<DragonGifts, DbPlayerDragonGift> gifts = await apiContext
            .PlayerDragonGifts.Where(x =>
                x.DragonGiftId == DragonGifts.FreshBread
                || x.DragonGiftId == DragonGifts.TastyMilk
                || x.DragonGiftId == DragonGifts.StrawberryTart
                || x.DragonGiftId == DragonGifts.HeartyStew
                || x.DragonGiftId == rotatingGift
            )
            .OrderBy(x => x.DragonGiftId)
            .ToDictionaryAsync(x => x.DragonGiftId);
        IEnumerable<AtgenShopGiftList> giftList = gifts
            .Values.Select(x => new AtgenShopGiftList()
            {
                DragonGiftId = (int)x.DragonGiftId,
                Price = DragonConstants.BuyGiftPrices.GetValueOrDefault(x.DragonGiftId, 0),
                IsBuy = x.Quantity > 0
            })
            .ToList();
        return new DragonGetContactDataResponse(giftList);
    }

    public Task<int> GetFreeGiftCount()
    {
        DragonGifts[] notificationGifts = [DragonGifts.FreshBread,];

        return apiContext.PlayerDragonGifts.CountAsync(x =>
            notificationGifts.Contains(x.DragonGiftId) && x.Quantity > 0
        );
    }

    private async Task<
        List<Tuple<DragonGifts, List<RewardReliabilityList>>>
    > IncreaseDragonReliability(
        DbPlayerDragonReliability dragonReliability,
        IEnumerable<Tuple<DragonGifts, int>> giftsAndQuantity
    )
    {
        List<Tuple<DragonGifts, List<RewardReliabilityList>>> levelGifts =
            new List<Tuple<DragonGifts, List<RewardReliabilityList>>>();
        int levelRewardIndex = dragonReliability.Level / 5;
        DragonData dragonData = MasterAsset.DragonData.Get(dragonReliability.DragonId);
        using IEnumerator<Tuple<DragonGifts, int>> enumerator = giftsAndQuantity.GetEnumerator();
        ImmutableArray<int> bondXpLimits =
            dragonReliability.DragonId == Dragons.Puppy
                ? DragonConstants.BondXpLimitsPuppy
                : DragonConstants.BondXpLimits;

        dragonReliability.LastContactTime = resetHelper.UtcNow;

        while (enumerator.MoveNext() && dragonReliability.Exp < bondXpLimits[^1])
        {
            dragonReliability.Exp = Math.Min(
                bondXpLimits[^1],
                dragonReliability.Exp
                    + (int)(
                        DragonConstants.FavorVals[enumerator.Current.Item1]
                        * (
                            DragonConstants.RotatingGifts[(int)dragonData.FavoriteType]
                            == enumerator.Current.Item1
                                ? DragonConstants.FavMulti
                                : 1
                        )
                        * enumerator.Current.Item2
                    )
            );
            List<RewardReliabilityList> returnGifts = new List<RewardReliabilityList>();

            int levelDifference = 0;

            while (
                !(
                    dragonReliability.Level == DragonConstants.MaxRelLevel
                    || bondXpLimits[dragonReliability.Level] > dragonReliability.Exp
                )
            )
            {
                levelDifference++;
                dragonReliability.Level++;
                if (dragonReliability.Level / 5 > levelRewardIndex)
                {
                    RewardReliabilityList? reward = await GetRewardDataForLevel(
                        dragonReliability.Level,
                        dragonData.Rarity,
                        dragonReliability.DragonId == Dragons.Puppy
                            ? Array.Empty<int>()
                            : MasterAsset
                                .DragonStories.Get((int)dragonReliability.DragonId)
                                .storyIds,
                        dragonReliability.DragonId == Dragons.Puppy
                    );
                    if (reward != null)
                    {
                        returnGifts.Add(reward);
                    }
                    levelRewardIndex++;
                }
            }

            if (levelDifference > 0)
            {
                missionProgressionService.OnDragonBondLevelUp(
                    dragonData.Id,
                    dragonData.ElementalType,
                    levelDifference,
                    dragonReliability.Level
                );
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

    private static readonly ImmutableArray<Materials> DragonLevelReward = new Materials[]
    {
        Materials.Omnicite,
        Materials.Talonstone,
        Materials.Omnicite,
        Materials.SucculentDragonfruit,
        Materials.Talonstone,
        Materials.SunlightOre
    }.ToImmutableArray();

    private static readonly int[][] DragonLevelRewardQuantity = new int[][]
    {
        new int[] { 999999, 3, 999999, 2, 5, 1 },
        new int[] { 999999, 4, 999999, 3, 7, 1 },
        new int[] { 999999, 5, 999999, 4, 10, 1 }
    };

    private static readonly ImmutableArray<Materials> PuppyLevelReward = new Materials[]
    {
        Materials.LightOrb,
        Materials.HalfEatenBread,
        Materials.HalfEatenBread,
        Materials.HalfEatenBread,
        Materials.HalfEatenBread,
        Materials.ImitationSquish
    }.ToImmutableArray();

    private static readonly int[] PuppyLevelRewardQuantity = new int[] { 1, 21, 28, 34, 40, 1 };

    private async Task<RewardReliabilityList?> GetRewardDataForLevel(
        int level,
        int rarity,
        int[] dragonStories,
        bool isPuppy
    )
    {
        RewardReliabilityList? reward = null;
        if (level > 4 && level % 5 == 0)
        {
            reward = new RewardReliabilityList()
            {
                Level = level,
                LevelupEntityList = new DragonRewardEntityList[1]
            };
            int levelIndex = level / 5;
            ImmutableArray<Materials> rewardMats = isPuppy ? PuppyLevelReward : DragonLevelReward;
            int[] rewardQuantity = isPuppy
                ? PuppyLevelRewardQuantity
                : DragonLevelRewardQuantity[rarity - 3];
            if (!isPuppy)
            {
                if (levelIndex == 1 || levelIndex == 3)
                {
                    int nextStoryUnlockIndex = await storyRepository
                        .Stories.Where(x => dragonStories.Contains(x.StoryId))
                        .CountAsync();

                    int nextStoryId = dragonStories.ElementAtOrDefault(nextStoryUnlockIndex);

                    if (nextStoryId != default)
                    {
                        await storyRepository.GetOrCreateStory(StoryTypes.Dragon, nextStoryId);
                        reward.IsReleaseStory = true;
                    }
                    else
                    {
                        logger.LogWarning(
                            "Failed to unlock next story for dragon: index {index} was out of range",
                            nextStoryUnlockIndex
                        );
                    }

                    return reward;
                }
                if (levelIndex == 6)
                {
                    //TODO: Add Notte's Nottes Bonus
                }
            }
            DragonRewardEntityList rewardItem = new DragonRewardEntityList()
            {
                EntityType = EntityTypes.Material,
                EntityId = (int)rewardMats[levelIndex - 1],
                EntityQuantity = rewardQuantity[levelIndex - 1],
                //TODO: check for mat limit
            };
            DbPlayerMaterial mat =
                await inventoryRepository.GetMaterial((Materials)rewardItem.EntityId)
                ?? inventoryRepository.AddMaterial((Materials)rewardItem.EntityId);
            mat.Quantity += rewardItem.EntityQuantity;
            ((DragonRewardEntityList[])reward.LevelupEntityList)[0] = rewardItem;
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
            Materials material = Random.Shared.Next(rewards);
            (
                await inventoryRepository.GetMaterial(material)
                ?? inventoryRepository.AddMaterial(material)
            ).Quantity += rewardQuantity;
            DragonRewardEntityList reward = new DragonRewardEntityList()
            {
                EntityType = EntityTypes.Material,
                EntityId = (int)material,
                EntityQuantity = rewardQuantity,
                //TODO: check for mat limit
            };
            giftPerGift.Add(new(gift, reward));
        }
        return giftPerGift;
    }

    private async Task<
        List<Tuple<DragonGifts, List<DragonRewardEntityList>>>
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
                            reward.EntityType = EntityTypes.Mana;
                            reward.EntityId = 0;
                            reward.EntityQuantity = r.Next(rarityQuantities[1]) * 500;
                            (await userDataRepository.UserData.SingleAsync()).ManaPoint +=
                                reward.EntityQuantity;
                            break;
                        case 1:
                            int rIndex = r.Next(fruits.Length);
                            Materials fruitMat = fruits[rIndex];
                            reward.EntityType = EntityTypes.Material;
                            reward.EntityId = (int)fruitMat;
                            reward.EntityQuantity = r.Next(rarityQuantities[rIndex]);
                            (
                                await inventoryRepository.GetMaterial(fruitMat)
                                ?? inventoryRepository.AddMaterial(fruitMat)
                            ).Quantity += reward.EntityQuantity;
                            break;
                        case 2:
                            rIndex = r.Next(orbs.Length);
                            reward.EntityType = EntityTypes.Material;
                            reward.EntityId = (int)orbs[rIndex][dragonElement];
                            reward.EntityQuantity = r.Next(rarityQuantities[rIndex]);
                            (
                                await inventoryRepository.GetMaterial((Materials)reward.EntityId)
                                ?? inventoryRepository.AddMaterial((Materials)reward.EntityId)
                            ).Quantity += reward.EntityQuantity;
                            break;
                        case 3:
                            reward.EntityType = EntityTypes.Material;
                            reward.EntityId = (int)Materials.Talonstone;
                            reward.EntityQuantity = r.Next(rarityQuantities[1]);
                            (
                                await inventoryRepository.GetMaterial(Materials.Talonstone)
                                ?? inventoryRepository.AddMaterial(Materials.Talonstone)
                            ).Quantity += reward.EntityQuantity;
                            break;
                    }
                    //TODO: check for mat limit
                    tyGifts.Add(reward);
                }
                tyGiftsPerItem.AddRange(tyGifts);
            }
            giftsPerGift.Add(new(gift.Item1, tyGiftsPerItem));
        }
        return giftsPerGift;
    }

    public async Task<DragonBuyGiftToSendMultipleResponse> DoDragonBuyGiftToSendMultiple(
        DragonBuyGiftToSendMultipleRequest request,
        CancellationToken cancellationToken
    )
    {
        DbPlayerUserData userData = await userDataRepository.UserData.SingleAsync(
            cancellationToken
        );

        int totalCost = request
            .DragonGiftIdList.Select(x => DragonConstants.BuyGiftPrices[x])
            .Sum();
        if (userData.Coin < totalCost)
            throw new DragaliaException(ResultCode.CommonMaterialShort, "Insufficient Rupies");

        Dictionary<DragonGifts, DbPlayerDragonGift> gifts = await apiContext
            .PlayerDragonGifts.Where(x => request.DragonGiftIdList.Contains(x.DragonGiftId))
            .ToDictionaryAsync(x => x.DragonGiftId, cancellationToken);

        DbPlayerDragonReliability dragonReliability = await unitRepository
            .DragonReliabilities.Where(x => x.DragonId == request.DragonId)
            .FirstAsync(cancellationToken);

        if (dragonReliability == null)
        {
            throw new DragaliaException(
                ResultCode.EntityNotFoundError,
                $"DragonReliability {request.DragonId} not found"
            );
        }

        DragonData dragonData = MasterAsset.DragonData.Get(dragonReliability.DragonId);

        List<Tuple<DragonGifts, List<DragonRewardEntityList>>> rewards =
            dragonReliability.DragonId == Dragons.Puppy
                ? (await RollPuppyThankYouRewards(request.DragonGiftIdList))
                    .Select(x => new Tuple<DragonGifts, List<DragonRewardEntityList>>(
                        x.Item1,
                        new List<DragonRewardEntityList>() { x.Item2 }
                    ))
                    .ToList()
                : await this.RollDragonThankYouRewards(
                    request.DragonGiftIdList.Select(x => new Tuple<DragonGifts, int>(x, 1)),
                    dragonData.ElementalType
                );

        List<Tuple<DragonGifts, List<RewardReliabilityList>>> levelGifts =
            await this.IncreaseDragonReliability(
                dragonReliability,
                request.DragonGiftIdList.Select(x => new Tuple<DragonGifts, int>((DragonGifts)x, 1))
            );

        List<AtgenDragonGiftRewardList> rewardObjList = new List<AtgenDragonGiftRewardList>();
        logger.LogDebug(
            "Creating GiftRewardList from rewards {@rewards} and levelGifts: {@levelGifts}",
            rewards,
            levelGifts
        );
        foreach (DragonGifts gift in request.DragonGiftIdList)
        {
            rewardObjList.Add(
                new AtgenDragonGiftRewardList()
                {
                    ReturnGiftList = rewards.First(x => x.Item1 == gift).Item2,
                    DragonGiftId = gift,
                    IsFavorite =
                        DragonConstants.RotatingGifts[(int)dragonData.FavoriteType] == gift,
                    RewardReliabilityList =
                        levelGifts.FirstOrDefault(x => x.Item1 == gift)?.Item2
                        ?? Enumerable.Empty<RewardReliabilityList>()
                }
            );

            missionProgressionService.OnDragonGiftSent(
                request.DragonId,
                gift,
                dragonData.ElementalType,
                1,
                0
            );
        }

        logger.LogDebug("GiftRewardList: {@list}", rewardObjList);

        foreach (DbPlayerDragonGift gift in gifts.Values)
        {
            gift.Quantity -= 1;
        }

        userData.Coin -= totalCost;

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return new DragonBuyGiftToSendMultipleResponse()
        {
            DragonContactFreeGiftCount = 0,
            DragonGiftRewardList = rewardObjList,
            EntityResult = null,
            ShopGiftList = (await DoDragonGetContactData()).ShopGiftList,
            UpdateDataList = updateDataList
        };
    }

    public async Task<DragonSendGiftMultipleResponse> DoDragonSendGiftMultiple(
        DragonSendGiftMultipleRequest request,
        CancellationToken cancellationToken
    )
    {
        DbPlayerDragonGift? gift = await apiContext
            .PlayerDragonGifts.Where(x => x.DragonGiftId == request.DragonGiftId)
            .FirstOrDefaultAsync(cancellationToken);

        if (gift == null || gift.Quantity < request.Quantity)
        {
            throw new DragaliaException(
                ResultCode.CommonMaterialShort,
                $"Insufficient gift quantity for: {request.DragonGiftId}"
            );
        }

        DbPlayerDragonReliability? dragonReliability = await unitRepository
            .DragonReliabilities.Where(x => x.DragonId == request.DragonId)
            .FirstOrDefaultAsync(cancellationToken);

        if (dragonReliability == null)
        {
            throw new DragaliaException(
                ResultCode.EntityNotFoundError,
                $"No such dragon in inventory: {request.DragonId}"
            );
        }

        List<Tuple<DragonGifts, int>> requestGift = new List<Tuple<DragonGifts, int>>()
        {
            Tuple.Create(request.DragonGiftId, request.Quantity)
        };

        DragonData dragonData = MasterAsset.DragonData.Get(dragonReliability.DragonId);

        IEnumerable<Tuple<DragonGifts, List<DragonRewardEntityList>>> rewards =
            dragonReliability.DragonId == Dragons.Puppy
                ? (
                    await RollPuppyThankYouRewards(new List<DragonGifts>() { request.DragonGiftId })
                ).Select(x => new Tuple<DragonGifts, List<DragonRewardEntityList>>(
                    x.Item1,
                    new List<DragonRewardEntityList>() { x.Item2 }
                ))
                : await this.RollDragonThankYouRewards(requestGift, dragonData.ElementalType);

        IEnumerable<Tuple<DragonGifts, List<RewardReliabilityList>>> levelGifts =
            await this.IncreaseDragonReliability(dragonReliability, requestGift);

        gift.Quantity -= request.Quantity;

        missionProgressionService.OnDragonGiftSent(
            request.DragonId,
            request.DragonGiftId,
            dragonData.ElementalType,
            request.Quantity,
            0
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        logger.LogDebug(
            "Creating response from rewards {@rewards} and levelGifts: {@levelGifts}",
            rewards,
            levelGifts
        );
        return new DragonSendGiftMultipleResponse()
        {
            IsFavorite = true,
            RewardReliabilityList =
                levelGifts.FirstOrDefault()?.Item2 ?? Enumerable.Empty<RewardReliabilityList>(),
            ReturnGiftList = rewards.First().Item2,
            UpdateDataList = updateDataList
        };
    }

    public async Task<DragonBuildupResponse> DoBuildup(
        DragonBuildupRequest request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<Materials> matIds = request
            .GrowMaterialList.Where(x => x.Type == EntityTypes.Material)
            .Select(x => x.Id)
            .Cast<Materials>();

        Dictionary<Materials, DbPlayerMaterial> dbMats = await inventoryRepository
            .Materials.Where(dbMat =>
                matIds.Contains(dbMat.MaterialId)
                || dbMat.MaterialId == Materials.FortifyingDragonscale
                || dbMat.MaterialId == Materials.AmplifyingDragonscale
            )
            .ToDictionaryAsync(dbMat => dbMat.MaterialId, cancellationToken);
        foreach (GrowMaterialList mat in request.GrowMaterialList)
        {
            if (mat.Quantity < 0)
            {
                throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Invalid quantity for MaterialList"
                );
            }
            if (
                mat.Type == EntityTypes.Material
                && (
                    !dbMats.ContainsKey((Materials)mat.Id)
                    || dbMats[(Materials)mat.Id].Quantity < mat.Quantity
                )
            )
            {
                throw new DragaliaException(
                    ResultCode.CommonMaterialShort,
                    "Invalid quantity for MaterialList"
                );
            }
        }
        DbPlayerDragonData playerDragonData = await unitRepository.Dragons.FirstAsync(
            dragon => (ulong)dragon.DragonKeyId == request.BaseDragonKeyId,
            cancellationToken
        );

        Dictionary<int, int> usedMaterials = new();
        await DragonLevelUp(request.GrowMaterialList, playerDragonData, usedMaterials);
        foreach (KeyValuePair<int, int> mat in usedMaterials)
        {
            dbMats[(Materials)mat.Key].Quantity -= mat.Value;
        }
        await unitRepository.RemoveDragons(
            request
                .GrowMaterialList.Where(x => x.Type == EntityTypes.Dragon)
                .Select(x => (long)x.Id)
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return new DragonBuildupResponse(
            updateDataList,
            new DeleteDataList(
                request
                    .GrowMaterialList.Where(x => x.Type == EntityTypes.Dragon)
                    .Select(x => new AtgenDeleteDragonList() { DragonKeyId = (ulong)x.Id }),
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
        Dictionary<int, int> usedMaterials
    )
    {
        DragonData dragonData = MasterAsset.DragonData.Get(playerDragonData.DragonId);

        //TODO: For now we'll trust the client to not allow leveling up/enhancing beyond allowed limits
        byte maxLevel = DragonConstants.GetMaxLevelFor(
            dragonData.Rarity,
            playerDragonData.LimitBreakCount
        );
        int gainedHpAugs = 0;
        int gainedAtkAugs = 0;
        foreach (GrowMaterialList materialList in materials)
        {
            if (materialList.Type == EntityTypes.Material)
            {
                switch ((Materials)materialList.Id)
                {
                    case Materials.Dragonfruit:
                    case Materials.RipeDragonfruit:
                    case Materials.SucculentDragonfruit:
                        playerDragonData.Exp = Math.Min(
                            playerDragonData.Exp
                                + (
                                    UpgradeMaterials.buildupXpValues[(Materials)materialList.Id]
                                    * materialList.Quantity
                                ),
                            DragonConstants.XpLimits[maxLevel - 1]
                        );
                        break;
                    case Materials.AmplifyingDragonscale:
                        playerDragonData.AttackPlusCount = (byte)
                            Math.Min(
                                playerDragonData.AttackPlusCount + materialList.Quantity,
                                DragonConstants.MaxAtkEnhance
                            );
                        break;
                    case Materials.FortifyingDragonscale:
                        playerDragonData.HpPlusCount = (byte)
                            Math.Min(
                                playerDragonData.HpPlusCount + materialList.Quantity,
                                DragonConstants.MaxHpEnhance
                            );
                        break;
                    default:
                        throw new DragaliaException(
                            ResultCode.DragonGrowNotBoostMaterial,
                            "Invalid MaterialList"
                        );
                }
                if (!usedMaterials.ContainsKey((int)materialList.Id))
                {
                    usedMaterials.Add((int)materialList.Id, 0);
                }
                usedMaterials[(int)materialList.Id] += materialList.Quantity;
            }
            else if (materialList.Type == EntityTypes.Dragon)
            {
                playerDragonData.Exp = Math.Min(
                    playerDragonData.Exp
                        + UpgradeMaterials.dragonBuildupXp[
                            MasterAsset.DragonData.Get(playerDragonData.DragonId).Rarity
                        ],
                    DragonConstants.XpLimits[maxLevel - 1]
                );
                DbPlayerDragonData dragonSacrifice = await unitRepository
                    .Dragons.Where(x => x.DragonKeyId == materialList.Id)
                    .FirstAsync();
                gainedHpAugs += dragonSacrifice.HpPlusCount;
                gainedAtkAugs += dragonSacrifice.AttackPlusCount;
            }
        }

        int levelDifference = 0;

        while (
            playerDragonData.Level < maxLevel
            && playerDragonData.Level < DragonConstants.XpLimits.Length
            && !(playerDragonData.Exp < DragonConstants.XpLimits[playerDragonData.Level])
        )
        {
            levelDifference++;
            playerDragonData.Level++;
        }

        if (levelDifference > 0)
        {
            missionProgressionService.OnDragonLevelUp(
                playerDragonData.DragonId,
                dragonData.ElementalType,
                levelDifference,
                playerDragonData.Level
            );
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

    public async Task<DragonResetPlusCountResponse> DoDragonResetPlusCount(
        DragonResetPlusCountRequest request,
        CancellationToken cancellationToken
    )
    {
        DbPlayerDragonData playerDragonData = await unitRepository.Dragons.FirstAsync(
            dragon => (ulong)dragon.DragonKeyId == request.DragonKeyId,
            cancellationToken
        );

        Materials material;
        int plusCount;

        switch (request.PlusCountType)
        {
            case PlusCountType.Atk:
                material = Materials.AmplifyingDragonscale;
                plusCount = playerDragonData.AttackPlusCount;
                playerDragonData.AttackPlusCount = 0;
                break;
            case PlusCountType.Hp:
                material = Materials.FortifyingDragonscale;
                plusCount = playerDragonData.HpPlusCount;
                playerDragonData.HpPlusCount = 0;
                break;
            default:
                throw new DragaliaException(
                    ResultCode.CommonInvalidArgument,
                    "Invalid plus_count_type"
                );
        }

        await paymentService.ProcessPayment(
            PaymentTypes.Coin,
            expectedPrice: DragonConstants.AugmentResetCost * plusCount
        );
        await rewardService.GrantReward(new Entity(EntityTypes.Material, (int)material, plusCount));

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return new DragonResetPlusCountResponse(updateDataList, rewardService.GetEntityResult());
    }

    public async Task<DragonLimitBreakResponse> DoDragonLimitBreak(
        DragonLimitBreakRequest request,
        CancellationToken cancellationToken
    )
    {
        DbPlayerDragonData playerDragonData =
            await unitRepository
                .Dragons.Where(x => x.DragonKeyId == (long)request.BaseDragonKeyId)
                .FirstAsync(cancellationToken)
            ?? throw new DragaliaException(
                ResultCode.EntityNotFoundError,
                "No such dragon in inventory"
            );

        DragonData dragonData = MasterAsset.DragonData.Get(playerDragonData.DragonId);

        logger.LogDebug("Pre-LimitBreak Dragon: {@dragon}", playerDragonData);

        playerDragonData.LimitBreakCount = (byte)request.LimitBreakGrowList.Last().LimitBreakCount;
        playerDragonData.Skill1Level = (byte)(1 + (playerDragonData.LimitBreakCount / 4));
        playerDragonData.Ability1Level = (byte)(playerDragonData.LimitBreakCount + 1);
        playerDragonData.Ability2Level = (byte)(playerDragonData.LimitBreakCount + 1);

        logger.LogDebug("Post-LimitBreak Dragon: {@dragon}", playerDragonData);

        LimitBreakGrowList[] deleteDragons = request
            .LimitBreakGrowList.Where(x =>
                (DragonLimitBreakMatTypes)x.LimitBreakItemType == DragonLimitBreakMatTypes.Dupe
            )
            .ToArray();

        if (deleteDragons.Length != 0)
        {
            await unitRepository.RemoveDragons(deleteDragons.Select(x => (long)x.TargetId));
        }

        int stonesToSpend = request
            .LimitBreakGrowList.Where(x =>
                (DragonLimitBreakMatTypes)x.LimitBreakItemType == DragonLimitBreakMatTypes.Stone
            )
            .Count();
        if (stonesToSpend > 0)
        {
            DbPlayerMaterial stones =
                await inventoryRepository.GetMaterial(
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
            request
                .LimitBreakGrowList.Where(x =>
                    (DragonLimitBreakMatTypes)x.LimitBreakItemType
                    == DragonLimitBreakMatTypes.Spheres
                )
                .Count() * 50;

        int lb5SpheresConsumed =
            request
                .LimitBreakGrowList.Where(x =>
                    (DragonLimitBreakMatTypes)x.LimitBreakItemType
                    == DragonLimitBreakMatTypes.SpheresLB5
                )
                .Count() * (dragonData.LimitBreakId == DragonLimitBreakTypes.Normal ? 50 : 120);
        if (spheresConsumed + lb5SpheresConsumed > 0)
        {
            DbPlayerMaterial spheres =
                await inventoryRepository.GetMaterial(dragonData.LimitBreakMaterialId)
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

        UpdateDataList udl = await updateDataService.SaveChangesAsync(cancellationToken);

        return new DragonLimitBreakResponse()
        {
            DeleteDataList = new DeleteDataList()
            {
                DeleteDragonList = deleteDragons.Select(x => new AtgenDeleteDragonList()
                {
                    DragonKeyId = x.TargetId
                })
            },
            UpdateDataList = udl,
            EntityResult = null
        };
    }

    public async Task<DragonSetLockResponse> DoDragonSetLock(
        DragonSetLockRequest request,
        CancellationToken cancellationToken
    )
    {
        (
            await unitRepository.Dragons.SingleOrDefaultAsync(
                dragon => (ulong)dragon.DragonKeyId == request.DragonKeyId,
                cancellationToken
            )
            ?? throw new DragaliaException(
                ResultCode.EntityNotFoundError,
                $"No dragon with KeyId: {request.DragonKeyId}"
            )
        ).IsLock = request.IsLock;

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        await userDataRepository.SaveChangesAsync();
        return new DragonSetLockResponse(updateDataList, new());
    }

    public async Task<DragonSellResponse> DoDragonSell(
        DragonSellRequest request,
        CancellationToken cancellationToken
    )
    {
        List<DbPlayerDragonData> selectedPlayerDragons = await unitRepository
            .Dragons.Where(x =>
                request.DragonKeyIdList.Select(y => (long)y).Contains(x.DragonKeyId)
            )
            .ToListAsync(cancellationToken);
        if (selectedPlayerDragons.Count < request.DragonKeyIdList.Count())
        {
            throw new DragaliaException(
                ResultCode.DragonSellNotFound,
                "Could not find all received dragonKeyIds to sell"
            );
        }

        if (selectedPlayerDragons.Where(x => x.DragonId == Dragons.Puppy).Any())
        {
            throw new DragaliaException(
                ResultCode.DragonSellLocked,
                "Invalid sale attempt of the puppy"
            );
        }

        logger.LogInformation(
            "Requested sale of {count} dragons: {@list}",
            selectedPlayerDragons.Count,
            selectedPlayerDragons
        );

        //DbPlayerCurrency rupies = await inventoryRepository.GetCurrency(deviceAccountId, CurrencyTypes.Rupies) ?? inventoryRepository.AddCurrency(deviceAccountId, CurrencyTypes.Rupies);
        //DbPlayerCurrency dew = await inventoryRepository.GetCurrency(deviceAccountId, CurrencyTypes.Dew) ?? inventoryRepository.AddCurrency(deviceAccountId, CurrencyTypes.Dew);
        DbPlayerUserData userData = await userDataRepository.UserData.SingleAsync(
            cancellationToken
        );
        logger.LogDebug(
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

        logger.LogDebug(
            "Post-sale: rupies {rupies}, eldwater {eldwater}",
            userData.Coin,
            userData.DewPoint
        );

        await unitRepository.RemoveDragons(request.DragonKeyIdList.Select(x => (long)x));

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        await userDataRepository.SaveChangesAsync();
        return new DragonSellResponse(
            new DeleteDataList(
                request.DragonKeyIdList.Select(x => new AtgenDeleteDragonList()
                {
                    DragonKeyId = x
                }),
                new List<AtgenDeleteTalismanList>(),
                new List<AtgenDeleteWeaponList>(),
                new List<AtgenDeleteAmuletList>()
            ),
            updateDataList,
            new()
        );
    }
}
