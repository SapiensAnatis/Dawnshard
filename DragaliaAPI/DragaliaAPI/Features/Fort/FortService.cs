﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Features.Player;
using DragaliaAPI.Features.Shared.Options;
using DragaliaAPI.Features.Shared.Reward;
using DragaliaAPI.Features.Shop;
using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Features.Fort;

public class FortService(
    IFortRepository fortRepository,
    IUserDataRepository userDataRepository,
    IInventoryRepository inventoryRepository,
    ILogger<FortService> logger,
    IPlayerIdentityService playerIdentityService,
    IFortMissionProgressionService fortMissionProgressionService,
    IPaymentService paymentService,
    IRewardService rewardService,
    IOptionsMonitor<DragonfruitConfig> config,
    IUserService userService,
    TimeProvider dateTimeProvider
) : IFortService
{
    public const int MaximumCarpenterNum = 5;
    private readonly DragonfruitConfig config = config.CurrentValue;

    public async Task<IEnumerable<BuildList>> GetBuildList()
    {
        return await fortRepository
            .Builds.AsNoTracking()
            .Select(x => new BuildList(
                (ulong)x.BuildId,
                x.PlantId,
                x.Level,
                // Using NotMapped getters like FortPlantDetailId isn't great, and will cause all columns to be
                // retrieved, but the query needs to retrieve all but one of the columns anyway.
                x.FortPlantDetailId,
                x.PositionX,
                x.PositionZ,
                x.BuildStatus,
                x.BuildStartDate,
                x.BuildEndDate,
                x.RemainTime,
                x.LastIncomeTime,
                x.IsNew
            ))
            .ToListAsync();
    }

    public async Task<FortDetail> AddCarpenter(PaymentTypes paymentType)
    {
        FortDetail fortDetail = await GetFortDetail();

        // https://dragalialost.wiki/w/Facilities
        // First 2 are free, 3rd 250, 4th 400, 5th 700
        int paymentCost = fortDetail.CarpenterNum switch
        {
            < 2 => 0,
            2 => 250,
            3 => 400,
            4 => 700,
            _ => throw new DragaliaException(
                ResultCode.FortExtendCarpenterLimit,
                $"User has reached maximum carpenter."
            ),
        };

        if (paymentType is not (PaymentTypes.Diamantium or PaymentTypes.Wyrmite))
        {
            throw new DragaliaException(
                ResultCode.ShopPaymentTypeInvalid,
                $"Invalid currency used to add carpenter."
            );
        }

        await paymentService.ProcessPayment(paymentType, expectedPrice: paymentCost);

        // Add carpenter
        await fortRepository.UpdateFortMaximumCarpenter(fortDetail.CarpenterNum + 1);

        logger.LogDebug(
            "Added carpenter using payment type {type}. New count: {count}",
            paymentType,
            fortDetail.CarpenterNum
        );

        return fortDetail;
    }

    public async Task<FortGetMultiIncomeResponse> CollectIncome(IEnumerable<long> idsToCollect)
    {
        Random rdm = Random.Shared;
        DateTimeOffset current = dateTimeProvider.GetUtcNow();

        FortGetMultiIncomeResponse resp = new();

        List<AtgenAddCoinList> coinList = new();
        List<AtgenAddStaminaList> staminaList = new();
        List<AtgenHarvestBuildList> harvestList = new();

        int coinTotal,
            normalTotal,
            ripeTotal,
            succulentTotal,
            staminaTotal;
        coinTotal = normalTotal = ripeTotal = succulentTotal = staminaTotal = 0;

        foreach (
            DbFortBuild build in await fortRepository
                .Builds.Where(x => idsToCollect.Contains(x.BuildId))
                .ToListAsync()
        )
        {
            double incomeTime = build.LastIncomeTime.TotalSeconds;
            FortPlantDetail detail = MasterAsset.FortPlantDetail[build.FortPlantDetailId];

            switch (build.PlantId)
            {
                case FortPlants.RupieMine:
                    int rupies =
                        incomeTime > detail.CostMaxTime
                            ? detail.CostMax
                            : (int)(detail.CostMax * (incomeTime / detail.CostMaxTime));

                    coinList.Add(new AtgenAddCoinList(build.BuildId, rupies));
                    coinTotal += rupies;
                    break;
                case FortPlants.Dragontree:
                    int fruits =
                        incomeTime > detail.MaterialMaxTime
                            ? detail.MaterialMax
                            : (int)(detail.MaterialMax * (incomeTime / detail.MaterialMaxTime));

                    DragonfruitOdds odds = this.config.FruitOdds[detail.Odds];

                    int ripe,
                        succulent;
                    int normal = ripe = succulent = 0;

                    for (int i = 0; i < fruits; i++)
                    {
                        int value = rdm.Next(101);
                        if (odds.Normal >= value)
                            normal++;
                        else if (odds.Normal + odds.Ripe >= value)
                            ripe++;
                        else
                            succulent++;
                    }

                    AtgenAddHarvestList[] lists = new AtgenAddHarvestList[3];
                    lists[0] = new AtgenAddHarvestList(Materials.Dragonfruit, normal);
                    lists[1] = new AtgenAddHarvestList(Materials.RipeDragonfruit, ripe);
                    lists[2] = new AtgenAddHarvestList(Materials.SucculentDragonfruit, succulent);

                    normalTotal += normal;
                    ripeTotal += ripe;
                    succulentTotal += succulent;

                    harvestList.Add(new AtgenHarvestBuildList(build.BuildId, lists));
                    break;
                case FortPlants.TheHalidom:
                    int stamina =
                        incomeTime > detail.StaminaMaxTime
                            ? detail.StaminaMax
                            : (int)(detail.StaminaMax * (incomeTime / detail.StaminaMaxTime));

                    staminaTotal += stamina;

                    staminaList.Add(new AtgenAddStaminaList(build.BuildId, stamina));
                    break;
                default:
                    throw new DragaliaException(
                        ResultCode.FortIncomeError,
                        "Attempted to collect income from a non-resource facility"
                    );
            }

            build.LastIncomeDate = current;
        }

        if (coinTotal != 0)
        {
            await rewardService.GrantReward(new Entity(EntityTypes.Rupies, 1, coinTotal));
            fortMissionProgressionService.OnFortIncomeCollected(EntityTypes.Rupies);
        }

        if (normalTotal != 0)
        {
            await rewardService.GrantReward(
                new Entity(EntityTypes.Material, (int)Materials.Dragonfruit, coinTotal)
            );
        }

        if (ripeTotal != 0)
        {
            await rewardService.GrantReward(
                new Entity(EntityTypes.Material, (int)Materials.RipeDragonfruit, coinTotal)
            );
        }

        if (succulentTotal != 0)
        {
            await rewardService.GrantReward(
                new Entity(EntityTypes.Material, (int)Materials.SucculentDragonfruit, coinTotal)
            );
        }

        if (staminaTotal != 0)
        {
            await userService.AddStamina(StaminaType.Single, staminaTotal);
        }

        resp.AddCoinList = coinList;
        resp.AddStaminaList = staminaList;
        resp.HarvestBuildList = harvestList;

        return resp;
    }

    /// <summary>
    /// Get the fort details object.
    /// </summary>
    /// <remarks>
    /// Where this is used after changing fort state, it MUST be used after calling SaveChangesAsync to ensure
    /// the active carpenter count is accurate.
    /// </remarks>
    /// <returns></returns>
    public async Task<FortDetail> GetFortDetail()
    {
        DbFortDetail dbDetail = await fortRepository.GetFortDetail();
        int activeCarpenters = await fortRepository.GetActiveCarpenters();

        logger.LogDebug(
            "Active carpenters: {n1}, max carpenters: {n2}",
            activeCarpenters,
            dbDetail.CarpenterNum
        );

        return new()
        {
            MaxCarpenterCount = MaximumCarpenterNum,
            WorkingCarpenterNum = activeCarpenters,
            CarpenterNum = dbDetail.CarpenterNum,
        };
    }

    public async Task BuildAtOnce(PaymentTypes paymentType, long buildId)
    {
        logger.LogDebug("BuildAtOnce called for build {buildId}", buildId);

        DbFortBuild build = await fortRepository.GetBuilding(buildId);

        if (build.BuildStatus is not FortBuildStatus.Building)
        {
            throw new InvalidOperationException($"This building is not currently being built.");
        }

        await CompleteAtOnce(build, paymentType, false);
    }

    public async Task LevelupAtOnce(PaymentTypes paymentType, long buildId)
    {
        logger.LogDebug("LevelupAtOnce called for build {buildId}", buildId);

        DbFortBuild build = await fortRepository.GetBuilding(buildId);

        if (build.BuildStatus is not FortBuildStatus.LevelUp)
        {
            throw new InvalidOperationException($"This building is not currently being upgraded.");
        }

        await CompleteAtOnce(build, paymentType, true);
    }

    private async Task CompleteAtOnce(DbFortBuild build, PaymentTypes paymentType, bool levelUp)
    {
        if (
            paymentType
            is not (
                PaymentTypes.Diamantium
                or PaymentTypes.Wyrmite
                or PaymentTypes.HalidomHustleHammer
            )
        )
        {
            throw new DragaliaException(ResultCode.ShopPaymentTypeInvalid, "Invalid payment type.");
        }

        int paymentCost = GetUpgradePaymentCost(
            paymentType,
            dateTimeProvider.GetUtcNow(),
            build.BuildEndDate
        );

        await paymentService.ProcessPayment(paymentType, expectedPrice: paymentCost);

        await FinishUpgrade(build, levelUp);
    }

    public async Task<DbFortBuild> CancelBuild(long buildId)
    {
        logger.LogDebug("Build cancelled for build {buildId}", buildId);

        DbFortBuild build = await fortRepository.GetBuilding(buildId);

        if (build.BuildStatus is not FortBuildStatus.Building)
            throw new InvalidOperationException($"This building is not currently being built.");

        fortRepository.DeleteBuild(build);

        return build;
    }

    public async Task<DbFortBuild> CancelLevelup(long buildId)
    {
        logger.LogDebug("Levelup cancelled for build {buildId}", buildId);

        DbFortBuild build = await fortRepository.GetBuilding(buildId);

        if (build.BuildStatus is not FortBuildStatus.LevelUp)
            throw new InvalidOperationException($"This building is not currently being upgraded.");

        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;

        return build;
    }

    public async Task EndBuild(long buildId)
    {
        DateTimeOffset current = dateTimeProvider.GetUtcNow();

        logger.LogDebug("Build ended for build {buildId}", buildId);

        DbFortBuild build = await fortRepository.GetBuilding(buildId);

        if (build.BuildStatus is not FortBuildStatus.Building)
            throw new DragaliaException(ResultCode.FortBuildIncomplete, "Invalid state");

        if (current < build.BuildEndDate)
        {
            throw new DragaliaException(
                ResultCode.FortBuildIncomplete,
                $"This building has not completed construction."
            );
        }

        build.LastIncomeDate = current;
        await FinishUpgrade(build, false);
    }

    public async Task EndLevelup(long buildId)
    {
        logger.LogDebug("Levelup ended for build {buildId}", buildId);

        DbFortBuild build = await fortRepository.GetBuilding(buildId);

        DateTimeOffset time = dateTimeProvider.GetUtcNow();

        if (build.BuildStatus is not FortBuildStatus.LevelUp || time < build.BuildEndDate)
        {
            logger.LogDebug(
                "Building {@Build} has not finished levelling up. Current time: {Time}",
                new
                {
                    build.BuildId,
                    build.PlantId,
                    build.Level,
                    build.BuildStartDate,
                    build.BuildEndDate,
                },
                time
            );
            throw new InvalidOperationException($"This building has not completed levelling up.");
        }

        await FinishUpgrade(build, true);
    }

    private async Task FinishUpgrade(DbFortBuild build, bool levelUp)
    {
        // Update values
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;
        build.Level++;

        if (levelUp)
            await fortMissionProgressionService.OnFortPlantLevelUp(build.PlantId, build.Level);
        else
            await fortMissionProgressionService.OnFortPlantBuild(build.PlantId);
    }

    public async Task<DbFortBuild> BuildStart(FortPlants fortPlantId, int positionX, int positionZ)
    {
        FortPlantDetail plantDetail = MasterAssetUtils.GetInitialFortPlant(fortPlantId);

        await Upgrade(plantDetail);

        DbFortBuild build = new()
        {
            ViewerId = playerIdentityService.ViewerId,
            PlantId = fortPlantId,
            Level = 0,
            PositionX = positionX,
            PositionZ = positionZ,
            IsNew = true,
            LastIncomeDate = DateTimeOffset.UnixEpoch,
        };

        await SetBuildTime(build, plantDetail);

        await fortRepository.AddBuild(build);

        return build;
    }

    public async Task<DbFortBuild> LevelupStart(long buildId)
    {
        logger.LogDebug("Levelup started for build {buildId}", buildId);

        // Get building
        DbFortBuild build = await fortRepository.GetBuilding(buildId);

        FortPlantDetail currentBuilding = MasterAsset.FortPlantDetail[build.FortPlantDetailId];
        if (currentBuilding.NextAssetGroup == 0)
        {
            logger.LogError("Tried to level up build {@build} but it has no next level", build);
            throw new DragaliaException(
                ResultCode.FortPlantDetailNotFound,
                "No next level available for building"
            );
        }

        // Get level up plans
        int buildPlantId = currentBuilding.NextAssetGroup;

        if (
            !MasterAsset.FortPlantDetail.TryGetValue(buildPlantId, out FortPlantDetail? plantDetail)
        )
        {
            // This is unlikely to happen, but best to keep the check just in case

            logger.LogError(
                "Failed to lookup build information for upgrade of build {@build} to level {level}!",
                build,
                build.Level + 1
            );

            throw new DragaliaException(
                ResultCode.FortPlantDetailNotFound,
                "Could not find next building level in data"
            );
        }

        await Upgrade(plantDetail);
        await SetBuildTime(build, plantDetail);

        return build;
    }

    public async Task<DbFortBuild> Move(long buildId, int afterPositionX, int afterPositionZ)
    {
        // Get building
        DbFortBuild build = await fortRepository.GetBuilding(buildId);

        if (build is { PositionX: -1, PositionZ: -1 })
        {
            logger.LogDebug(
                "Placed build {buildId} from storage at {x}/{z}",
                buildId,
                afterPositionX,
                afterPositionZ
            );

            await fortMissionProgressionService.OnFortPlantPlace(build.PlantId);
        }
        else
        {
            logger.LogDebug(
                "Moved build {buildId} to {x}/{z}",
                buildId,
                afterPositionX,
                afterPositionZ
            );
        }

        // Move building to requested coordinate
        build.PositionX = afterPositionX;
        build.PositionZ = afterPositionZ;

        return build;
    }

    private async Task SetBuildTime(DbFortBuild build, FortPlantDetail plantDetail)
    {
        if (plantDetail.Time == 0)
        {
            build.BuildStartDate = build.BuildEndDate = DateTimeOffset.UnixEpoch;
        }
        else
        {
            DateTimeOffset fortOpenTime = await userDataRepository.GetFortOpenTimeAsync();
            DateTimeOffset current = dateTimeProvider.GetUtcNow();

            build.BuildStartDate = current;

            // 1/2 build time for the first 30 days
            build.BuildEndDate =
                fortOpenTime.AddDays(30) > current
                    ? build.BuildStartDate.AddSeconds(Math.Ceiling(plantDetail.Time * 0.5))
                    : build.BuildStartDate.AddSeconds(plantDetail.Time);
        }
    }

    private async Task Upgrade(FortPlantDetail plantDetail)
    {
        FortDetail fortDetail = await GetFortDetail();

        // Check Carpenter available
        if (fortDetail.WorkingCarpenterNum >= fortDetail.CarpenterNum)
        {
            var builds = await fortRepository
                .Builds.Where(x => x.BuildEndDate != DateTimeOffset.UnixEpoch)
                .Select(x => new
                {
                    x.BuildId,
                    x.PlantId,
                    x.Level,
                    x.BuildStartDate,
                    x.BuildEndDate,
                })
                .ToListAsync();

            logger.LogDebug(
                "Failed to perform upgrade {@PlantDetail} - carpenters busy",
                plantDetail
            );
            logger.LogDebug("FortDetail: {@FortDetail}", fortDetail);
            logger.LogDebug("Currently in progress builds: {@Builds}", builds);

            throw new DragaliaException(
                ResultCode.FortBuildCarpenterBusy,
                "All carpenters are currently busy"
            );
        }

        // Remove resources from player
        await paymentService.ProcessPayment(PaymentTypes.Coin, expectedPrice: plantDetail.Cost);
        await inventoryRepository.UpdateQuantity(plantDetail.CreateMaterialMap.Invert());
    }

    private static int GetUpgradePaymentCost(
        PaymentTypes paymentType,
        DateTimeOffset currentTime,
        DateTimeOffset buildEndDate
    )
    {
        if (paymentType == PaymentTypes.HalidomHustleHammer)
            return 1; // Only 1 Hammer is consumed

        // Construction can be immediately completed by spending either Wyrmite or Diamantium,
        // where the amount required depends on the time left until construction is complete.
        // This amount scales at 1 per 12 minutes, or 5 per hour.
        // https://dragalialost.wiki/w/Facilities
        return (int)Math.Ceiling((buildEndDate - currentTime).TotalMinutes / 12);
    }

    public async Task ClearPlantNewStatuses(IEnumerable<FortPlants> plantIds)
    {
        List<FortPlants> ids = plantIds.ToList();

        foreach (DbFortBuild build in await fortRepository.Builds.ToListAsync())
        {
            build.IsNew = ids.Contains(build.PlantId);
        }
    }

    public async Task ClearPlantNewStatuses(IEnumerable<long> buildIds)
    {
        foreach (
            DbFortBuild build in await fortRepository
                .Builds.Where(x => buildIds.Contains(x.BuildId))
                .ToListAsync()
        )
        {
            build.IsNew = false;
        }
    }

    public async Task<AtgenProductionRp> GetRupieProduction()
    {
        float production = 0;
        int max = 0;

        foreach (
            DbFortBuild build in await fortRepository
                .Builds.Where(x => x.PlantId == FortPlants.RupieMine)
                .ToListAsync()
        )
        {
            FortPlantDetail detail = MasterAsset.FortPlantDetail[build.FortPlantDetailId];
            production += detail.CostMax / (float)detail.CostMaxTime;
            max += detail.CostMax;
        }

        return new AtgenProductionRp(production, max);
    }

    public async Task<AtgenProductionRp> GetDragonfruitProduction()
    {
        DbFortBuild? build = await fortRepository.Builds.SingleOrDefaultAsync(x =>
            x.PlantId == FortPlants.Dragontree
        );
        if (build == null)
        {
            return new AtgenProductionRp(0, 0);
        }

        FortPlantDetail detail = MasterAsset.FortPlantDetail[build.FortPlantDetailId];

        return new AtgenProductionRp(
            detail.MaterialMax / (float)detail.MaterialMaxTime,
            detail.MaterialMax
        );
    }

    public async Task<AtgenProductionRp> GetStaminaProduction()
    {
        DbFortBuild? build = await fortRepository.Builds.SingleOrDefaultAsync(x =>
            x.PlantId == FortPlants.TheHalidom
        );
        if (build == null)
        {
            return new AtgenProductionRp(0, 0);
        }

        FortPlantDetail detail = MasterAsset.FortPlantDetail[build.FortPlantDetailId];

        return new AtgenProductionRp(
            detail.StaminaMax / (float)detail.StaminaMaxTime,
            detail.StaminaMax
        );
    }

    public async Task<(int HalidomLevel, int SmithyLevel)> GetCoreLevels()
    {
        IEnumerable<(FortPlants PlantId, int Level)> queryResult = await fortRepository
            .Builds.Where(x => x.PlantId == FortPlants.TheHalidom || x.PlantId == FortPlants.Smithy)
            .Select(x => new ValueTuple<FortPlants, int>(x.PlantId, x.Level))
            .ToListAsync();

        // We will get level = 0 if FirstOrDefault does not find a match.
        int halidomLevel = queryResult
            .FirstOrDefault(x => x.PlantId == FortPlants.TheHalidom)
            .Level;
        int smithyLevel = queryResult.FirstOrDefault(x => x.PlantId == FortPlants.Smithy).Level;

        return (halidomLevel, smithyLevel);
    }
}
