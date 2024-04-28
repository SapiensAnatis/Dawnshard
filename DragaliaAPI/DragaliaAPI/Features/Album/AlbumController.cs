using DragaliaAPI.Controllers;
using DragaliaAPI.Features.Shared.Models.Generated;
using DragaliaAPI.Infrastructure.Results;
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
