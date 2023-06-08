using System;
using System.Text;
using DragaliaAPI.Photon.Dto.Game;

namespace DragaliaAPI.Photon.Dto.Requests
{
    /// <summary>
    /// Request sent by Photon to store a new game in Redis.
    /// </summary>
    public partial class GameCreateRequest : WebhookRequest
    {
        /// <summary>
        /// The game information.
        /// </summary>
        public GameBase Game { get; set; }
    }
}
