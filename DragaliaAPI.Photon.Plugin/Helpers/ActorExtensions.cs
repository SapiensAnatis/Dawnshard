using System;
using System.Runtime.CompilerServices;
using DragaliaAPI.Photon.Plugin.Constants;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Helpers
{
    public static class ActorExtensions
    {
        public static bool TryGetViewerId(this IActor actor, out int viewerId)
        {
            return actor.Properties.TryGetInt(ActorPropertyKeys.PlayerId, out viewerId);
        }

        public static int GetViewerId(this IActor actor)
        {
            if (!actor.TryGetViewerId(out int viewerId))
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
