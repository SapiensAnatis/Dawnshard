using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

public class FortService : IFortService
{
    public const int MaximumCarpenterNum = 5;

    private readonly IFortRepository fortRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly ILogger<FortService> logger;
    private readonly IMapper mapper;

    public FortService(
        IFortRepository fortRepository,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        ILogger<FortService> logger,
        IMapper mapper
    )
    {
        this.fortRepository = fortRepository;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.logger = logger;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<BuildList>> GetBuildList()
    {
        return (await this.fortRepository.Builds.ToListAsync()).Select(mapper.Map<BuildList>);
    }

    public async Task<FortDetail> AddCarpenter(string accountId, PaymentTypes paymentType)
    {
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(accountId)
            .FirstAsync();

        FortDetail fortDetail = await this.GetFortDetail();

        if (fortDetail.carpenter_num == MaximumCarpenterNum)
        {
            throw new DragaliaException(
                ResultCode.FortExtendCarpenterLimit,
                $"User has reached maximum carpenter."
            );
        }

        int paymentHeld = 0;
        // https://dragalialost.wiki/w/Facilities
        // First 2 are free, 3rd 250, 4th 400, 5th 700
        int paymentCost = 250;
        switch (fortDetail.carpenter_num)
        {
            case 3:
                paymentCost = 400;
                break;
            case 4:
                paymentCost = 700;
                break;
        }

        switch (paymentType)
        {
            case PaymentTypes.Wyrmite:
                paymentHeld = userData.Crystal;
                break;
            case PaymentTypes.Diamantium:
                // TODO How do I diamantium?
                break;
            default:
                throw new DragaliaException(
                    ResultCode.FortExtendCarpenterLimit,
                    $"Invalid currency used to add carpenter."
                );
        }

        if (paymentHeld < paymentCost)
        {
            throw new DragaliaException(
                ResultCode.FortExtendCarpenterLimit,
                $"User did not have enough {paymentType}."
            );
        }

        this.fortRepository.ConsumePaymentCost(userData, paymentType, paymentCost);

        // Add carpenter
        fortDetail.carpenter_num++;
        await this.fortRepository.UpdateFortMaximumCarpenter(fortDetail.carpenter_num);

        return fortDetail;
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
        DbFortDetail dbDetail = await this.fortRepository.GetFortDetails();
        int activeCarpenters = await this.fortRepository.GetActiveCarpenters();

        return new()
        {
            max_carpenter_count = MaximumCarpenterNum,
            working_carpenter_num = activeCarpenters,
            carpenter_num = dbDetail.CarpenterNum
        };
    }

    public async Task CompleteAtOnce(string accountId, PaymentTypes paymentType, long buildId)
    {
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(accountId)
            .FirstAsync();

        await this.fortRepository.UpgradeAtOnce(userData, buildId, paymentType);
    }

    public async Task<DbFortBuild> CancelUpgrade(long buildId)
    {
        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(buildId);

        if (build.BuildEndDate == DateTimeOffset.UnixEpoch)
        {
            throw new InvalidOperationException($"This building is not currently being upgraded.");
        }

        // Cancel build
        build.Level--;
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;

        if (build.Level == 0)
        {
            this.fortRepository.DeleteBuild(build);
        }

        return build;
    }

    public async Task EndUpgrade(long buildId)
    {
        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(buildId);

        // Update values
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;
    }

    public async Task<DbFortBuild> BuildStart(
        string accountId,
        FortPlants fortPlantId,
        int level,
        int positionX,
        int positionZ
    )
    {
        // Get build plans
        int buildPlantId = MasterAssetUtils.GetPlantDetailId(fortPlantId, level);
        FortPlantDetail plantDetail = MasterAsset.FortPlant.Get(buildPlantId);

        // Start building
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate.AddSeconds(plantDetail.Time).AddSeconds(10);

        DbFortBuild build =
            new()
            {
                DeviceAccountId = accountId,
                PlantId = fortPlantId,
                Level = 1,
                PositionX = positionX,
                PositionZ = positionZ,
                BuildStartDate = startDate,
                BuildEndDate = endDate,
                IsNew = true,
                LastIncomeDate = DateTimeOffset.UnixEpoch
            };

        await Upgrade(accountId, build, plantDetail);

        return build;
    }

    public async Task<DbFortBuild> LevelupStart(string accountId, long buildId)
    {
        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(buildId);

        // Get level up plans (current level+1 to get plans of the next level)
        int buildPlantId = MasterAssetUtils.GetPlantDetailId(build.PlantId, build.Level + 1);
        FortPlantDetail plantDetail = MasterAsset.FortPlant.Get(buildPlantId);

        // Start level up
        DateTimeOffset startDate = DateTimeOffset.UtcNow;
        DateTimeOffset endDate = startDate.AddSeconds(plantDetail.Time);

        build.Level += 1;
        build.BuildStartDate = startDate;
        build.BuildEndDate = endDate;

        await Upgrade(accountId, build, plantDetail);

        return build;
    }

    public async Task<DbFortBuild> Move(long buildId, int afterPositionX, int afterPositionZ)
    {
        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(buildId);

        // Move building to requested coordinate
        build.PositionX = afterPositionX;
        build.PositionZ = afterPositionZ;

        return build;
    }

    public async Task GetFortPlantIdList(IEnumerable<int> fortPlantIdList)
    {
        await this.fortRepository.GetFortPlantIdList(fortPlantIdList);
    }

    private async Task Upgrade(string accountId, DbFortBuild build, FortPlantDetail plantDetail)
    {
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(accountId)
            .FirstAsync();

        FortDetail fortDetail = await this.GetFortDetail();

        // Get Materials
        IQueryable<DbPlayerMaterial> userMaterials = this.inventoryRepository.GetMaterials(
            accountId
        );

        // Check Carpenter available
        if (fortDetail.working_carpenter_num > fortDetail.carpenter_num)
        {
            throw new DragaliaException(
                ResultCode.FortBuildCarpenterBusy,
                $"All carpenters are currently busy"
            );
        }

        // Remove resources from player
        userData.Coin -= plantDetail.Cost;
        IEnumerable<KeyValuePair<Materials, int>> quantityMap = plantDetail.CreateMaterialMap;
        await this.inventoryRepository.UpdateQuantity(quantityMap);

        if (build.Level == 1)
        {
            // Only has to be added if it is being created for the first time
            await this.fortRepository.AddBuild(build);
        }
    }
}
