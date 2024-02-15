using DragaliaAPI.Controllers;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Fort;

[Route("fort")]
public class FortController : DragaliaControllerBase
{
    private readonly IFortService fortService;
    private readonly IBonusService bonusService;
    private readonly IUpdateDataService updateDataService;
    private readonly IRewardService rewardService;
    private readonly IDragonService dragonService;

    public FortController(
        IFortService fortService,
        IBonusService bonusService,
        IUpdateDataService updateDataService,
        IRewardService rewardService,
        IDragonService dragonService
    )
    {
        this.fortService = fortService;
        this.bonusService = bonusService;
        this.updateDataService = updateDataService;
        this.rewardService = rewardService;
        this.dragonService = dragonService;
    }

    [HttpPost("get_data")]
    public async Task<DragaliaResult> GetData()
    {
        FortDetail fortDetail = await fortService.GetFortDetail();
        IEnumerable<BuildList> buildList = await fortService.GetBuildList();

        FortBonusList bonusList = await bonusService.GetBonusList();

        int freeGiftCount = await this.dragonService.GetFreeGiftCount();

        FortGetDataData data =
            new()
            {
                build_list = buildList,
                fort_bonus_list = bonusList,
                dragon_contact_free_gift_count = freeGiftCount,
                production_rp = await this.fortService.GetRupieProduction(),
                production_st = await this.fortService.GetStaminaProduction(),
                production_df = await this.fortService.GetDragonfruitProduction(),
                fort_detail = fortDetail,
                current_server_time = DateTimeOffset.UtcNow
            };

        await updateDataService.SaveChangesAsync();

        return Ok(data);
    }

    [HttpPost("add_carpenter")]
    public async Task<DragaliaResult> AddCarpenter(FortAddCarpenterRequest request)
    {
        await fortService.AddCarpenter(request.payment_type);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        FortAddCarpenterData data =
            new()
            {
                result = 1,
                fort_detail = await fortService.GetFortDetail(),
                update_data_list = updateDataList
            };
        return Ok(data);
    }

