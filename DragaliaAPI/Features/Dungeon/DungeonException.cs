using DragaliaAPI.Services.Exceptions;

namespace DragaliaAPI.Features.Dungeon;

public class DungeonException : DragaliaException
{
    public DungeonException(string dungeonKeyId)
        : base(ResultCode.DungeonAreaNotFound, $"Failed to lookup dungeon: {dungeonKeyId}") { }
}
