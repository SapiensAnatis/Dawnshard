using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic
{
    [MessagePackObject]
    public class HeroParamExData
    {
        [Key(0)]
        public int sequenceNumber;

        [Key(1)]
        public int limitOverCount;

        [Key(2)]
        public int brSpecialSkillId;
    }
}