    [HttpPost("build_at_once")]
    public async Task<DragaliaResult> BuildAtOnce(FortBuildAtOnceRequest request)
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await fortService.BuildAtOnce(request.payment_type, request.build_id);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortBuildAtOnceData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                fort_bonus_list = bonusList,
                production_rp = await this.fortService.GetRupieProduction(),
                production_st = await this.fortService.GetStaminaProduction(),
                production_df = await this.fortService.GetDragonfruitProduction(),
                fort_detail = fortDetail,
                update_data_list = updateDataList,
            };
        return Ok(data);
    }

    [HttpPost("build_cancel")]
    public async Task<DragaliaResult> BuildCancel(FortBuildCancelRequest request)
    {
        DbFortBuild cancelledBuild = await fortService.CancelBuild(request.build_id);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortBuildCancelData data =
            new()
            {
                result = 1,
                build_id = cancelledBuild.BuildId,
                fort_detail = fortDetail,
                update_data_list = updateDataList
            };
        return Ok(data);
    }

    [HttpPost("build_end")]
    public async Task<DragaliaResult> BuildEnd(FortBuildEndRequest request)
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await fortService.EndBuild(request.build_id);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortBuildEndData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                fort_bonus_list = bonusList,
                production_rp = await this.fortService.GetRupieProduction(),
                production_st = await this.fortService.GetStaminaProduction(),
                production_df = await this.fortService.GetDragonfruitProduction(),
                fort_detail = fortDetail,
                update_data_list = updateDataList,
            };
        return Ok(data);
    }

    [HttpPost("build_start")]
    public async Task<DragaliaResult> BuildStart(FortBuildStartRequest request)
    {
        DbFortBuild build = await fortService.BuildStart(
            request.fort_plant_id,
            request.position_x,
            request.position_z
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await fortService.GetFortDetail();

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
        return Ok(data);
    }

    [HttpPost("levelup_at_once")]
    public async Task<DragaliaResult> LevelupAtOnce(FortLevelupAtOnceRequest request)
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await fortService.LevelupAtOnce(request.payment_type, request.build_id);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        (int HalidomLevel, int SmithyLevel) levels = await this.fortService.GetCoreLevels();
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortLevelupAtOnceData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                current_fort_level = levels.HalidomLevel,
                current_fort_craft_level = levels.SmithyLevel,
                fort_bonus_list = bonusList,
                production_rp = await this.fortService.GetRupieProduction(),
                production_st = await this.fortService.GetStaminaProduction(),
                production_df = await this.fortService.GetDragonfruitProduction(),
                fort_detail = fortDetail,
                update_data_list = updateDataList,
            };
        return Ok(data);
    }

    [HttpPost("levelup_cancel")]
    public async Task<DragaliaResult> LevelupCancel(FortLevelupCancelRequest request)
    {
        DbFortBuild cancelledBuild = await fortService.CancelLevelup(request.build_id);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortLevelupCancelData data =
            new()
            {
                result = 1,
                build_id = cancelledBuild.BuildId,
                fort_detail = fortDetail,
                update_data_list = updateDataList
            };
        return Ok(data);
    }

    [HttpPost("levelup_end")]
    public async Task<DragaliaResult> LevelupEnd(FortLevelupEndRequest request)
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await fortService.EndLevelup(request.build_id);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();

        (int HalidomLevel, int SmithyLevel) levels = await this.fortService.GetCoreLevels();
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortLevelupEndData data =
            new()
            {
                result = 1,
                build_id = request.build_id,
                current_fort_level = levels.HalidomLevel,
                current_fort_craft_level = levels.SmithyLevel,
                fort_bonus_list = bonusList,
                production_rp = await this.fortService.GetRupieProduction(),
                production_st = await this.fortService.GetStaminaProduction(),
                production_df = await this.fortService.GetDragonfruitProduction(),
                fort_detail = fortDetail,
                update_data_list = updateDataList,
            };
        return Ok(data);
    }

    [HttpPost("levelup_start")]
    public async Task<DragaliaResult> LevelupStart(FortLevelupStartRequest request)
    {
        DbFortBuild build = await fortService.LevelupStart(request.build_id);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        FortDetail fortDetail = await fortService.GetFortDetail();

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
                entity_result = this.rewardService.GetEntityResult()
            };
        return Ok(data);
    }

    [HttpPost("move")]
    public async Task<DragaliaResult> Move(FortMoveRequest request)
    {
        DbFortBuild build = await fortService.Move(
            request.build_id,
            request.after_position_x,
            request.after_position_z
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync();
        FortBonusList bonusList = await bonusService.GetBonusList();

        FortMoveData data =
            new()
            {
                result = 1,
                build_id = build.BuildId,
                fort_bonus_list = bonusList,
                production_rp = await this.fortService.GetRupieProduction(),
                production_st = await this.fortService.GetStaminaProduction(),
                production_df = await this.fortService.GetDragonfruitProduction(),
                update_data_list = updateDataList
            };
        return Ok(data);
    }

    // What exactly does it do

    // Unsure about this, but this looks like it might do the correct thing
    [HttpPost("set_new_fort_plant")]
    public async Task<DragaliaResult> SetNewFortPlant(FortSetNewFortPlantRequest request)
    {
        FortSetNewFortPlantData resp = new();

        await fortService.ClearPlantNewStatuses(request.fort_plant_id_list);

        resp.result = 1;
        resp.update_data_list = await updateDataService.SaveChangesAsync();

        return Ok(resp);
    }

    [HttpPost("get_multi_income")]
    public async Task<DragaliaResult> GetMultiIncome(FortGetMultiIncomeRequest request)
    {
        FortGetMultiIncomeData resp = await this.fortService.CollectIncome(request.build_id_list);

        resp.update_data_list = await this.updateDataService.SaveChangesAsync();
        resp.entity_result = this.rewardService.GetEntityResult();

        return Ok(resp);
    }
}
