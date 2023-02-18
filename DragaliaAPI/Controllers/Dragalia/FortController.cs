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
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );
        IEnumerable<BuildList> buildList = (
            await this.fortRepository.GetBuilds(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<BuildList>);

        FortBonusList bonusList = await this.bonusService.GetBonusList();

        FortGetDataData data =
            new()
            {
                build_list = buildList,
                fort_bonus_list = bonusList,
                dragon_contact_free_gift_count = StubData.DragonFreeGifts,
                production_df = StubData.ProductionDf,
                production_rp = StubData.ProductionRp,
                production_st = StubData.ProductionSt,
                fort_detail = fortDetail,
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
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );

        if (fortDetail.carpenter_num == FortRepository.MaximumCarpenterNum)
        {
            throw new DragaliaException(
                ResultCode.FortExtendCarpenterLimit,
                $"User has reached maximum carpenter."
            );
        }

        PaymentTypes paymentType = request.payment_type;
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
        await this.fortRepository.UpdateFortMaximumCarpenter(
            this.DeviceAccountId,
            fortDetail.carpenter_num
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.updateDataService.SaveChangesAsync();

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
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );
        FortBonusList bonusList = await bonusService.GetBonusList();

        PaymentTypes paymentType = (PaymentTypes)request.payment_type;
        await this.fortRepository.UpgradeAtOnce(
            userData,
            this.DeviceAccountId,
            (long)request.build_id,
            paymentType
        );

        fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.UpdateCarpenterUsage(this.DeviceAccountId)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.updateDataService.SaveChangesAsync();

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
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );
        DbFortBuild cancelledBuild = await this.fortRepository.CancelUpgrade(
            this.DeviceAccountId,
            (long)request.build_id
        );

        fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.UpdateCarpenterUsage(this.DeviceAccountId)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );
        updateDataList.functional_maintenance_list = new List<FunctionalMaintenanceList>();

        await this.updateDataService.SaveChangesAsync();

        FortBuildCancelData data =
            new()
            {
                result = 1,
                build_id = (ulong)cancelledBuild.BuildId,
                fort_detail = fortDetail,
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    [HttpPost("build_end")]
    public async Task<DragaliaResult> BuildEnd(FortBuildEndRequest request)
    {
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );
        FortBonusList bonusList = await bonusService.GetBonusList();

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId,
            (long)request.build_id
        );

        // Update values
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;

        fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.UpdateCarpenterUsage(this.DeviceAccountId)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.updateDataService.SaveChangesAsync();

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
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );

        IQueryable<DbPlayerMaterial> userMaterials = this.inventoryRepository.GetMaterials(
            this.DeviceAccountId
        );

        // Check Carpenter available
        if (fortDetail.working_carpenter_num > fortDetail.carpenter_num)
        {
            throw new DragaliaException(
                ResultCode.FortBuildCarpenterBusy,
                $"All carpenters are currently busy"
            );
        }

        // Get build plans
        FortPlants BuildPlantId = request.fort_plant_id;
        int buildIdAtLevel1 = MasterAssetUtils.GetPlantDetailId(BuildPlantId, 1);
        FortPlantDetail plantDetail = MasterAsset.FortPlant.Get(buildIdAtLevel1);

        // Remove player resources
        userData.Coin -= plantDetail.Cost;
        IEnumerable<KeyValuePair<Materials, int>> quantityMap = plantDetail.CreateMaterialMap;
        await ConsumePlayerMaterials(userMaterials, quantityMap);

        // Start building
        DateTime startDate = DateTime.UtcNow;
        DateTime endDate = startDate.AddSeconds(plantDetail.Time);

        DbFortBuild build =
            new()
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
        fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.UpdateCarpenterUsage(this.DeviceAccountId)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.updateDataService.SaveChangesAsync();

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
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );

        FortBonusList bonusList = await bonusService.GetBonusList();

        IQueryable<DbFortBuild> builds = this.fortRepository.Builds;
        DbFortBuild halidom = builds.First(x => x.PlantId == FortPlants.TheHalidom);
        DbFortBuild smithy = builds.First(x => x.PlantId == FortPlants.Smithy);

        PaymentTypes paymentType = (PaymentTypes)request.payment_type;

        await this.fortRepository.UpgradeAtOnce(
            userData,
            this.DeviceAccountId,
            (long)request.build_id,
            paymentType
        );

        fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.UpdateCarpenterUsage(this.DeviceAccountId)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.updateDataService.SaveChangesAsync();

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
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );

        DbFortBuild cancelledBuild = await this.fortRepository.CancelUpgrade(
            this.DeviceAccountId,
            (long)request.build_id
        );

        fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.UpdateCarpenterUsage(this.DeviceAccountId)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.updateDataService.SaveChangesAsync();

        FortLevelupCancelData data =
            new()
            {
                result = 1,
                build_id = (ulong)cancelledBuild.BuildId,
                fort_detail = fortDetail,
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    [HttpPost("levelup_end")]
    public async Task<DragaliaResult> LevelUpEnd(FortLevelupEndRequest request)
    {
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );

        FortBonusList bonusList = await bonusService.GetBonusList();
        IQueryable<DbFortBuild> builds = this.fortRepository.Builds;
        DbFortBuild halidom = builds.First(x => x.PlantId == FortPlants.TheHalidom);
        DbFortBuild smithy = builds.First(x => x.PlantId == FortPlants.Smithy);

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId,
            (long)request.build_id
        );

        // Update values
        build.BuildStartDate = DateTimeOffset.UnixEpoch;
        build.BuildEndDate = DateTimeOffset.UnixEpoch;

        fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.UpdateCarpenterUsage(this.DeviceAccountId)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.updateDataService.SaveChangesAsync();

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
        FortDetail fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.GetFortDetails()
        );

        IQueryable<DbPlayerMaterial> userMaterials = this.inventoryRepository.GetMaterials(
            this.DeviceAccountId
        );

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
            this.DeviceAccountId,
            (long)request.build_id
        );

        // Get level up plans (current FortPlantDetailId +1 to get plans of the next level)
        int targetBuildingId = build.FortPlantDetailId + 1;
        FortPlantDetail plantDetail = MasterAsset.FortPlant.Get(targetBuildingId);

        // Remove resources from player
        userData.Coin -= plantDetail.Cost;
        IEnumerable<KeyValuePair<Materials, int>> quantityMap = plantDetail.CreateMaterialMap;
        await ConsumePlayerMaterials(userMaterials, quantityMap);

        // Start level up
        DateTimeOffset startDate = DateTimeOffset.UtcNow;
        DateTimeOffset endDate = startDate.AddSeconds(plantDetail.Time);

        build.Level += plantDetail.NeedLevel;
        build.BuildStartDate = startDate;
        build.BuildEndDate = endDate;

        // Increment carpenter usage
        fortDetail = this.mapper.Map<FortDetail>(
            await this.fortRepository.UpdateCarpenterUsage(this.DeviceAccountId)
        );

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.updateDataService.SaveChangesAsync();

        FortLevelupStartData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                levelup_start_date = startDate,
                levelup_end_date = endDate,
                remain_time = endDate - startDate,
                fort_detail = fortDetail,
                update_data_list = updateDataList,
                entity_result = new EntityResult() // What does it do?
            };
        return this.Ok(data);
    }

    [HttpPost("move")]
    public async Task<DragaliaResult> Move(FortMoveRequest request)
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        // Get building
        DbFortBuild build = await this.fortRepository.GetBuilding(
            this.DeviceAccountId,
            (long)request.build_id
        );

        // Move building to requested coordinate
        build.PositionX = request.after_position_x;
        build.PositionZ = request.after_position_z;

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        await this.updateDataService.SaveChangesAsync();

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

        FortSetNewFortPlantData data = new() { result = 1, update_data_list = updateDataList };
        return this.Ok(data);
    }

    private async Task<bool> ConsumePlayerMaterials(
        IQueryable<DbPlayerMaterial> userMaterials,
        IEnumerable<KeyValuePair<Materials, int>> quantityMap
    )
    {
        foreach (KeyValuePair<Materials, int> requested in quantityMap)
        {
            if (requested.Key == Materials.Empty)
                continue;

            DbPlayerMaterial dbMaterial = await userMaterials.FirstAsync(
                x => x.MaterialId == requested.Key
            );
            await this.inventoryRepository.UpdateQuantity(
                this.DeviceAccountId,
                dbMaterial.MaterialId,
                -requested.Value
            );
        }

        return true;
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
