using Microsoft.AspNetCore.Mvc;

namespace DragaliaAPI.Photon.StateManager.Controllers;

/// <summary>
/// Ping controller for Kubernetes liveness checks.
/// </summary>
public class PingController : ControllerBase
{
    /// <summary>
    /// Return a standard HTTP response to indicate the server is alive.
    /// </summary>
    /// <returns>The response.</returns>
    [HttpGet("[controller]")]
    public IActionResult Ping() => this.Ok();
}
