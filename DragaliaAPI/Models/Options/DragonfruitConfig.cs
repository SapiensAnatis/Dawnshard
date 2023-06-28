namespace DragaliaAPI.Models.Options;

public class DragonfruitConfig
{
    public required Dictionary<string, DragonfruitOdds> FruitOdds { get; set; }
}

public class DragonfruitOdds
{
    public required int Normal { get; set; }
    public required int Ripe { get; set; }
    public required int Succulent { get; set; }
}
