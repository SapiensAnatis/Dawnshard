using DragaliaAPI.Photon.Shared.Models;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    [MessagePackObject(false)]
    public class CharacterData : EventBase<CharacterData>
    {
        [Key(1)]
        public int playerId { get; set; }

        [Key(2)]
        public HeroParam[] heroParams { get; set; }

        [Key(3)]
        public HeroParam[] unusedHeroParams { get; set; }

        [Key(4)]
        public HeroParamExData[] heroParamExs { get; set; }
    }
}
