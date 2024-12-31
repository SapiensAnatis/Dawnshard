#pragma warning disable IDE1006 // Naming Styles

using System.Collections.Generic;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic
{
    [MessagePackObject(false)]
    public struct GoToIngameState
    {
        [Key(0)]
        public IEnumerable<ActorData> Elements { get; set; }

        [Key(1)]
        public BrInitData? BrInitData { get; set; }
    }

    [MessagePackObject(false)]
    public struct BrInitData
    {
        [Key(0)]
        public int LocationPattern { get; set; }

        [Key(1)]
        public List<int> PlayerPositions { get; set; }

        [Key(2)]
        public string MultiPlayKey { get; set; }
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
