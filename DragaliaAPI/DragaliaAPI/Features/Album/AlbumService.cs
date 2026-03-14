using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Album;

internal partial class AlbumService(
    IPlayerIdentityService playerIdentityService,
    ApiContext apiContext,
    ILogger<AlbumService> logger
)
{
    public async Task GrantCharaHonors(IEnumerable<Charas> charas, int questId)
    {
        IList<Charas> charasList = charas as IList<Charas> ?? charas.ToList();

        int? medalId = CharaHonorHelper.GetMedalId(questId);
        if (medalId == null)
        {
            Log.QuestDoesNotGrantMedals(logger, questId);
            return;
        }

        // Temporary adventurers cannot receive honors. Raid events and temporary events are not yet implemented, but
        // add a check anyway for future-proofing
        List<Charas> tempCharas = await apiContext
            .PlayerCharaData.AsNoTracking()
            .Where(x => charasList.Contains(x.CharaId) && x.IsTemporary)
            .Select(x => x.CharaId)
            .ToListAsyncEF();

        if (tempCharas.Count > 0)
        {
            Log.TemporaryCharactersIneligibleForMedals(logger, tempCharas);
        }

        List<DbPlayerCharaHonor> honors = charasList
            .Select(x => new DbPlayerCharaHonor()
            {
                ViewerId = playerIdentityService.ViewerId,
                CharaId = x,
                HonorId = medalId.Value,
            })
            .ExceptBy(tempCharas, x => x.CharaId)
            .ToList();

        // Honors don't go in the update data list, so we can bypass the change tracker and do a merge here
        // for better efficiency w.r.t not adding honors that already exist

        int medalsAdded = await apiContext
            .PlayerCharaHonors.Merge()
            .Using(honors)
            .OnTargetKey()
            .InsertWhenNotMatched()
            .MergeAsync();

        Log.MedalsGranted(logger, medalsAdded);
    }

    public async Task<IEnumerable<AtgenCharaHonorList>> GetCharaHonorList()
    {
        return await apiContext
            .PlayerCharaHonors.GroupBy(x => x.CharaId)
            .Select(g => new AtgenCharaHonorList()
            {
                CharaId = g.Key,
                HonorList = g.Select(x => new AtgenHonorList() { HonorId = x.HonorId }),
            })
            .ToListAsync();
    }

    private static partial class Log
    {
        [LoggerMessage(
            LogLevel.Information,
            "Some characters are ineligible for medals due to being temporary: {@Charas}"
        )]
        public static partial void TemporaryCharactersIneligibleForMedals(
            ILogger logger,
            List<Charas> charas
        );

        [LoggerMessage(LogLevel.Information, "Added {NumMedals} new medals")]
        public static partial void MedalsGranted(ILogger logger, int numMedals);

        [LoggerMessage(
            LogLevel.Information,
            "Quest clear is ineligible for medals: quest {QuestId} does not grant medals"
        )]
        public static partial void QuestDoesNotGrantMedals(ILogger logger, int questId);
    }
}
