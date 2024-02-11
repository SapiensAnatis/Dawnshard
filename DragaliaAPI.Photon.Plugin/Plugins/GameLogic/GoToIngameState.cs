#pragma warning disable IDE1006 // Naming Styles

using System.Collections.Generic;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic
{
    [MessagePackObject(false)]
    public struct GoToIngameState
    {
        [Key(0)]
        public IEnumerable<ActorData> elements { get; set; }

        [Key(1)]
        public BRInitData? brInitData { get; set; }
    }

    [MessagePackObject(false)]
    public struct BRInitData
    {
        [Key(0)]
        public int locationPattern { get; set; }

        [Key(1)]
        public List<int> playerPositions { get; set; }

        [Key(2)]
        public string multiPlayKey { get; set; }
    }

    [MessagePackObject(false)]
    public struct ActorData
    {
        [Key(0)]
        public int ActorId { get; set; }

        [Key(1)]
        public ulong ViewerId { get; set; }
    }
}
