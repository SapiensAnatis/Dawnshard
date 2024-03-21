namespace DragaliaAPI.Photon.Shared.Models;

public class HeroParamRequest
{
    public List<ActorInfo> Query { get; set; }
}

public class ActorInfo
{
    public int ActorNr { get; set; }

    public long ViewerId { get; set; }

    public int[] PartySlots { get; set; }
}
