using AutoMapper;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("fort")]
public class FortController : DragaliaControllerBase
{
    private readonly IFortRepository fortRepository;
    private readonly IBonusService bonusService;
    private readonly IMapper mapper;

    public FortController(
        IFortRepository fortRepository,
        IBonusService bonusService,
        IMapper mapper
    )
    {
        this.fortRepository = fortRepository;
        this.bonusService = bonusService;
        this.mapper = mapper;
    }

    [HttpPost("get_data")]
    public async Task<DragaliaResult> GetData(FortGetDataRequest request)
    {
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
                fort_detail = StubData.FortDetail,
                current_server_time = DateTime.UtcNow
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
