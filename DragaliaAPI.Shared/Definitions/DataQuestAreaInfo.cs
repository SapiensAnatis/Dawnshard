namespace DragaliaAPI.Shared.Definitions;

public class DataQuestAreaInfo
{
    public DataQuestAreaInfo(string scenePath, string areaName)
    {
        this.ScenePath = scenePath;
        this.AreaName = areaName;
    }

    public string ScenePath { get; set; }

    public string AreaName { get; set; }
}
