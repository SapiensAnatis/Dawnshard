using DragaliaAPI.Infrastructure;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Album;

[Route("album")]
public class AlbumController(IAlbumService albumService) : DragaliaControllerBase
{
    [HttpPost("index")]
    public async Task<DragaliaResult> Index()
    {
        return this.Ok(new AlbumIndexResponse()
        {
            AlbumDragonList = Enumerable.Empty<AlbumDragonData>(),
            AlbumQuestPlayRecordList = Enumerable.Empty<AtgenAlbumQuestPlayRecordList>(),
            CharaHonorList = await albumService.GetCharaHonorList(),
            AlbumPassiveUpdateResult = new(),
            UpdateDataList = new(),
        });
    }
}
