using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("fort")]
public class FortController : DragaliaControllerBase
{
    private readonly IFortService fortService;
    private readonly IBonusService bonusService;
    private readonly IUpdateDataService updateDataService;
    private readonly IUserDataRepository userDataRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IFortRepository fortRepository;

    public FortController(
        IFortService fortService,
        IBonusService bonusService,
        IUpdateDataService updateDataService,
        IUserDataRepository userDataRepository,
        IInventoryRepository inventoryRepository,
        IFortRepository fortRepository
    )
    {
        this.fortService = fortService;
        this.bonusService = bonusService;
        this.updateDataService = updateDataService;
        this.userDataRepository = userDataRepository;
        this.inventoryRepository = inventoryRepository;
        this.fortRepository = fortRepository;
    }

    [HttpPost("get_data")]
    public async Task<DragaliaResult> GetData()
    {
        FortDetail fortDetail = await this.fortService.GetFortDetail();
        IEnumerable<BuildList> buildList = await this.fortService.GetBuildList();

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
                current_server_time = DateTimeOffset.UtcNow
            };

        return this.Ok(data);
    }

    [HttpPost("get_multi_income")]
    public async Task<DragaliaResult> GetMultiIncome(FortGetMultiIncomeRequest request)
    {
        Random rng = new(); // for dragonfruit drops

        // AtgenHarvestBuildList: buildId, AddHarvestList
        // AddHarvestList: materialId, fruit quantity
        List<AtgenHarvestBuildList> harvest = new();

        // AtgenAddCoinList: buildId, rupie quantity
        List<AtgenAddCoinList> addCoin = new();

        // AtgenAddStaminaList: buildId, stamina count
        List<AtgenAddStaminaList> addStamina = new();

        IEnumerable<DbFortBuild> fortList = await this.fortRepository.Builds.ToListAsync();
        IEnumerable<DbFortBuild> resourceFortList = fortList.Where(build => 
            request.build_id_list.Contains((ulong)build.BuildId));

        // materials to add to player inventory
        int rupiesAdded = 0;
        List<KeyValuePair<Materials, int>> addDragonfruitList = new();

        foreach (DbFortBuild build in resourceFortList)
        {
            FortPlantDetail fortPlantDetail = MasterAsset.FortPlant.Get(build.FortPlantDetailId);
            int elapsedSecs = (int) build.LastIncomeTime.TotalSeconds;
            switch (build.PlantId)
            {
                case FortPlants.RupieMine:
                    elapsedSecs = Math.Min(fortPlantDetail.CostMaxTime, elapsedSecs);
                    double rupiesPerSec = fortPlantDetail.CostMax / (fortPlantDetail.CostMaxTime * 1.0);
                    int rupiesCount = (int)(rupiesPerSec * elapsedSecs);
                    addCoin.Add(new AtgenAddCoinList((ulong) build.BuildId, rupiesCount));
                    rupiesAdded += rupiesCount;
                    break;
                case FortPlants.Dragontree:
                    elapsedSecs = Math.Min(fortPlantDetail.MaterialMaxTime, elapsedSecs);
                    double fruitsPerSec = fortPlantDetail.MaterialMax / (fortPlantDetail.MaterialMaxTime * 1.0);
                    int fruitsCount = (int)(fruitsPerSec * elapsedSecs);
                    int dragonfruitCount = 0;
                    int ripeDragonfruitCount = 0;
                    int succulentDragonfruitCount = 0;
                    // randomly assign dragonfruit drops 
                    // these arent supposed to be equally distrubuted...
                    // but I don't think anyone really cares
                    for (int i = 0; i < fruitsCount; i++)
                    {
                        switch (rng.Next(3))
                        {
                            case 0:
                                dragonfruitCount++;
                                break;
                            case 1:
                                ripeDragonfruitCount++;
                                break;
                            case 2:
                                succulentDragonfruitCount++;
                                break;
                        }
                    }
                    
                    List<AtgenAddHarvestList> addHarvest = new()
                    {
                        new AtgenAddHarvestList((int) Materials.Dragonfruit, dragonfruitCount),
                        new AtgenAddHarvestList((int) Materials.RipeDragonfruit, ripeDragonfruitCount),
                        new AtgenAddHarvestList((int) Materials.SucculentDragonfruit, succulentDragonfruitCount)
                    };
                    harvest.Add(new AtgenHarvestBuildList((ulong)build.BuildId, addHarvest));

                    addDragonfruitList.Add(new KeyValuePair<Materials, int>(Materials.Dragonfruit, dragonfruitCount));
                    addDragonfruitList.Add(new KeyValuePair<Materials, int>(Materials.RipeDragonfruit, ripeDragonfruitCount));
                    addDragonfruitList.Add(new KeyValuePair<Materials, int>(Materials.SucculentDragonfruit, succulentDragonfruitCount));
                    break;
                case FortPlants.TheHalidom:
                    elapsedSecs = Math.Min(fortPlantDetail.StaminaMaxTime, elapsedSecs);
                    double staminaPerSec = fortPlantDetail.StaminaMax / (fortPlantDetail.StaminaMaxTime * 1.0);
                    int stamina = (int)(staminaPerSec * elapsedSecs);
                    addStamina.Add(new AtgenAddStaminaList((ulong) build.BuildId, stamina));
                    //verify that collecting stamina works properly after stamina is implemented
                    break;
                default:
                    throw new DragaliaException(
                        ResultCode.FortIncomeError,
                        $"Attempted to collect resources from non-resource facility"
                    );
            }
            build.LastIncomeDate = DateTimeOffset.UtcNow;
        }

        await this.userDataRepository.UpdateCoin(rupiesAdded);
        await this.inventoryRepository.UpdateQuantity(addDragonfruitList);

        UpdateDataList updateDataList = updateDataService.GetUpdateDataList(DeviceAccountId);
        await this.userDataRepository.SaveChangesAsync();
        FortGetMultiIncomeData data =
            new()
            {
                result = 1,
                harvest_build_list = harvest,
                add_coin_list = addCoin,
                add_stamina_list = addStamina,
                is_over_coin = 0,
                is_over_material = 0,
                update_data_list = updateDataList,
                entity_result = new()
            };

        return this.Ok(data);
    }

    [HttpPost("add_carpenter")]
    public async Task<DragaliaResult> AddCarpenter(FortAddCarpenterRequest request)
    {
        await this.fortService.AddCarpenter(request.payment_type);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

        FortAddCarpenterData data =
            new()
            {
                result = 1,
                fort_detail = await this.fortService.GetFortDetail(),
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    [HttpPost("build_at_once")]
    public async Task<DragaliaResult> BuildAtOnce(FortBuildAtOnceRequest request)
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await this.fortService.CompleteAtOnce(request.payment_type, request.build_id);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await this.fortService.GetFortDetail();

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
        DbFortBuild cancelledBuild = await this.fortService.CancelUpgrade(request.build_id);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await this.fortService.GetFortDetail();

        FortBuildCancelData data =
            new()
            {
                result = 1,
                build_id = cancelledBuild.BuildId,
                fort_detail = fortDetail,
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    [HttpPost("build_end")]
    public async Task<DragaliaResult> BuildEnd(FortBuildEndRequest request)
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await this.fortService.EndUpgrade(request.build_id);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await this.fortService.GetFortDetail();

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
        DbFortBuild build = await this.fortService.BuildStart(
            request.fort_plant_id,
            1, // Build always starts at 1
            request.position_x,
            request.position_z
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await this.fortService.GetFortDetail();

        FortBuildStartData data =
            new()
            {
                result = 1,
                build_id = (ulong)build.BuildId,
                build_start_date = build.BuildStartDate,
                build_end_date = build.BuildEndDate,
                remain_time = build.RemainTime,
                fort_detail = fortDetail,
                update_data_list = updateDataList,
                entity_result = new EntityResult() // What does it do?
            };
        return this.Ok(data);
    }

    [HttpPost("levelup_at_once")]
    public async Task<DragaliaResult> LevelupAtOnce(FortLevelupAtOnceRequest request)
    {
        FortBonusList bonusList = await bonusService.GetBonusList();
        IEnumerable<BuildList> builds = await this.fortService.GetBuildList();
        BuildList halidom = builds.First(x => x.plant_id == FortPlants.TheHalidom);
        BuildList smithy = builds.First(x => x.plant_id == FortPlants.Smithy);

        await this.fortService.CompleteAtOnce(request.payment_type, request.build_id);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await this.fortService.GetFortDetail();

        FortLevelupAtOnceData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                current_fort_level = halidom.level,
                current_fort_craft_level = smithy.level,
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
    public async Task<DragaliaResult> LevelupCancel(FortLevelupCancelRequest request)
    {
        DbFortBuild cancelledBuild = await this.fortService.CancelUpgrade(request.build_id);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await this.fortService.GetFortDetail();

        FortLevelupCancelData data =
            new()
            {
                result = 1,
                build_id = cancelledBuild.BuildId,
                fort_detail = fortDetail,
                update_data_list = updateDataList
            };
        return this.Ok(data);
    }

    [HttpPost("levelup_end")]
    public async Task<DragaliaResult> LevelupEnd(FortLevelupEndRequest request)
    {
        IEnumerable<BuildList> builds = await this.fortService.GetBuildList();
        BuildList halidom = builds.First(x => x.plant_id == FortPlants.TheHalidom);
        BuildList smithy = builds.First(x => x.plant_id == FortPlants.Smithy);

        FortBonusList bonusList = await bonusService.GetBonusList();

        await this.fortService.EndUpgrade(request.build_id);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await this.fortService.GetFortDetail();

        FortLevelupEndData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                current_fort_level = halidom.level,
                current_fort_craft_level = smithy.level,
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
    public async Task<DragaliaResult> LevelupStart(FortLevelupStartRequest request)
    {
        DbFortBuild build = await this.fortService.LevelupStart(request.build_id);

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await this.fortService.GetFortDetail();

        FortLevelupStartData data =
            new()
            {
                result = 1,
                build_id = build.BuildId,
                levelup_start_date = build.BuildStartDate,
                levelup_end_date = build.BuildEndDate,
                remain_time = build.BuildEndDate - build.BuildStartDate,
                fort_detail = fortDetail,
                update_data_list = updateDataList,
                entity_result = new EntityResult() // What does it do?
            };
        return this.Ok(data);
    }

    [HttpPost("move")]
    public async Task<DragaliaResult> Move(FortMoveRequest request)
    {
        DbFortBuild build = await this.fortService.Move(
            request.build_id,
            request.after_position_x,
            request.after_position_z
        );

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();
        FortBonusList bonusList = await bonusService.GetBonusList();

        FortMoveData data =
            new()
            {
                result = 1,
                build_id = build.BuildId,
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
        await this.fortService.GetFortPlantIdList(request.fort_plant_id_list);

        UpdateDataList updateDataList = this.updateDataService.GetUpdateDataList(
            this.DeviceAccountId
        );

        FortSetNewFortPlantData data = new() { result = 1, update_data_list = updateDataList };
        return this.Ok(data);
    }

    private static class StubData
    {
        public static readonly AtgenProductionRp ProductionRp = new();

        public static readonly AtgenProductionRp ProductionDf = new();

        public static readonly AtgenProductionRp ProductionSt = new() { speed = 0.03f, max = 144 };

        public const int DragonFreeGifts = 1;
    }
}
