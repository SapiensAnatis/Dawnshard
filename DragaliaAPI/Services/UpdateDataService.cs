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
        IEnumerable<IDbHasAccountId> updatedEntities = this.apiContext.ChangeTracker
            .Entries<IDbHasAccountId>()
            .Where(
                x =>
                    (x.State is EntityState.Modified or EntityState.Added)
                    && x.Entity.DeviceAccountId == deviceAccountId
            )
            .Select(x => x.Entity);

        return new()
        {
            chara_list = this.ConvertEntities<CharaList, DbPlayerCharaData>(updatedEntities),
            dragon_list = this.ConvertEntities<DragonList, DbPlayerDragonData>(updatedEntities),
            dragon_reliability_list = this.ConvertEntities<
                DragonReliabilityList,
                DbPlayerDragonReliability
            >(updatedEntities),
            user_data = this.ConvertEntities<UserData, DbPlayerUserData>(updatedEntities)
                ?.SingleOrDefault(),
            party_list = this.ConvertEntities<PartyList, DbParty>(updatedEntities),
            quest_story_list = this.ConvertEntities<QuestStoryList, DbPlayerStoryState>(
                updatedEntities
            ),
            material_list = this.ConvertEntities<MaterialList, DbPlayerMaterial>(updatedEntities),
        };
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
