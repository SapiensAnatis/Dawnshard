using System;
using DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events;
using MessagePack;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Shared.Helpers
{
    public static class InfoExtensions
    {
        private const int EventDataKey = 245;

        public static TEvent DeserializeEvent<TEvent>(this IRaiseEventCallInfo info)
            where TEvent : EventBase<TEvent>
        {
            if (
                !info.Request.Parameters.TryGetValue(EventDataKey, out object eventDataObj)
                || !(eventDataObj is byte[] blob)
            )
            {
                throw new ArgumentException(
                    "Attempted to deserialize event without event data property",
                    nameof(info)
                );
            }

            return MessagePackSerializer.Deserialize<TEvent>(
                blob,
                MessagePackSerializerOptions.Standard.WithCompression(
                    MessagePackCompression.Lz4Block
                )
            );
        }
    }
}
