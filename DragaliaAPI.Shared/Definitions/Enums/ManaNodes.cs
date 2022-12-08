using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
    public const ManaNodes MaxManaNodesSpiral = ManaNodes.Circle7 - 1;

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

    public struct ManaNodeInfo
    {
        public enum NodeTypes
        {
            Hp = 10101,
            Atk = 10102,
            HpAtk = 10103,
            FS = 10201,
            S1 = 10401,
            S2 = 10402,
            A1 = 10301,
            A2 = 10302,
            A3 = 10303,
            Ex = 10501,
            Mat = 10601,
            MaxLvUp = 10801,
            StdAtkUp = 10701
        }

        [JsonPropertyName("MC")]
        public string ManaCircleName { get; set; }

        public bool IsReleaseStory { get; set; }

        [JsonPropertyName("TypeId")]
        public NodeTypes NodeType { get; set; }

        [JsonPropertyName("NodeNr")]
        public int NodeNr { get; set; }

        public ManaNodeInfo(
            NodeTypes nodeType,
            int nodeNr,
            string manaCircleName = "",
            bool isReleaseStory = false
        )
        {
            ManaCircleName = manaCircleName;
            NodeType = nodeType;
            NodeNr = nodeNr;
            IsReleaseStory = isReleaseStory;
        }

        public override string ToString()
        {
            return $"ManaCircleName={ManaCircleName}, NodeNr={NodeNr}, NodeType={NodeType}, IsReleaseStory={IsReleaseStory}";
        }
    }
}
