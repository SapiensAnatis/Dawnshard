namespace DragaliaAPI.Services.Exceptions;

public class DungeonException : Exception
{
    public DungeonException(string dungeonKeyId) : base($"Failed to lookup dungeon: {dungeonKeyId}")
    { }
}
