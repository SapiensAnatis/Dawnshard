using System;
using System.Collections.Generic;

namespace DragaliaAPI.Photon.Dto.Game
{
    /// <summary>
    /// Data transfer object for a game to be sent by Photon and stored in Redis.
    /// </summary>
    public class GameBase
    {
        /// <summary>
        /// The room id / passcode.
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// The unique name of the game.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The compatibility ID of this game.
        /// </summary>
        public int MatchingCompatibleId { get; set; }

        /// <summary>
        /// The quest ID for this game.
        /// </summary>
        public int QuestId { get; set; }

        /// <summary>
        /// The time the room was opened.
        /// </summary>
        public DateTimeOffset StartEntryTime { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// The entry conditions attached to this room.
        /// </summary>
        public EntryConditions EntryConditions { get; set; } = new EntryConditions();

        /// <summary>
        /// The list of players in this game.
        /// </summary>
        public List<Player> Players { get; set; } = new List<Player>();

        /// <summary>
        /// Whether the room should be shown to players.
        /// </summary>
        public bool Visible { get; set; } = true;
    }
}
