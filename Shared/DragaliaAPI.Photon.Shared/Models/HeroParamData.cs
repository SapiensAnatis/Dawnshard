namespace DragaliaAPI.Photon.Shared.Models;

public class HeroParamData
{
    public int ActorNr { get; set; }

    public long ViewerId { get; set; }

    public List<List<HeroParam>> HeroParamLists { get; set; } = [];
}
