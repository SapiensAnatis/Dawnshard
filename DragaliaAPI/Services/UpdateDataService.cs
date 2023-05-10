using System.Collections;
using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Extensions;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DragaliaAPI.Services;

public class UpdateDataService : IUpdateDataService
{
    private readonly ApiContext apiContext;
    private readonly IMapper mapper;
    private readonly IPlayerDetailsService playerDetailsService;

    public UpdateDataService(
        ApiContext apiContext,
        IMapper mapper,
        IPlayerDetailsService playerDetailsService
    )
    {
        this.apiContext = apiContext;
        this.mapper = mapper;
        this.playerDetailsService = playerDetailsService;
    }

    [Obsolete(
        "Prefer UpdateDataService.SaveChangesAsync instead due to key id bugs with this method"
    )]
    public UpdateDataList GetUpdateDataList(string deviceAccountId)
    {
        this.apiContext.ChangeTracker.LazyLoadingEnabled = false;

        List<IDbHasAccountId> entities = this.apiContext.ChangeTracker
            .Entries<IDbHasAccountId>()
            .Where(
                x =>
                    (x.State is EntityState.Modified or EntityState.Added)
                    && x.Entity.DeviceAccountId == deviceAccountId
            )
            .Select(x => x.Entity)
            .ToList();

        return this.MapUpdateDataList(entities);
    }

    public async Task<UpdateDataList> SaveChangesAsync()
    {
        List<IDbHasAccountId> entities = this.apiContext.ChangeTracker
            .Entries<IDbHasAccountId>()
            .Where(
                x =>
                    (x.State is EntityState.Modified or EntityState.Added)
                    && x.Entity.DeviceAccountId == this.playerDetailsService.AccountId
            )
            .Select(x => x.Entity)
            .ToList();

        await this.apiContext.SaveChangesAsync();

        return this.MapUpdateDataList(entities);
    }

    private UpdateDataList MapUpdateDataList(List<IDbHasAccountId> entities) =>
        new()
        {
            user_data = this.ConvertEntities<UserData, DbPlayerUserData>(entities)?.Single(), // Can't use SingleOrDefault if the list itself is null
            chara_list = this.ConvertEntities<CharaList, DbPlayerCharaData>(entities),
            dragon_list = this.ConvertEntities<DragonList, DbPlayerDragonData>(entities),
            dragon_reliability_list = this.ConvertEntities<
                DragonReliabilityList,
                DbPlayerDragonReliability
            >(entities),
            weapon_body_list = this.ConvertEntities<WeaponBodyList, DbWeaponBody>(entities),
            weapon_skin_list = this.ConvertEntities<WeaponSkinList, DbWeaponSkin>(entities),
            ability_crest_list = this.ConvertEntities<AbilityCrestList, DbAbilityCrest>(entities),
            party_list = this.ConvertEntities<PartyList, DbParty>(entities),
            quest_story_list = this.ConvertEntities<QuestStoryList, DbPlayerStoryState>(
                entities,
                x => x.StoryType == StoryTypes.Quest
            ),
            unit_story_list = this.ConvertEntities<UnitStoryList, DbPlayerStoryState>(
                entities,
                x => x.StoryType == StoryTypes.Chara || x.StoryType == StoryTypes.Dragon
            ),
            castle_story_list = this.ConvertEntities<CastleStoryList, DbPlayerStoryState>(
                entities,
                x => x.StoryType == StoryTypes.Castle
            ),
            material_list = this.ConvertEntities<MaterialList, DbPlayerMaterial>(entities),
            dragon_gift_list = this.ConvertEntities<DragonGiftList, DbPlayerDragonGift>(
                entities,
                x => x.DragonGiftId > DragonGifts.GoldenChalice
            ),
            quest_list = this.ConvertEntities<QuestList, DbQuest>(entities),
            build_list = this.ConvertEntities<BuildList, DbFortBuild>(entities),
            weapon_passive_ability_list = this.ConvertEntities<
                WeaponPassiveAbilityList,
                DbWeaponPassiveAbility
            >(entities)
        };

    private List<TNetwork>? ConvertEntities<TNetwork, TDatabase>(
        IEnumerable<IDbHasAccountId> baseEntries,
        Func<TDatabase, bool>? filterPredicate = null
    )
        where TDatabase : IDbHasAccountId
    {
        IEnumerable<TDatabase> typedEntries = baseEntries.OfType<TDatabase>();

        if (filterPredicate is not null)
        {
            typedEntries = typedEntries.Where(filterPredicate);
        }

        return typedEntries.Any()
            ? typedEntries.Select(x => this.mapper.Map<TNetwork>(x)).ToList()
            : null;
    }

    public class UnmappedUpdateDataList
    {
        public DbPlayerUserData? UserData { get; set; }

        public IEnumerable<DbPlayerCharaData>? CharaList { get; set; }

        public IEnumerable<DbPlayerDragonData>? DragonList { get; set; }

        public IEnumerable<DbPlayerDragonReliability>? DragonReliabilityList { get; set; }

        public IEnumerable<DbWeaponBody>? WeaponBodyList { get; set; }

        public IEnumerable<DbAbilityCrest>? AbilityCrestList { get; set; }

        public IEnumerable<DbParty>? PartyList { get; set; }

        public IEnumerable<DbPlayerStoryState>? QuestStoryList { get; set; }

        public IEnumerable<DbPlayerStoryState>? UnitStoryList { get; set; }

        public IEnumerable<DbPlayerStoryState>? CastleStoryList { get; set; }

        public IEnumerable<DbPlayerMaterial>? MaterialList { get; set; }

        public IEnumerable<DbPlayerDragonGift>? DragonGiftList { get; set; }

        public IEnumerable<DbQuest>? QuestList { get; set; }

        public IEnumerable<DbFortBuild>? BuildList { get; set; }

        public IEnumerable<DbWeaponPassiveAbility>? WeaponPassiveAbilityList { get; set; }

        public IEnumerable<DbWeaponSkin>? WeaponSkinList { get; set; }
    }
}
