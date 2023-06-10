using System;
using System.Collections.Generic;
using System.Text;
using DragaliaAPI.Photon.Dto.Game;

namespace DragaliaAPI.Photon.Dto.Requests
{
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
}
