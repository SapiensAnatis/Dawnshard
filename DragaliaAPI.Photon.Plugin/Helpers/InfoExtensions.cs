using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.Plugin.Constants;
using DragaliaAPI.Photon.Plugin.Models.Events;
using MessagePack;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Helpers
{
    public static class InfoExtensions
    {
        public static TEvent DeserializeEvent<TEvent>(this IRaiseEventCallInfo info)
            where TEvent : EventBase<TEvent>
        {
            if (
                !info.Request.Parameters.TryGetValue(
                    Event.Constants.EventDataKey,
                    out object eventDataObj
                ) || !(eventDataObj is byte[] blob)
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
