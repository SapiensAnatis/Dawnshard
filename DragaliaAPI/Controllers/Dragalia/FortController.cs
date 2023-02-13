using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("fort")]
public class FortController : DragaliaControllerBase
{
    private readonly IFortRepository fortRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IBonusService bonusService;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IUpdateDataService updateDataService;
    private readonly IMapper mapper;

    public FortController(
        IFortRepository fortRepository,
        IUserDataRepository userDataRepository,
        IBonusService bonusService,
        IInventoryRepository inventoryRepository,
        IUpdateDataService updateDataService,
        IMapper mapper
    )
    {
        this.fortRepository = fortRepository;
        this.userDataRepository = userDataRepository;
        this.bonusService = bonusService;
        this.inventoryRepository = inventoryRepository;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
    }

    [HttpPost("get_data")]
    public async Task<DragaliaResult> GetData()
    {
        IEnumerable<BuildList> buildList = (
            await this.fortRepository.GetBuilds(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<BuildList>);

        FortBonusList bonusList = await this.bonusService.GetBonusList();

        FortDetail fortDetails;
        IQueryable<DbFortDetail> query = this.fortRepository.Details;
        if (!query.Any())
        {
            await this.fortRepository.InitFortDetail(this.DeviceAccountId);
            fortDetails = StubData.FortDetail;
        }
        else
        {
            fortDetails = query.Select(mapper.Map<FortDetail>).First();
        }

        FortGetDataData data =
            new()
            {
                build_list = buildList,
                fort_bonus_list = bonusList,
                dragon_contact_free_gift_count = StubData.DragonFreeGifts,
                production_df = StubData.ProductionDf,
                production_rp = StubData.ProductionRp,
                production_st = StubData.ProductionSt,
                fort_detail = fortDetails,
                current_server_time = DateTime.UtcNow
            };

        return this.Ok(data);
    }

    [HttpPost("add_carpenter")]
    public async Task<DragaliaResult> AddCarpenter(FortAddCarpenterRequest request)
    {
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();
        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();

        if (fortDetail.carpenter_num == fortDetail.max_carpenter_count)
        {
            throw new DragaliaException(
                ResultCode.FortExtendCarpenterLimit,
                $"User has reached maximum carpenter."
            );
        }

        PaymentTypes paymentType = (PaymentTypes)request.payment_type;
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

        ConsumePaymentCost(userData, paymentType, paymentCost);

        // Add carpenter
        fortDetail.carpenter_num++;
        await this.fortRepository.UpdateFortCarpenterNum(
            this.DeviceAccountId, 
            fortDetail.carpenter_num
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        FortAddCarpenterData data =
            new()
            {
                result = 1,
                fort_detail = fortDetail,
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    [HttpPost("build_at_once")]
    public async Task<DragaliaResult> BuildAtOnce(FortBuildAtOnceRequest request)
    {
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();
        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();
        FortBonusList bonusList = await bonusService.GetBonusList();

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId, (long)request.build_id
        );

        PaymentTypes paymentType = (PaymentTypes)request.payment_type;
        int paymentHeld = GetUpgradePaymentHeld(userData, paymentType);
        int paymentCost = GetUpgradePaymentCost(
            paymentType,
            build.BuildStartDate,
            build.BuildEndDate
        );

        if (paymentHeld < paymentCost)
        {
            throw new DragaliaException(
                ResultCode.FortLevelupIncomplete,
                $"User did not have enough {paymentType}."
            );
        }

        ConsumePaymentCost(userData, paymentType, paymentCost);

        // Update build
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;
        this.fortRepository.UpdateBuild(build);

        // Update carpenter usage
        await DecrementCarpenterUsage(fortDetail);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        FortBuildAtOnceData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                fort_bonus_list = bonusList,
                production_rp = StubData.ProductionRp,
                production_st = StubData.ProductionSt,
                production_df = StubData.ProductionDf,
                fort_detail = fortDetail,
                update_data_list = updateDataList,
            };
        return this.Ok(data);
    }

    [HttpPost("build_cancel")]
    public async Task<DragaliaResult> BuildCancel(FortBuildCancelRequest request)
    {
        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId, (long)request.build_id
        );

        // Cancel build
        build.Level--;
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;

        if (build.Level > 0)
        {
            this.fortRepository.UpdateBuild(build);
        }
        else
        {
            this.fortRepository.DeleteBuild(build);
        }

        // Update carpenter usage
        await DecrementCarpenterUsage(fortDetail);

        await this.fortRepository.UpdateFortWorkingCarpenter(this.DeviceAccountId, fortDetail.working_carpenter_num);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        FortBuildCancelData data =
            new()
            {
                result = 1,
                build_id = (ulong)build.BuildId,
                fort_detail = fortDetail,
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    [HttpPost("build_end")]
    public async Task<DragaliaResult> BuildEnd(FortBuildEndRequest request)
    {
        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();
        FortBonusList bonusList = await bonusService.GetBonusList();

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId, (long)request.build_id
        );

        // Update values
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;
        this.fortRepository.UpdateBuild(build);

        // Update carpenter usage
        await DecrementCarpenterUsage(fortDetail);

        await this.fortRepository.UpdateFortWorkingCarpenter(this.DeviceAccountId, fortDetail.working_carpenter_num);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        FortBuildEndData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                fort_bonus_list = bonusList,
                production_rp = StubData.ProductionRp,
                production_st = StubData.ProductionSt,
                production_df = StubData.ProductionDf,
                fort_detail = fortDetail,
                update_data_list = updateDataList,
            };
        return this.Ok(data);
    }

    [HttpPost("build_start")]
    public async Task<DragaliaResult> BuildStart(FortBuildStartRequest request)
    {
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();

        IQueryable<DbPlayerMaterial> userMaterials = this.inventoryRepository
            .GetMaterials(this.DeviceAccountId);

        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();

        // Check Carpenter available
        if (fortDetail.working_carpenter_num > fortDetail.carpenter_num)
        {
            throw new DragaliaException(
                ResultCode.FortBuildCarpenterBusy,
                $"All carpenters are currently busy"
            );
        }

        // Get build plans
        FortPlants BuildPlantId = (FortPlants)request.fort_plant_id;
        int buildIdAtLevel1 = MasterAssetUtils.GetPlantDetailId(BuildPlantId, 1);
        FortPlantDetail plantDetail = MasterAsset.FortPlant.Get(buildIdAtLevel1);

        // Remove player resources
        await ConsumePlayerMaterials(userMaterials, userData, plantDetail);

        // Start building
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate.AddSeconds(plantDetail.Time);

        DbFortBuild build = new()
        {
            DeviceAccountId = this.DeviceAccountId,
            PlantId = BuildPlantId,
            Level = 1,
            PositionX = request.position_x,
            PositionZ = request.position_z,
            BuildStartDate = startDate,
            BuildEndDate = endDate,
            IsNew = true,
            LastIncomeDate = DateTimeOffset.UnixEpoch
        };
        await this.fortRepository.AddBuild(build);

        // Increment worker carpenters
        await IncrementCarpenterUsage(fortDetail);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        int startDateUnix = (int)((DateTimeOffset)startDate).ToUnixTimeSeconds();
        int endDateUnix = (int)((DateTimeOffset)endDate).ToUnixTimeSeconds();
        FortBuildStartData data =
            new()
            {
                result = 1,
                build_id = (ulong)build.BuildId,
                build_start_date = startDateUnix,
                build_end_date = endDateUnix,
                remain_time = endDateUnix - startDateUnix,
                fort_detail = fortDetail,
                update_data_list = updateDataList,
                entity_result = new EntityResult() // What does it do?
            };
        return this.Ok(data);
    }

    [HttpPost("levelup_at_once")]
    public async Task<DragaliaResult> LevelUpAtOnce(FortLevelupAtOnceRequest request)
    {
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();
        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();
        FortBonusList bonusList = await bonusService.GetBonusList();

        IQueryable<DbFortBuild> builds = this.fortRepository.Builds;
        DbFortBuild halidom = builds.First(x => x.PlantId == FortPlants.TheHalidom);
        DbFortBuild smithy = builds.First(x => x.PlantId == FortPlants.Smithy);

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId, (long)request.build_id
        );

        PaymentTypes paymentType = (PaymentTypes)request.payment_type;
        int paymentHeld = GetUpgradePaymentHeld(userData, paymentType);
        int paymentCost = GetUpgradePaymentCost(
            paymentType, 
            build.BuildStartDate, 
            build.BuildEndDate
        );

        if (paymentHeld < paymentCost)
        {
            throw new DragaliaException(
                ResultCode.FortLevelupIncomplete,
                $"User did not have enough {request.payment_type}."
            );
        }

        ConsumePaymentCost(userData, paymentType, paymentCost);

        // Update build
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;
        this.fortRepository.UpdateBuild(build);

        // Update carpenter usage
        await DecrementCarpenterUsage(fortDetail);

        await this.fortRepository.UpdateFortWorkingCarpenter(this.DeviceAccountId, fortDetail.working_carpenter_num);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        FortLevelupAtOnceData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                current_fort_level = halidom.Level,
                current_fort_craft_level = smithy.Level,
                fort_bonus_list = bonusList,
                production_rp = StubData.ProductionRp,
                production_st = StubData.ProductionSt,
                production_df = StubData.ProductionDf,
                fort_detail = fortDetail,
                update_data_list = updateDataList,
            };
        return this.Ok(data);
    }

    [HttpPost("levelup_cancel")]
    public async Task<DragaliaResult> LevelUpCancel(FortLevelupCancelRequest request)
    {
        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId, (long)request.build_id
        );

        // Cancel build
        build.Level--;
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;

        if (build.Level > 0)
        {
            this.fortRepository.UpdateBuild(build);
        }
        else
        {
            this.fortRepository.DeleteBuild(build);
        }

        // Update carpenter usage
        await DecrementCarpenterUsage(fortDetail);

        await this.fortRepository.UpdateFortWorkingCarpenter(this.DeviceAccountId, fortDetail.working_carpenter_num);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        FortLevelupCancelData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                fort_detail = fortDetail,
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    [HttpPost("levelup_end")]
    public async Task<DragaliaResult> LevelUpEnd(FortLevelupEndRequest request)
    {
        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();

        FortBonusList bonusList = await bonusService.GetBonusList();
        IQueryable<DbFortBuild> builds = this.fortRepository.Builds;
        DbFortBuild halidom = builds.First(x => x.PlantId == FortPlants.TheHalidom);
        DbFortBuild smithy = builds.First(x => x.PlantId == FortPlants.Smithy);

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId, (long)request.build_id
        );

        // Update values
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;
        this.fortRepository.UpdateBuild(build);

        // Update carpenter usage
        await DecrementCarpenterUsage(fortDetail);

        await this.fortRepository.UpdateFortWorkingCarpenter(this.DeviceAccountId, fortDetail.working_carpenter_num);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        FortLevelupEndData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                current_fort_level = halidom.Level,
                current_fort_craft_level = smithy.Level,
                fort_bonus_list = bonusList,
                production_rp = StubData.ProductionRp,
                production_st = StubData.ProductionSt,
                production_df = StubData.ProductionDf,
                fort_detail = fortDetail,
                update_data_list = updateDataList,
            };
        return this.Ok(data);
    }

    [HttpPost("levelup_start")]
    public async Task<DragaliaResult> LevelUpStart(FortLevelupStartRequest request)
    {
        DbPlayerUserData userData = await this.userDataRepository
            .GetUserData(this.DeviceAccountId)
            .FirstAsync();

        IQueryable<DbPlayerMaterial> userMaterials = this.inventoryRepository
            .GetMaterials(this.DeviceAccountId);

        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();

        // Check Carpenter available
        if (fortDetail.working_carpenter_num > fortDetail.carpenter_num)
        {
            throw new DragaliaException(
                ResultCode.FortBuildCarpenterBusy,
                $"All carpenters are currently busy"
            );
        }

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId, (long)request.build_id
        );

        // Get level up plans (current FortPlantDetailId +1 to get plans of the next level)
        int targetBuildingId = build.FortPlantDetailId + 1;
        FortPlantDetail plantDetail = MasterAsset.FortPlant.Get(targetBuildingId);

        // Remove resources from player
        await ConsumePlayerMaterials(userMaterials, userData, plantDetail);

        // Start level up
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate.AddSeconds(plantDetail.Time);

        build.Level += plantDetail.NeedLevel;
        build.BuildStartDate = startDate;
        build.BuildEndDate = endDate;
        this.fortRepository.UpdateBuild(build);

        // Update carpenter usage
        await IncrementCarpenterUsage(fortDetail);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        int startDateUnix = (int)((DateTimeOffset)startDate).ToUnixTimeSeconds();
        int endDateUnix = (int)((DateTimeOffset)endDate).ToUnixTimeSeconds();
        FortLevelupStartData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                levelup_start_date = startDateUnix,
                levelup_end_date = endDateUnix,
                remain_time = endDateUnix - startDateUnix,
                fort_detail = fortDetail,
                update_data_list = updateDataList,
                entity_result = new EntityResult() // What does it do?
            };
        return this.Ok(data);
    }

    [HttpPost("move")]
    public async Task<DragaliaResult> Move(FortMoveRequest request)
    {
        FortDetail fortDetail = this.fortRepository.Details.Select(mapper.Map<FortDetail>).First();
        FortBonusList bonusList = await bonusService.GetBonusList();

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId, (long)request.build_id
        );

        // Move building to requested coordinate
        build.PositionX = request.after_position_x;
        build.PositionZ = request.after_position_z;
        this.fortRepository.UpdateBuild(build);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.fortRepository.SaveChangesAsync();

        FortMoveData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                fort_bonus_list = bonusList,
                production_rp = StubData.ProductionRp,
                production_df = StubData.ProductionDf,
                production_st = StubData.ProductionSt,
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    // What exactly does it do
    [HttpPost("set_new_fort_plant")]
    public async Task<DragaliaResult> SetNewFortPlant(FortSetNewFortPlantRequest request)
    {
        await this.fortRepository.GetFortPlantIdList(request.fort_plant_id_list);
        
        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );
        
        FortSetNewFortPlantData data =
            new()
            {
                result = 1,
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    private async Task IncrementCarpenterUsage(FortDetail fortDetail)
    {
        fortDetail.working_carpenter_num++;
        await this.fortRepository.UpdateFortWorkingCarpenter(
            this.DeviceAccountId, 
            fortDetail.working_carpenter_num
        );
    }

    private async Task DecrementCarpenterUsage(FortDetail fortDetail)
    {
        fortDetail.working_carpenter_num--;
        if (fortDetail.working_carpenter_num < 0)
        {
            fortDetail.working_carpenter_num = 0;
        }

        await this.fortRepository.UpdateFortWorkingCarpenter(
            this.DeviceAccountId, 
            fortDetail.working_carpenter_num
        );
    }

    private async Task ConsumePlayerMaterials(IQueryable<DbPlayerMaterial> userMaterials, DbPlayerUserData userData, FortPlantDetail plantDetail)
    {
        userData.Coin -= plantDetail.Cost;

        // Is there maybe a more efficient way to do this? Seems ugly, but works
        await ConsumeMaterial(userMaterials, plantDetail.MaterialsId1, plantDetail.MaterialsNum1);
        await ConsumeMaterial(userMaterials, plantDetail.MaterialsId2, plantDetail.MaterialsNum2);
        await ConsumeMaterial(userMaterials, plantDetail.MaterialsId3, plantDetail.MaterialsNum3);
        await ConsumeMaterial(userMaterials, plantDetail.MaterialsId4, plantDetail.MaterialsNum4);
        await ConsumeMaterial(userMaterials, plantDetail.MaterialsId5, plantDetail.MaterialsNum5);
    }

    private async Task ConsumeMaterial(IQueryable<DbPlayerMaterial> userMaterials, Materials material, int cost)
    {
        if (material != Materials.Empty)
        {
            DbPlayerMaterial dbMaterial = await userMaterials.FirstAsync(x => x.MaterialId == material);
            await this.inventoryRepository.UpdateQuantity(this.DeviceAccountId, dbMaterial.MaterialId, -cost);
        }
    }

    private int GetUpgradePaymentHeld(DbPlayerUserData userData, PaymentTypes paymentType)
    {
        return paymentType switch
        {
            PaymentTypes.Wyrmite => userData.Crystal,
            PaymentTypes.Diamantium => 0,// TODO How do I diamantium?
            PaymentTypes.HalidomHustleHammer => userData.BuildTimePoint,
            _ => throw new DragaliaException(
                                ResultCode.FortBuildNotStart,
                                $"Invalid payment type for this operation."
                            ),
        };
    }

    private int GetUpgradePaymentCost(PaymentTypes paymentType, DateTimeOffset BuildStartDate, DateTimeOffset BuildEndDate)
    {
        if (paymentType == PaymentTypes.HalidomHustleHammer)
        {
            return 1; // Only 1 Hammer is consumed
        }
        else
        {
            // Construction can be immediately completed by spending either Wyrmite or Diamantium,
            // where the amount required depends on the time left until construction is complete.
            // This amount scales at 1 per 12 minutes, or 5 per hour. 
            // https://dragalialost.wiki/w/Facilities
            return (int)Math.Floor((BuildEndDate - BuildStartDate).TotalMinutes / 12);
        }
    }

    private void ConsumePaymentCost(DbPlayerUserData userData, PaymentTypes paymentType, int paymentCost)
    {
        switch (paymentType)
        {
            case PaymentTypes.Wyrmite:
                userData.Crystal -= paymentCost;
                break;
            case PaymentTypes.Diamantium:
                // TODO How do I diamantium?
                break;
            case PaymentTypes.HalidomHustleHammer:
                userData.BuildTimePoint -= paymentCost;
                break;
            default:
                throw new DragaliaException(
                    ResultCode.FortBuildNotStart,
                    $"User did not have enough {paymentType}."
                );
        }
    }

    private static class StubData
    {
        public static readonly FortDetail FortDetail =
            new()
            {
                carpenter_num = 5,
                max_carpenter_count = 5,
                working_carpenter_num = 0
            };

        public static readonly AtgenProductionRp ProductionRp = new();

        public static readonly AtgenProductionRp ProductionDf = new();

        public static readonly AtgenProductionRp ProductionSt = new() { speed = 0.03f, max = 144 };

        public const int DragonFreeGifts = 1;
    }
}
