using System;
using DragaliaAPI.Photon.Plugin.Shared.Constants;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Shared.Helpers
{
    public static class ActorExtensions
    {
        public static bool TryGetViewerId(this IActor actor, out long viewerId)
        {
            viewerId = 0;

            if (!actor.Properties.TryGetValue(ActorPropertyKeys.PlayerId, out object viewerIdObj))
                return false;

            if (!(viewerIdObj is string playerIdString))
                return false;

            viewerId = long.Parse(playerIdString);
            return true;
        }

        public static long GetViewerId(this IActor actor)
        {
            if (!actor.TryGetViewerId(out long viewerId))
            {
                throw new InvalidOperationException(
                    $"Failed to get viewer ID for actor {actor.ActorNr}"
                );
            }

            return viewerId;
        }

        public static int[] GetPartySlots(this IActor actor)
        {
            return (int[])actor.Properties.GetProperty(ActorPropertyKeys.UsePartySlot).Value;
        }
    }
}
