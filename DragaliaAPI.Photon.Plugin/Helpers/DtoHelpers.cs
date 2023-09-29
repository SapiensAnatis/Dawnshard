using System.Collections;
using System.Collections.Generic;
using DragaliaAPI.Photon.Plugin.Constants;
using DragaliaAPI.Photon.Plugin.Models;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using MessagePack;

namespace DragaliaAPI.Photon.Plugin.Helpers
{
    /// <summary>
    /// Alternative ctors for DTO classes which create them from Photon types.
    /// </summary>
    internal static class DtoHelpers
    {
        public static Player CreatePlayer(int actorNr, Hashtable actorProperties)
        {
            return new Player()
            {
                ActorNr = actorNr,
                ViewerId = actorProperties.GetInt(ActorPropertyKeys.PlayerId),
                PartyNoList = (int[])actorProperties[ActorPropertyKeys.UsePartySlot]
            };
        }

        public static GameBase CreateGame(string name, Hashtable gameProperties)
        {
            GameBase result = new GameBase
            {
                Name = name,
                MatchingCompatibleId = gameProperties.GetInt(GamePropertyKeys.MatchingCompatibleId),
                RoomId = gameProperties.GetInt(GamePropertyKeys.RoomId),
                QuestId = gameProperties.GetInt(GamePropertyKeys.QuestId),
                MatchingType = (MatchingTypes)gameProperties.GetInt(GamePropertyKeys.MatchingType)
            };

            EntryConditions conditions = CreateEntryConditions(gameProperties);
            if (conditions != null)
                result.EntryConditions = conditions;

            return result;
        }

        public static EntryConditions CreateEntryConditions(Hashtable gameProperties)
        {
            if (
                !gameProperties.TryGetValue(
                    GamePropertyKeys.EntryConditions,
                    out object entryConditionObj
                )
            )
            {
                return null;
            }

            if (!(entryConditionObj is byte[] entryConditionBlob))
                return null;

            RoomEntryCondition deserialized = MessagePackSerializer.Deserialize<RoomEntryCondition>(
                entryConditionBlob,
                MessagePackSerializerOptions.Standard.WithCompression(
                    MessagePackCompression.Lz4Block
                )
            );

            return new EntryConditions()
            {
                UnacceptedElementTypeList = deserialized.UnacceptedElementals,
                UnacceptedWeaponTypeList = deserialized.UnacceptedWeapons,
                ObjectiveTextId = deserialized.Objective.TextId,
                RequiredPartyPower = deserialized.RequiredPower
            };
        }
    }
}
