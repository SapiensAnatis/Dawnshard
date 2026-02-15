using DragaliaAPI.Database;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.StorySkip;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Login.SavefileUpdate;

/// <summary>
/// Fixes some story characters having invalid properties due to being added incorrectly by story skip.
/// </summary>
public partial class V27Update(ApiContext apiContext, ILogger<V27Update> logger) : ISavefileUpdate
{
    public int SavefileVersion => 27;

    public async Task Apply()
    {
        // Story skip would add story characters who should start with Ability1Level = 0 at Ability1Level = 1, leading
        // to them being able to reach Ability1Level = 4 when unlocking a mana spiral.
        //
        // Reaching ability level 4 is impossible in general, as per master asset CharaData query:
        //  SELECT COUNT(*) FROM CharaData WHERE _Abilities14 <> 0
        // returning 0
        int abilityLevelRowsAffected = await apiContext
            .PlayerCharaData.Where(x => x.Ability1Level >= 4)
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.Ability1Level, 3));

        // Alex is a special case where she doesn't have a mana spiral, and so she's broken if she has ability level 3
        abilityLevelRowsAffected += await apiContext
            .PlayerCharaData.Where(x => x.CharaId == Charas.Alex && x.Ability1Level >= 3)
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.Ability1Level, 2));

        Log.FixedAbilityLevelCount(logger, abilityLevelRowsAffected);

        // Similarly despite adding story characters, story skip would set their shared skill to be locked.
        int sharedSkillRowsAffected = await apiContext
            .PlayerCharaData.Where(x =>
                StorySkipRewards.CharasList.Contains(x.CharaId) && !x.IsUnlockEditSkill
            )
            .ExecuteUpdateAsync(x => x.SetProperty(e => e.IsUnlockEditSkill, true));

        Log.FixedSharedSkillLockedCount(logger, sharedSkillRowsAffected);
    }

    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Information,
            "Fixed {RowsAffected} characters with broken ability 1 levels"
        )]
        public static partial void FixedAbilityLevelCount(
            ILogger<V27Update> logger,
            int rowsAffected
        );

        [LoggerMessage(
            LogLevel.Information,
            "Fixed {RowsAffected} story characters with locked shared skill"
        )]
        public static partial void FixedSharedSkillLockedCount(
            ILogger<V27Update> logger,
            int rowsAffected
        );
    }
}
