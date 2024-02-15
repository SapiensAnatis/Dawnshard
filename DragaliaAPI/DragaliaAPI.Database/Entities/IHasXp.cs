namespace DragaliaAPI.Database.Entities;

public interface IHasXp
{
    public int Exp { get; set; }

    public byte Level { get; set; }
}
