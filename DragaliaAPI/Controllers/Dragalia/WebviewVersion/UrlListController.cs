using DragaliaAPI.Models.Dragalia.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia.WebviewVersion;
[Route("webview_version/url_list")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class UrlListController : ControllerBase
{
    [HttpPost]
    public DragaliaResult Post()
    {
        return Ok(new UrlListResponse());
    } 
}
