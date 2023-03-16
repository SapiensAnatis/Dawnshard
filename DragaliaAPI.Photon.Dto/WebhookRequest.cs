namespace DragaliaAPI.Photon.Dto
{
    /// <summary>
    /// Request object for Photon webhooks.
    /// </summary>
    public class WebhookRequest
    {
        /// <summary>
        /// The player initiating the action that triggered this request.
        /// </summary>
        public PlayerDto Player { get; set; }

        /// <summary>
        /// The game this request relates to.
        /// </summary>
        public GameDto Game { get; set; }
    }
}
