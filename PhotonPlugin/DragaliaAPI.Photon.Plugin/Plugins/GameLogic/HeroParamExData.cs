using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic
{
    [MessagePackObject]
    public class HeroParamExData
    {
        [Key(0)]
        public int SequenceNumber;

        [Key(1)]
        public int LimitOverCount;

        [Key(2)]
        public int BrSpecialSkillId;
    }
}
