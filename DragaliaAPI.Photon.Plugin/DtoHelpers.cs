using System.Collections;
using System.Xml.Linq;
using DragaliaAPI.Photon.Dto;
using DragaliaAPI.Photon.Dto.Game;

namespace DragaliaAPI.Photon.Plugin
{
    /// <summary>
    /// Alternative ctors for DTO classes which create them from Photon types.
    /// </summary>
    internal static class DtoHelpers
    {
        public static Player CreatePlayer(Hashtable actorProperties)
        {
            return new Player() { ViewerId = actorProperties.GetInt("PlayerId") };
        }

        public static GameBase CreateGame(string name, Hashtable gameProperties)
        {
            GameBase result = new GameBase { Name = name };

            // These props may not exist during a GameClose event
            if (
                gameProperties.TryGetInt(
                    nameof(result.MatchingCompatibleId),
                    out int matchingCompatibleId
                )
            )
            {
                result.MatchingCompatibleId = matchingCompatibleId;
            }

            if (gameProperties.TryGetInt(nameof(result.RoomId), out int roomId))
            {
                result.RoomId = roomId;
            }

            if (gameProperties.TryGetInt("C0", out int questId))
            {
                result.QuestId = questId;
            }

            return result;
        }
    }
}
