using AutoMapper;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("fort")]
public class FortController : DragaliaControllerBase
{
    private readonly IFortRepository fortRepository;
    private readonly IMapper mapper;

    public FortController(IFortRepository fortRepository, IMapper mapper)
    {
        this.fortRepository = fortRepository;
        this.mapper = mapper;
    }

    [HttpPost("get_data")]
    public async Task<DragaliaResult> GetData(FortGetDataRequest request)
    {
        IEnumerable<BuildList> buildList = (
            await this.fortRepository.GetBuilds(this.DeviceAccountId).ToListAsync()
        ).Select(mapper.Map<BuildList>);

        FortGetDataData data =
            new()
            {
                build_list = buildList,
                fort_bonus_list = StubData.EmptyBonusList,
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
        private static readonly IEnumerable<AtgenParamBonus> WeaponBonus = Enumerable
            .Range(1, 9)
            .Select(
                x =>
                    new AtgenParamBonus()
                    {
                        weapon_type = x,
                        hp = 200,
                        attack = 200
                    }
            );

        private static readonly IEnumerable<AtgenElementBonus> EmptyElementBonus = Enumerable
            .Range(1, 5)
            .Select(
                x =>
                    new AtgenElementBonus()
                    {
                        elemental_type = x,
                        hp = 200,
                        attack = 200
                    }
            )
            .Append(
                new AtgenElementBonus()
                {
                    elemental_type = 99,
                    hp = 20,
                    attack = 20
                }
            );

        private static readonly IEnumerable<AtgenDragonBonus> EmptyDragonBonus = Enumerable
            .Range(1, 5)
            .Select(
                x =>
                    new AtgenDragonBonus()
                    {
                        elemental_type = x,
                        dragon_bonus = 200,
                        hp = 200,
                        attack = 200
                    }
            )
            .Append(
                new AtgenDragonBonus()
                {
                    elemental_type = 99,
                    hp = 200,
                    attack = 200
                }
            );

        public static readonly FortBonusList EmptyBonusList =
            new()
            {
                param_bonus = WeaponBonus,
                param_bonus_by_weapon = WeaponBonus,
                element_bonus = EmptyElementBonus,
                chara_bonus_by_album = EmptyElementBonus,
                all_bonus = new() { hp = 200, attack = 200 },
                dragon_bonus = EmptyDragonBonus,
                dragon_bonus_by_album = EmptyElementBonus,
                dragon_time_bonus = new() { dragon_time_bonus = 20 }
            };

        public static FortDetail FortDetail =
            new()
            {
                carpenter_num = 5,
                max_carpenter_count = 5,
                working_carpenter_num = 0
            };

        public static AtgenProductionRp ProductionRp = new();

        public static AtgenProductionRp ProductionDf = new();

        public static AtgenProductionRp ProductionSt = new() { speed = 0.03f, max = 144 };

        public const int DragonFreeGifts = 1;
    }
}
