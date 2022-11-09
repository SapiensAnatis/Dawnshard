using DragaliaAPI.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("webview_version")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class UrlListController : ControllerBase
{
    [HttpPost]
    [Route("url_list")]
    public DragaliaResult UrlList()
    {
        // Webview URLs such as localhost:5000/News are not considered acceptable by the game;
        // the webview pages will only load when the server is deployed to a dedicated domain.
        WebviewUrlListResponse response =
            new(new WebviewUrlListData(WebviewUrlListStatic.GetAllUrls(this.Request.Host.Value)));
        return Ok(response);
    }
}
