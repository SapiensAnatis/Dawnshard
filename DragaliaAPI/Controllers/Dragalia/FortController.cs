using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("fort")]
public class FortController : DragaliaControllerBase
{
    private readonly IFortRepository fortRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly IBonusService bonusService;
    private readonly IUpdateDataService updateDataService;
    private readonly IMapper mapper;

    public FortController(
        IFortRepository fortRepository,
        IUserDataRepository userDataRepository,
        IBonusService bonusService,
        IUpdateDataService updateDataService,
        IMapper mapper
    )
    {
        this.fortRepository = fortRepository;
        this.userDataRepository = userDataRepository;
        this.bonusService = bonusService;
        this.updateDataService = updateDataService;
        this.mapper = mapper;
    }

    [HttpPost("get_data")]
    public async Task<DragaliaResult> GetData(FortGetDataRequest request)
    {
        IEnumerable<BuildList> buildList = (
            await this.fortRepository.GetBuilds(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<BuildList>);

        FortBonusList bonusList = await this.bonusService.GetBonusList();

        FortDetail fortDetails;
        IQueryable<Database.Entities.DbFortDetail> query = this.fortRepository.Details;
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

        switch (request.payment_type)
        {
            //case (int)PaymentTypes.Diamantium:
            //    paymentHeld = userData.diamond_data.free_diamond + updateDataList.diamond_data.paid_diamond;
            //    updateDataList.diamond_data.paid_diamond += leftover;
            //    break;
            case (int)PaymentTypes.Wyrmite:
                paymentHeld = userData.Crystal - paymentCost;
                userData.Crystal -= paymentCost;
                break;
            default:
                throw new DragaliaException(
                    ResultCode.FortExtendCarpenterLimit,
                    $"Invalid currency used to add carpenter."
                );
        }

        paymentHeld -= paymentCost;
        if (paymentHeld < 0)
        {
            throw new DragaliaException(
                ResultCode.FortExtendCarpenterLimit,
                $"User did not have enough {request.payment_type}."
            );
        }

        // Add carpenter and write to database
        fortDetail.carpenter_num++;
        await this.fortRepository.UpdateFortCarpenterNum(this.DeviceAccountId, fortDetail.carpenter_num);

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
