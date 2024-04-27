namespace DragaliaAPI.Features.StorySkip;

public class FortConfig
{
    public int BuildCount { get; set; }
    public int Level { get; set; }
    public FortConfig(int level, int buildCount)
    {
        BuildCount = buildCount;
        Level = level;
    }
}