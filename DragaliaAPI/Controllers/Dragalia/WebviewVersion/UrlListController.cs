using DragaliaAPI.Models.Responses;
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
        // Webview URLs such as localhost:5000/News are not considered acceptable by the game;
        // the webview pages will only load when the server is deployed to a dedicated domain.
        WebviewUrlListResponse response =
            new(new WebviewUrlListData(WebviewUrlListStatic.GetAllUrls(Request.Host.Value)));
        return Ok(response);
    }
}
