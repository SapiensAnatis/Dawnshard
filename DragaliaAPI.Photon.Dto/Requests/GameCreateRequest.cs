using System;
using System.Collections.Generic;
using System.Text;

namespace DragaliaAPI.Photon.Dto.Requests
{
    /// <summary>
    /// Request sent by Photon to store a new game in Redis.
    /// </summary>
    public class GameCreateRequest : WebhookRequest
    {
        /// <summary>
        /// The game information.
        /// </summary>
        public GameBase Game { get; set; }
    }
}
