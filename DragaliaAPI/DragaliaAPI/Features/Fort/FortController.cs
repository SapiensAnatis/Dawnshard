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
    public async Task<DragaliaResult> GetData(CancellationToken cancellationToken)
    {
        FortDetail fortDetail = await fortService.GetFortDetail();
        IEnumerable<BuildList> buildList = await fortService.GetBuildList();

        FortBonusList bonusList = await bonusService.GetBonusList();

        int freeGiftCount = await this.dragonService.GetFreeGiftCount();

        FortGetDataResponse data =
            new()
            {
                BuildList = buildList,
                FortBonusList = bonusList,
                DragonContactFreeGiftCount = freeGiftCount,
                ProductionRp = await this.fortService.GetRupieProduction(),
                ProductionSt = await this.fortService.GetStaminaProduction(),
                ProductionDf = await this.fortService.GetDragonfruitProduction(),
                FortDetail = fortDetail,
                CurrentServerTime = DateTimeOffset.UtcNow
            };

        await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(data);
    }

    [HttpPost("add_carpenter")]
    public async Task<DragaliaResult> AddCarpenter(
        FortAddCarpenterRequest request,
        CancellationToken cancellationToken
    )
    {
        await fortService.AddCarpenter(request.PaymentType);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        FortAddCarpenterResponse data =
            new()
            {
                Result = 1,
                FortDetail = await fortService.GetFortDetail(),
                UpdateDataList = updateDataList
            };
        return Ok(data);
    }

    [HttpPost("build_at_once")]
    public async Task<DragaliaResult> BuildAtOnce(
        FortBuildAtOnceRequest request,
        CancellationToken cancellationToken
    )
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await fortService.BuildAtOnce(request.PaymentType, request.BuildId);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortBuildAtOnceResponse data =
            new()
            {
                Result = 1,
                BuildId = request.BuildId,
                FortBonusList = bonusList,
                ProductionRp = await this.fortService.GetRupieProduction(),
                ProductionSt = await this.fortService.GetStaminaProduction(),
                ProductionDf = await this.fortService.GetDragonfruitProduction(),
                FortDetail = fortDetail,
                UpdateDataList = updateDataList,
            };
        return Ok(data);
    }

    [HttpPost("build_cancel")]
    public async Task<DragaliaResult> BuildCancel(
        FortBuildCancelRequest request,
        CancellationToken cancellationToken
    )
    {
        DbFortBuild cancelledBuild = await fortService.CancelBuild(request.BuildId);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortBuildCancelResponse data =
            new()
            {
                Result = 1,
                BuildId = cancelledBuild.BuildId,
                FortDetail = fortDetail,
                UpdateDataList = updateDataList
            };
        return Ok(data);
    }

    [HttpPost("build_end")]
    public async Task<DragaliaResult> BuildEnd(
        FortBuildEndRequest request,
        CancellationToken cancellationToken
    )
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await fortService.EndBuild(request.BuildId);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortBuildEndResponse data =
            new()
            {
                Result = 1,
                BuildId = request.BuildId,
                FortBonusList = bonusList,
                ProductionRp = await this.fortService.GetRupieProduction(),
                ProductionSt = await this.fortService.GetStaminaProduction(),
                ProductionDf = await this.fortService.GetDragonfruitProduction(),
                FortDetail = fortDetail,
                UpdateDataList = updateDataList,
            };
        return Ok(data);
    }

    [HttpPost("build_start")]
    public async Task<DragaliaResult> BuildStart(
        FortBuildStartRequest request,
        CancellationToken cancellationToken
    )
    {
        DbFortBuild build = await fortService.BuildStart(
            request.FortPlantId,
            request.PositionX,
            request.PositionZ
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortBuildStartResponse data =
            new()
            {
                Result = 1,
                BuildId = build.BuildId,
                BuildStartDate = build.BuildStartDate,
                BuildEndDate = build.BuildEndDate,
                RemainTime = build.RemainTime,
                FortDetail = fortDetail,
                UpdateDataList = updateDataList,
                EntityResult = new EntityResult() // What does it do?
            };
        return Ok(data);
    }

    [HttpPost("levelup_at_once")]
    public async Task<DragaliaResult> LevelupAtOnce(
        FortLevelupAtOnceRequest request,
        CancellationToken cancellationToken
    )
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await fortService.LevelupAtOnce(request.PaymentType, request.BuildId);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        (int HalidomLevel, int SmithyLevel) levels = await this.fortService.GetCoreLevels();
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortLevelupAtOnceResponse data =
            new()
            {
                Result = 1,
                BuildId = request.BuildId,
                CurrentFortLevel = levels.HalidomLevel,
                CurrentFortCraftLevel = levels.SmithyLevel,
                FortBonusList = bonusList,
                ProductionRp = await this.fortService.GetRupieProduction(),
                ProductionSt = await this.fortService.GetStaminaProduction(),
                ProductionDf = await this.fortService.GetDragonfruitProduction(),
                FortDetail = fortDetail,
                UpdateDataList = updateDataList,
            };
        return Ok(data);
    }

    [HttpPost("levelup_cancel")]
    public async Task<DragaliaResult> LevelupCancel(
        FortLevelupCancelRequest request,
        CancellationToken cancellationToken
    )
    {
        DbFortBuild cancelledBuild = await fortService.CancelLevelup(request.BuildId);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortLevelupCancelResponse data =
            new()
            {
                Result = 1,
                BuildId = cancelledBuild.BuildId,
                FortDetail = fortDetail,
                UpdateDataList = updateDataList
            };
        return Ok(data);
    }

    [HttpPost("levelup_end")]
    public async Task<DragaliaResult> LevelupEnd(
        FortLevelupEndRequest request,
        CancellationToken cancellationToken
    )
    {
        FortBonusList bonusList = await bonusService.GetBonusList();

        await fortService.EndLevelup(request.BuildId);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        (int HalidomLevel, int SmithyLevel) levels = await this.fortService.GetCoreLevels();
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortLevelupEndResponse data =
            new()
            {
                Result = 1,
                BuildId = request.BuildId,
                CurrentFortLevel = levels.HalidomLevel,
                CurrentFortCraftLevel = levels.SmithyLevel,
                FortBonusList = bonusList,
                ProductionRp = await this.fortService.GetRupieProduction(),
                ProductionSt = await this.fortService.GetStaminaProduction(),
                ProductionDf = await this.fortService.GetDragonfruitProduction(),
                FortDetail = fortDetail,
                UpdateDataList = updateDataList,
            };
        return Ok(data);
    }

    [HttpPost("levelup_start")]
    public async Task<DragaliaResult> LevelupStart(
        FortLevelupStartRequest request,
        CancellationToken cancellationToken
    )
    {
        DbFortBuild build = await fortService.LevelupStart(request.BuildId);

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        FortDetail fortDetail = await fortService.GetFortDetail();

        FortLevelupStartResponse data =
            new()
            {
                Result = 1,
                BuildId = build.BuildId,
                LevelupStartDate = build.BuildStartDate,
                LevelupEndDate = build.BuildEndDate,
                RemainTime = build.BuildEndDate - build.BuildStartDate,
                FortDetail = fortDetail,
                UpdateDataList = updateDataList,
                EntityResult = this.rewardService.GetEntityResult()
            };
        return Ok(data);
    }

    [HttpPost("move")]
    public async Task<DragaliaResult> Move(
        FortMoveRequest request,
        CancellationToken cancellationToken
    )
    {
        DbFortBuild build = await fortService.Move(
            request.BuildId,
            request.AfterPositionX,
            request.AfterPositionZ
        );

        UpdateDataList updateDataList = await updateDataService.SaveChangesAsync(cancellationToken);
        FortBonusList bonusList = await bonusService.GetBonusList();

        FortMoveResponse data =
            new()
            {
                Result = 1,
                BuildId = build.BuildId,
                FortBonusList = bonusList,
                ProductionRp = await this.fortService.GetRupieProduction(),
                ProductionSt = await this.fortService.GetStaminaProduction(),
                ProductionDf = await this.fortService.GetDragonfruitProduction(),
                UpdateDataList = updateDataList
            };
        return Ok(data);
    }

    // What exactly does it do

    // Unsure about this, but this looks like it might do the correct thing
    [HttpPost("set_new_fort_plant")]
    public async Task<DragaliaResult> SetNewFortPlant(
        FortSetNewFortPlantRequest request,
        CancellationToken cancellationToken
    )
    {
        FortSetNewFortPlantResponse resp = new();

        await fortService.ClearPlantNewStatuses(request.FortPlantIdList);

        resp.Result = 1;
        resp.UpdateDataList = await updateDataService.SaveChangesAsync(cancellationToken);

        return Ok(resp);
    }

    [HttpPost("get_multi_income")]
    public async Task<DragaliaResult> GetMultiIncome(
        FortGetMultiIncomeRequest request,
        CancellationToken cancellationToken
    )
    {
        FortGetMultiIncomeResponse resp = await this.fortService.CollectIncome(request.BuildIdList);

        resp.UpdateDataList = await this.updateDataService.SaveChangesAsync(cancellationToken);

        resp.EntityResult = this.rewardService.GetEntityResult();

        return Ok(resp);
    }
}
