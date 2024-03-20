namespace DragaliaAPI.Photon.Shared.Requests;

/// <summary>
/// A request to modify a game -- either join, leave, or close.
/// </summary>
public class GameModifyRequest : WebhookRequest
{
    /// <summary>
    /// The name of the game that is to be modified.
    /// </summary>
    public string GameName { get; set; } = string.Empty;
}
