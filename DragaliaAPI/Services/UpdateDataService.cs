using AutoMapper;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models;
using DragaliaAPI.Models.Generated;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services;

public class UpdateDataService : IUpdateDataService
{
    private readonly ApiContext apiContext;
    private readonly IMapper mapper;

    public UpdateDataService(ApiContext apiContext, IMapper mapper)
    {
        this.apiContext = apiContext;
        this.mapper = mapper;
    }

    public UpdateDataList GetUpdateDataList(string deviceAccountId)
    {
        this.apiContext.ChangeTracker.LazyLoadingEnabled = false;

        IEnumerable<IDbHasAccountId> entities = this.apiContext.ChangeTracker
            .Entries<IDbHasAccountId>()
            .Where(
                x =>
                    (x.State is EntityState.Modified or EntityState.Added)
                    && x.Entity.DeviceAccountId == deviceAccountId
            )
            .Select(x => x.Entity);

        UpdateDataList result =
            new()
            {
                user_data = this.ConvertEntities<UserData, DbPlayerUserData>(entities)?.Single(),
                chara_list = this.ConvertEntities<CharaList, DbPlayerCharaData>(entities),
                dragon_list = this.ConvertEntities<DragonList, DbPlayerDragonData>(entities),
                dragon_reliability_list = this.ConvertEntities<
                    DragonReliabilityList,
                    DbPlayerDragonReliability
                >(entities),
                weapon_body_list = this.ConvertEntities<WeaponBodyList, DbWeaponBody>(entities),
                ability_crest_list = this.ConvertEntities<AbilityCrestList, DbAbilityCrest>(
                    entities
                ),
                party_list = this.ConvertEntities<PartyList, DbParty>(entities),
                quest_story_list = this.ConvertEntities<QuestStoryList, DbPlayerStoryState>(
                    entities
                ),
                material_list = this.ConvertEntities<MaterialList, DbPlayerMaterial>(entities),
                quest_list = this.ConvertEntities<QuestList, DbQuest>(entities)
            };

        this.apiContext.ChangeTracker.LazyLoadingEnabled = true;

        return result;
    }

    private IEnumerable<TNetwork>? ConvertEntities<TNetwork, TDatabase>(
        IEnumerable<IDbHasAccountId> baseEntries
    ) where TDatabase : IDbHasAccountId
    {
        IEnumerable<TDatabase> typedEntries = baseEntries.OfType<TDatabase>();

        return typedEntries.Any()
            ? typedEntries.Select(x => this.mapper.Map<TNetwork>(x)).ToList()
            : null;
    }
}
