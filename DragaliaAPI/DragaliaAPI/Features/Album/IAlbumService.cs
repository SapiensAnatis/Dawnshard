using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Album;

internal interface IAlbumService
{
    Task GrantCharaHonors(IEnumerable<Charas> charas, int questId);

    Task<IEnumerable<AtgenCharaHonorList>> GetCharaHonorList();
}
