using DragaliaAPI.Controllers;
using DragaliaAPI.Models.Generated;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Features.Album;

[Route("album")]
public class AlbumController : DragaliaControllerBase
{
    [HttpPost("index")]
    public DragaliaResult Index()
    {
        AlbumIndexResponse stubResponse =
            new()
            {
                AlbumDragonList = Enumerable.Empty<AlbumDragonData>(),
                AlbumQuestPlayRecordList = Enumerable.Empty<AtgenAlbumQuestPlayRecordList>(),
                CharaHonorList = Enumerable.Empty<AtgenCharaHonorList>(),
                AlbumPassiveUpdateResult = new(),
                UpdateDataList = new()
            };

        return this.Ok(stubResponse);
    }
}
