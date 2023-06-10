using System;
using System.Collections.Generic;

namespace DragaliaAPI.Photon.Shared.Models
{
    public class GameBase : IGame
    {
        public int RoomId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int MatchingCompatibleId { get; set; }

        public MatchingTypes MatchingType { get; set; }

        public int QuestId { get; set; }

        public EntryConditions EntryConditions { get; set; } = new EntryConditions();

        public DateTimeOffset StartEntryTime { get; set; } = DateTimeOffset.UtcNow;

        public List<Player> Players { get; set; } = new List<Player>();
    }
}
