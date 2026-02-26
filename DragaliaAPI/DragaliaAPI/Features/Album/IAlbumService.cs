using DragaliaAPI.Models.Generated;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Features.Album;

public interface IAlbumService
{
    Task GrantCharaHonor(Charas charaId, int questId);

    Task<IEnumerable<AtgenCharaHonorList>> GetCharaHonorList();
}
