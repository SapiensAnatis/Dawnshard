using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.Photon.Shared.Requests;

/// <summary>
/// Base request object for Photon webhooks.
/// </summary>
public abstract class WebhookRequest
{
    /// <summary>
    /// The player sending the request.
    /// </summary>
    public Player Player { get; set; }
}
