using DragaliaAPI.Models;

namespace DragaliaAPI.Services.Exceptions;

public class DungeonException : DragaliaException
{
    public DungeonException(string dungeonKeyId)
        : base(ResultCode.DungeonAreaNotFound, $"Failed to lookup dungeon: {dungeonKeyId}") { }
}
