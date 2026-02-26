using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.Album;

public class AlbumService(ApiContext apiContext) : IAlbumService
{
    public async Task GrantCharaHonor(Charas charaId, int questId)
    {
        int medalId = CharaHonorHelper.GetMedalId(questId);
        if (medalId == 0)
            return;

        bool alreadyHeld = await apiContext.PlayerCharaHonors.AnyAsync(x =>
            x.CharaId == charaId && x.HonorId == medalId
        );

        if (!alreadyHeld)
        {
            apiContext.PlayerCharaHonors.Add(
                new DbPlayerCharaHonor { CharaId = charaId, HonorId = medalId }
            );
        }
    }

    public async Task<IEnumerable<AtgenCharaHonorList>> GetCharaHonorList()
    {
        return await apiContext.PlayerCharaHonors
            .GroupBy(x => x.CharaId)
            .Select(g => new AtgenCharaHonorList()
            {
                CharaId = g.Key,
                HonorList = g.Select(x => new AtgenHonorList() { HonorId = x.HonorId }),
            })
            .ToListAsync();
    }
}
