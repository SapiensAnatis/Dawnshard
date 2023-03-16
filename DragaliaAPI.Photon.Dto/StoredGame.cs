using System.Collections.Generic;
using System.Linq;

namespace DragaliaAPI.Photon.Dto
{
    /// <summary>
    /// An object representing an open game.
    /// </summary>
    public class StoredGame : GameDto
    {
        /// <summary>
        /// The viewer ID of the host of this game.
        /// </summary>
        public int HostViewerId { get; set; }

        /// <summary>
        /// The list of players in this game, identified by viewer ID.
        /// </summary>
        public IEnumerable<int> Players { get; set; } = Enumerable.Empty<int>();
    }
}
