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
        ManaNodes flag = baseNodes;

        //These flags are usually set by limitbreaking but not for circle 6 and 7, so I manually do
        if (max is > 50 and < 71)
        {
            flag = (ManaNodes)
                Math.Min(
                    (int)(flag & ~ManaNodes.Circle7) | ((max / 10) << 10),
                    (ushort)ManaNodes.Circle7
                );
        }

        ManaNodes[] allNodes = Enum.GetValues<ManaNodes>();

        int minNode = 10 * ((int)(flag & ManaNodes.Circle7) >> 10);

        for (int i = minNode + 1; i <= max; ++i)
        {
            if (enumerable.Contains(i))
            {
                int id = (i - minNode) % 10;
                flag |= allNodes[id == 0 ? 10 : id];
            }
        }

        return flag;
    }

    public static SortedSet<int> GetSetFromManaNodes(ManaNodes flag)
    {
        SortedSet<int> nodes = new();
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
}
