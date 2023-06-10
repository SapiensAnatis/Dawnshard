using System;
using System.Collections.Generic;

namespace DragaliaAPI.Photon.Dto.Game
{
    public interface IGame
    {
        EntryConditions EntryConditions { get; set; }

        int MatchingCompatibleId { get; set; }

        MatchingTypes MatchingType { get; set; }

        string Name { get; set; }

        List<Player> Players { get; set; }

        int QuestId { get; set; }

        int RoomId { get; set; }

        DateTimeOffset StartEntryTime { get; set; }
    }
}
