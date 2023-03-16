namespace DragaliaAPI.Photon.Dto
{
    /// <summary>
    /// Response object for Photon webhooks.
    /// </summary>
    public class WebhookResponse
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WebhookResponse"/> class.
        /// </summary>
        public WebhookResponse()
        {
            this.ResultCode = 0xFF;
        }

        /// <summary>
        /// Additional data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// The message accompanying the result.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// A result code for the webhook's associated action.
        /// </summary>
        public byte ResultCode { get; set; }
    }
}
