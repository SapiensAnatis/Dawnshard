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
        AlbumIndexData stubResponse =
            new()
            {
                album_dragon_list = Enumerable.Empty<AlbumDragonData>(),
                album_quest_play_record_list = Enumerable.Empty<AtgenAlbumQuestPlayRecordList>(),
                chara_honor_list = Enumerable.Empty<AtgenCharaHonorList>(),
                album_passive_update_result = new(),
                update_data_list = new()
            };

        return this.Ok(stubResponse);
    }
}
