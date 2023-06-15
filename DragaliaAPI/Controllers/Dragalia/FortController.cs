using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("fort")]
public class FortController : DragaliaControllerBase
{
    private readonly IFortService fortService;
    private readonly IBonusService bonusService;
    private readonly IUpdateDataService updateDataService;

    public FortController(
        IFortService fortService,
        IBonusService bonusService,
        IUpdateDataService updateDataService
    )
    {
        this.fortService = fortService;
        this.bonusService = bonusService;
        this.updateDataService = updateDataService;
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

        await this.updateDataService.SaveChangesAsync();

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

        BuildList halidom =
            builds.FirstOrDefault(x => x.plant_id == FortPlants.TheHalidom)
            ?? throw new InvalidOperationException("Missing Halidom building!");

        BuildList? smithy = builds.FirstOrDefault(x => x.plant_id == FortPlants.Smithy);

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
                current_fort_craft_level = smithy?.level ?? 0,
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

        UpdateDataList updateDataList = await this.updateDataService.SaveChangesAsync();

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
