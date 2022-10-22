namespace DragaliaAPI.Models.Data;

[Flags]
public enum ManaNodes
{
    Clear = 0,
    Node1 = 0b_0000_0000_0001,
    Node2 = 0b_0000_0000_0010,
    Node3 = 0b_0000_0000_0100,
    Node4 = 0b_0000_0000_1000,
    Node5 = 0b_0000_0001_0000,
    Node6 = 0b_0000_0010_0000,
    Node7 = 0b_0000_0100_0000,
    Node8 = 0b_0000_1000_0000,
    Node9 = 0b_0001_0000_0000,
    Circle1 = 0b_0010_0000_0000,
    Circle2 = 0b_0100_0000_0000,
    Circle3 = 0b_0110_0000_0000,
    Circle4 = 0b_1000_0000_0000,
    Circle5 = 0b_1010_0000_0000,
    Circle6 = 0b_1100_0000_0000,
    Circle7 = 0b_1110_0000_0000
}

public static class ManaNodesUtil
{
    public static ManaNodes SetManaCircleNodesFromSet(IEnumerable<int> enumerable)
    {
        if (!enumerable.GetEnumerator().MoveNext() || enumerable.Contains(0))
        {
            return ManaNodes.Clear;
        }
        int max = enumerable.Max();
        int ten = max / 10;
        int finalDigit = max % 10;
        ManaNodes flag = (ManaNodes)(ten << 9);
        ManaNodes[] allNodes = Enum.GetValues<ManaNodes>();

        for (int i = 1; !(i > finalDigit); i++)
        {
            if (enumerable.Contains(ten + i))
            {
                flag |= allNodes[i];
            }
        }
        return flag;
    }

    public static SortedSet<int> GetSetFromManaNodes(ManaNodes flag)
    {
        SortedSet<int> nodes = new();
        if (flag == ManaNodes.Clear)
        {
            return nodes;
        }
        ManaNodes circle = flag & ManaNodes.Circle7;
        int circleNr = ((int)circle >> 9) * 10;
        ManaNodes nodeNr = flag & ~ManaNodes.Circle7;
        for (int i = 1; i < circleNr; i++)
        {
            nodes.Add(i);
        }
        if (circle < ManaNodes.Circle7)
        {
            ManaNodes[] allNodes = Enum.GetValues<ManaNodes>();
            for (int i = 1; !(allNodes[i] > nodeNr); i++)
            {
                if (circle > ManaNodes.Circle4 || nodeNr.HasFlag(allNodes[i]))
                {
                    nodes.Add(circleNr + i);
                }
            }
        }
        return nodes;
    }
}
