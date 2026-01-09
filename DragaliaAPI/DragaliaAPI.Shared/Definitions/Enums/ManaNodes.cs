namespace DragaliaAPI.Shared.Definitions.Enums;

// csharpier-ignore-start
[Flags]
public enum ManaNodes : ushort
{
    Clear   = 0,
    Node1   = 0b_0000_0000_0000_0001,
    Node2   = 0b_0000_0000_0000_0010,
    Node3   = 0b_0000_0000_0000_0100,
    Node4   = 0b_0000_0000_0000_1000,
    Node5   = 0b_0000_0000_0001_0000,
    Node6   = 0b_0000_0000_0010_0000,
    Node7   = 0b_0000_0000_0100_0000,
    Node8   = 0b_0000_0000_1000_0000,
    Node9   = 0b_0000_0001_0000_0000,
    Node10  = 0b_0000_0010_0000_0000,
    Circle1 = 0b_0000_0100_0000_0000,
    Circle2 = 0b_0000_1000_0000_0000,
    Circle3 = 0b_0000_1100_0000_0000,
    Circle4 = 0b_0001_0000_0000_0000,
    Circle5 = 0b_0001_0100_0000_0000,
    Circle6 = 0b_0001_1000_0000_0000,
    Circle7 = 0b_0001_1100_0000_0000,
}
// csharpier-ignore-end

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
        SortedSet<int> set = enumerable as SortedSet<int> ?? new SortedSet<int>(enumerable);

        if (set.Count == 0 || set.Contains(0))
        {
            return ManaNodes.Clear;
        }

        int max = set.Max;
        ManaNodes flag = baseNodes;

        // Bottom limit on for loop - how many nodes are already unlocked?
        int minNode = 10 * ((int)(flag & ManaNodes.Circle7) >> 10);

        // These flags are usually set by setting LimitBreakCount but not for circle 6 and 7 i.e. mana spirals
        if (max is > 50 and < 71)
        {
            ManaNodes existingNonLimitBreakFlags = flag & ~ManaNodes.Circle7;
            ManaNodes newLimitBreakFlag = (ManaNodes)((max / 10) << 10);

            flag = (ManaNodes)
                Math.Min(
                    (int)(newLimitBreakFlag | existingNonLimitBreakFlags),
                    (ushort)ManaNodes.Circle7
                );
        }

        ManaNodes[] allNodes = Enum.GetValues<ManaNodes>();

        for (int i = minNode + 1; i <= max; ++i)
        {
            if (set.Contains(i))
            {
                int id = (i - minNode) % 10;
                flag |= allNodes[id == 0 ? 10 : id];
            }

            if (i is > 50 and < 71 && i % 10 == 0)
            {
                // In the mana spiral, if we reach a limit break, discard non limit break nodes
                // Consider the transition from node 59 to 60:
                // Node 59: ManaNodes.Circle5 | ManaNodes.Node9 ... ManaNodes.Node1
                // Node 60: ManaNodes.Circle6
                // Node 61: ManaNodes.Circle6 | ManaNodes.Node1
                // We can use Circle7 as a 'spiral limit break only' bitmask because Circle7 = Circle6 | Circle5
                flag &= ManaNodes.Circle7;
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
