using DragaliaAPI.Photon.Shared.Models;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events
{
    [MessagePackObject(false)]
    public class CharacterData : EventBase<CharacterData>
    {
        [Key(1)]
        public int PlayerId { get; set; }

        [Key(2)]
        public required HeroParam[] HeroParams { get; set; }

        [Key(3)]
        public HeroParam[] UnusedHeroParams { get; set; } = [];

        [Key(4)]
        public required HeroParamExData[] HeroParamExs { get; set; }
    }
}
