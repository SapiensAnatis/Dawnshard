namespace DragaliaAPI.Photon.Shared.Models;

public class ActorInfo
{
    public int QuestId { get; set; }

    public int ActorNr { get; set; }

    public long ViewerId { get; set; }

    public int[] PartySlots { get; set; }
}
