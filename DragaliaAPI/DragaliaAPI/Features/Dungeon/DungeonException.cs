using DragaliaAPI.Features.Shared;
using DragaliaAPI.Infrastructure.Exceptions;
using DragaliaAPI.Infrastructure.Results;
using DragaliaAPI.Services.Exceptions;

namespace DragaliaAPI.Features.Dungeon;

public class DungeonException : DragaliaException
{
    public DungeonException(string dungeonKeyId)
        : base(ResultCode.DungeonAreaNotFound, $"Failed to lookup dungeon: {dungeonKeyId}") { }
}
