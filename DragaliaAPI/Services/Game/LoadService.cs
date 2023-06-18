using System.Diagnostics;
using AutoMapper;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.SavefileUpdate;
using DragaliaAPI.Features.Stamp;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Models.Options;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DragaliaAPI.Services.Game;

public class LoadService : ILoadService
{
    private readonly ISavefileService savefileService;
    private readonly IBonusService bonusService;
    private readonly IMapper mapper;
    private readonly ILogger<LoadService> logger;
    private readonly IOptionsMonitor<PhotonOptions> photonOptions;
    private readonly IEnumerable<ISavefileUpdate> savefileUpdates;

    public LoadService(
        ISavefileService savefileService,
        IBonusService bonusService,
        IMapper mapper,
        ILogger<LoadService> logger,
        IOptionsMonitor<PhotonOptions> photonOptions,
        IEnumerable<ISavefileUpdate> savefileUpdates
    )
    {
        this.savefileService = savefileService;
        this.bonusService = bonusService;
        this.mapper = mapper;
        this.logger = logger;
        this.photonOptions = photonOptions;
        this.savefileUpdates = savefileUpdates;
    }

    public async Task<LoadIndexData> BuildIndexData()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        DbPlayer savefile = await this.savefileService.Load().SingleAsync();

        this.logger.LogInformation("{time} ms: Load query complete", stopwatch.ElapsedMilliseconds);

        FortBonusList bonusList = await bonusService.GetBonusList();

        this.logger.LogInformation("{time} ms: Bonus list acquired", stopwatch.ElapsedMilliseconds);

        LoadIndexData data =
            new()
            {
                build_list = savefile.BuildList.Select(this.mapper.Map<BuildList>),
                user_data = this.mapper.Map<UserData>(savefile.UserData),
                chara_list = savefile.CharaList.Select(this.mapper.Map<CharaList>),
                dragon_list = savefile.DragonList.Select(this.mapper.Map<DragonList>),
                dragon_reliability_list = savefile.DragonReliabilityList.Select(
                    this.mapper.Map<DragonReliabilityList>
                ),
                ability_crest_list = savefile.AbilityCrestList.Select(
                    this.mapper.Map<AbilityCrestList>
                ),
                dragon_gift_list = savefile.DragonGiftList
                    .Where(x => x.DragonGiftId > DragonGifts.GoldenChalice)
                    .Select(this.mapper.Map<DragonGiftList>),
                talisman_list = savefile.TalismanList.Select(this.mapper.Map<TalismanList>),
                weapon_body_list = savefile.WeaponBodyList.Select(this.mapper.Map<WeaponBodyList>),
                party_list = savefile.PartyList.Select(this.mapper.Map<PartyList>),
                quest_story_list = savefile.StoryStates
                    .Where(x => x.StoryType == StoryTypes.Quest)
                    .Select(mapper.Map<QuestStoryList>),
                unit_story_list = savefile.StoryStates
                    .Where(x => x.StoryType == StoryTypes.Chara || x.StoryType == StoryTypes.Dragon)
                    .Select(mapper.Map<UnitStoryList>),
                castle_story_list = savefile.StoryStates
                    .Where(x => x.StoryType == StoryTypes.Castle)
                    .Select(mapper.Map<CastleStoryList>),
                quest_list = savefile.QuestList.Select(mapper.Map<QuestList>),
                material_list = savefile.MaterialList.Select(mapper.Map<MaterialList>),
                weapon_skin_list = savefile.WeaponSkinList.Select(mapper.Map<WeaponSkinList>),
                weapon_passive_ability_list = savefile.WeaponPassiveAbilityList.Select(
                    mapper.Map<WeaponPassiveAbilityList>
                ),
                fort_bonus_list = bonusList,
                party_power_data = new(999999),
                friend_notice = new(0, 0),
                present_notice = new(0, 0),
                guild_notice = new(0, 0, 0, 0, 0),
                //fort_plant_list = buildSummary,
                shop_notice = new ShopNotice(0),
                server_time = DateTimeOffset.UtcNow,
                stamina_multi_system_max = 99,
                stamina_multi_user_max = 12,
                quest_skip_point_system_max = 400,
                quest_skip_point_use_limit_max = 30,
                functional_maintenance_list = new List<FunctionalMaintenanceList>(),
                multi_server = new()
                {
                    host = photonOptions.CurrentValue.ServerUrl,
                    app_id = string.Empty
                },
                equip_stamp_list = savefile.EquippedStampList
                    .Select(this.mapper.Map<DbEquippedStamp, EquipStampList>)
                    .OrderBy(x => x.slot)
            };

        this.logger.LogInformation("{time} ms: Mapping complete", stopwatch.ElapsedMilliseconds);
        return data;
    }
}
