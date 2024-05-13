using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models.Story;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Reward.Handlers;

public partial class CharaHandler(
    ApiContext apiContext,
    IPlayerIdentityService playerIdentityService,
    ILogger<CharaHandler> logger
) : IRewardHandler, IBatchRewardHandler
{
    public IReadOnlyList<EntityTypes> SupportedTypes { get; } = [EntityTypes.Chara];

    public async Task<GrantReturn> Grant(Entity entity)
    {
        Charas chara = (Charas)entity.Id;
        if (!Enum.IsDefined(chara))
        {
            Log.InvalidCharacterId(logger, entity, chara);
            return GrantReturn.FailError();
        }

        if (
            apiContext.PlayerCharaData.Local.Any(x => x.CharaId == chara)
            || await apiContext.PlayerCharaData.AnyAsync(x => x.CharaId == chara)
        )
        {
            return GrantReturn.Discarded();
        }

        apiContext.PlayerCharaData.Add(
            new DbPlayerCharaData(playerIdentityService.ViewerId, chara)
        );

        if (
            MasterAsset.CharaStories.TryGetValue((int)chara, out StoryData? storyData)
            && !await apiContext.PlayerStoryState.AnyAsync(x =>
                x.StoryType == StoryTypes.Chara && x.StoryId == storyData.StoryIds[0]
            )
        )
        {
            apiContext.PlayerStoryState.Add(
                new DbPlayerStoryState()
                {
                    ViewerId = playerIdentityService.ViewerId,
                    StoryType = StoryTypes.Chara,
                    StoryId = storyData.StoryIds[0]
                }
            );
        }

        return GrantReturn.Added();
    }

    public async Task<IDictionary<TKey, GrantReturn>> GrantRange<TKey>(
        IDictionary<TKey, Entity> entities
    )
        where TKey : struct
    {
        List<Charas> charasToAdd = entities.Select(x => (Charas)x.Value.Id).ToList();
        IEnumerable<int> firstStoryIds = GetFirstStoryIds(charasToAdd).ToList();

        Dictionary<TKey, GrantReturn> result = [];

        HashSet<Charas> ownedCharacters = await apiContext
            .PlayerCharaData.Where(x => charasToAdd.Contains(x.CharaId))
            .Select(x => x.CharaId)
            .ToHashSetAsync();

        HashSet<int> ownedStoryIds = await apiContext
            .PlayerStoryState.Where(x =>
                x.StoryType == StoryTypes.Chara && firstStoryIds.Contains(x.StoryId)
            )
            .Select(x => x.StoryId)
            .ToHashSetAsync();

        foreach ((TKey key, Entity entity) in entities)
        {
            Charas chara = (Charas)entity.Id;
            if (!Enum.IsDefined(chara))
            {
                Log.InvalidCharacterId(logger, entity, chara);
                result.Add(key, GrantReturn.FailError());
                continue;
            }

            if (
                apiContext.PlayerCharaData.Local.Any(x => x.CharaId == chara)
                || ownedCharacters.Contains(chara)
            )
            {
                result.Add(key, GrantReturn.Discarded());
                continue;
            }

            apiContext.PlayerCharaData.Add(
                new DbPlayerCharaData(playerIdentityService.ViewerId, chara)
            );
            result.Add(key, GrantReturn.Added());
            ownedCharacters.Add(chara);

            if (
                MasterAsset.CharaStories.TryGetValue((int)chara, out StoryData? storyData)
                && !ownedStoryIds.Contains(storyData.StoryIds[0])
            )
            {
                apiContext.PlayerStoryState.Add(
                    new DbPlayerStoryState()
                    {
                        ViewerId = playerIdentityService.ViewerId,
                        StoryType = StoryTypes.Chara,
                        StoryId = storyData.StoryIds[0]
                    }
                );
            }
        }

        return result;

        static IEnumerable<int> GetFirstStoryIds(IEnumerable<Charas> characters)
        {
            foreach (Charas c in characters)
            {
                if (!MasterAsset.CharaStories.TryGetValue((int)c, out StoryData? storyData))
                {
                    continue;
                }

                yield return storyData.StoryIds[0];
            }
        }
    }

    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Error,
            "Entity {Entity} is not a valid character entity: {CharaId} is not a character ID"
        )]
        public static partial void InvalidCharacterId(
            ILogger logger,
            Entity entity,
            Charas charaId
        );
    }
}
