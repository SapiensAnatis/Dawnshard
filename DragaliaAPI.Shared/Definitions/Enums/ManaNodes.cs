using System.Collections.Immutable;

namespace DragaliaAPI.Shared.Definitions.Enums;

[Flags]
public enum ManaNodes : ushort
{
    Clear = 0,
    Node1 = 0b_0000_0000_0000_0001,
    Node2 = 0b_0000_0000_0000_0010,
    Node3 = 0b_0000_0000_0000_0100,
    Node4 = 0b_0000_0000_0000_1000,
    Node5 = 0b_0000_0000_0001_0000,
    Node6 = 0b_0000_0000_0010_0000,
    Node7 = 0b_0000_0000_0100_0000,
    Node8 = 0b_0000_0000_1000_0000,
    Node9 = 0b_0000_0001_0000_0000,
    Node10 = 0b_0000_0010_0000_0000,
    Circle1 = 0b_0000_0100_0000_0000,
    Circle2 = 0b_0000_1000_0000_0000,
    Circle3 = 0b_0000_1100_0000_0000,
    Circle4 = 0b_0001_0000_0000_0000,
    Circle5 = 0b_0001_0100_0000_0000,
    Circle6 = 0b_0001_1000_0000_0000,
    Circle7 = 0b_0001_1100_0000_0000
}

public static class ManaNodesUtil
{
    public const byte MaxLimitbreak = 4;
    public const byte MaxLimitbreakSpiral = 5;
    public const ManaNodes MaxManaNodes = ManaNodes.Circle5 - 1;

    public static ManaNodes SetManaCircleNodesFromSet(
        IEnumerable<int> enumerable,
        ManaNodes baseNodes = ManaNodes.Clear
    )
    {
        if (!enumerable.Any() || enumerable.Contains(0))
            return ManaNodes.Clear;

        int max = enumerable.Max();
        int finalDigit = max % 10 == 0 ? 10 : max % 10;
        int ten = (max - finalDigit) / 10;
        ManaNodes flag = baseNodes;
        ManaNodes[] allNodes = Enum.GetValues<ManaNodes>();

        for (int i = 1; !(i > finalDigit); i++)
        {
            if (enumerable.Contains((ten * 10) + i))
            {
                flag |= allNodes[i];
            }
        }
        //These flags are usually set by limitbreaking but not for circle 6 and 7, so I manually do
        if (max > 50 && max < 71)
        {
            flag = (ManaNodes)
                Math.Min(
                    (int)(flag & ~ManaNodes.Circle7) | ((max / 10) << 10),
                    (ushort)ManaNodes.Circle7
                );
        }
        return flag;
    }

    public static SortedSet<int> GetSetFromManaNodes(ManaNodes flag)
    {
        SortedSet<int> nodes = new SortedSet<int>();
        if (flag == ManaNodes.Clear)
            return nodes;
        ManaNodes circle = flag & ManaNodes.Circle7;
        int circleNr = ((int)circle >> 10) * 10;
        ManaNodes nodeNr = flag & ~ManaNodes.Circle7;
        for (int i = 1; !(i > circleNr); i++)
        {
            nodes.Add(i);
        }
        if (circle < ManaNodes.Circle7)
        {
            ManaNodes[] allNodes = Enum.GetValues<ManaNodes>();
            for (int i = 1; !(allNodes[i] > nodeNr); i++)
            {
                if (circle > ManaNodes.Circle4 || nodeNr.HasFlag(allNodes[i]))
                    nodes.Add(circleNr + i);
            }
        }
        return nodes;
    }

    public class ManaNodeInfo
    {
        public enum UpgradeAbilityTypes
        {
            FS = EffectTypes.FS,
            S1 = EffectTypes.S1,
            S2 = EffectTypes.S2,
            A1 = EffectTypes.A1,
            A2 = EffectTypes.A2,
            A3 = EffectTypes.A3,
            Ex = EffectTypes.Ex,
            MaxLvUp = EffectTypes.MaxLvUp,
            StdAtkUp = EffectTypes.StdAtkUp
        }

        [Flags]
        public enum EffectTypes
        {
            Hp = 1,
            Atk = 2,
            FS = 4,
            S1 = 8,
            S2 = 16,
            A1 = 32,
            A2 = 64,
            A3 = 128,
            Ex = 256,
            Mat = 512,
            MaxLvUp = 1024,
            StdAtkUp = 2048
        }

        public int? StoryId { get; }
        public Materials? MatId { get; }
        public EffectTypes EffectType { get; }
        public int? HpAdd { get; }
        public int? AtkAdd { get; }

        private ManaNodeInfo() { }

        private ManaNodeInfo(
            EffectTypes effectType,
            int? storyId = null,
            Materials? matId = null,
            int? hpAdd = null,
            int? atkAdd = null
        )
        {
            this.EffectType = effectType;
            this.MatId = matId;
            this.StoryId = storyId;
            this.HpAdd = hpAdd;
            this.AtkAdd = atkAdd;
        }

        public static ManaNodeInfo CreateHpNode(int hpPlus, int? storyId = null) =>
            new ManaNodeInfo(EffectTypes.Hp, storyId: storyId, hpAdd: hpPlus);

        public static ManaNodeInfo CreateAtkNode(int atkPlus, int? storyId = null) =>
            new ManaNodeInfo(EffectTypes.Atk, storyId: storyId, atkAdd: atkPlus);

        public static ManaNodeInfo CreateHpAtkNode(int hpPlus, int atkPlus, int? storyId = null) =>
            new ManaNodeInfo(
                EffectTypes.Atk | EffectTypes.Hp,
                storyId,
                hpAdd: hpPlus,
                atkAdd: atkPlus
            );

        public static ManaNodeInfo CreateHpAtkNode(Materials? material) =>
            new ManaNodeInfo(EffectTypes.Mat, matId: material);

        public static ManaNodeInfo CreateAbilityUpgradeNode(UpgradeAbilityTypes type) =>
            new ManaNodeInfo((EffectTypes)type);
    }
}
