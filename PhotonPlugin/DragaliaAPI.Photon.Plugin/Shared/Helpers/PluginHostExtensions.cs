using System.Collections.Generic;
using System.Text.Json;
using DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events;
using DragaliaAPI.Photon.Plugin.Shared.Constants;
using MessagePack;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Shared.Helpers
{
    public static class PluginHostExtensions
    {
        private const int EventDataKey = 245;
        private const int EventActorNrKey = 254;

        private static readonly MessagePackSerializerOptions MessagePackOptions =
            MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

        /// <summary>
        /// Helper method to raise events.
        /// </summary>
        /// <param name="pluginHost">Instance of <see cref="IPluginHost"/>.</param>
        /// <param name="eventCode">The event code to raise.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="target">The actor to target -- if null, all actors will be targeted.</param>
        public static void RaiseEvent(
            this IPluginHost pluginHost,
            Event eventCode,
            object eventData,
            int? target = null
        )
        {
            byte[] serializedEvent = MessagePackSerializer.Serialize(eventData, MessagePackOptions);
            Dictionary<byte, object> props = new Dictionary<byte, object>()
            {
                { EventDataKey, serializedEvent },
                { EventActorNrKey, 0 }
            };

            pluginHost.LogInfo($"Raising event {eventCode} (0x{eventCode:X})");
#if DEBUG
            pluginHost.LogDebug($"Event data: {JsonSerializer.Serialize(eventData)}");
#endif

            if (target is null)
            {
                pluginHost.BroadcastEvent(ReciverGroup.All, 0, 0, (byte)eventCode, props, 0);
            }
            else
            {
                pluginHost.LogDebug($"Event will target actor {target}");
                pluginHost.BroadcastEvent(
                    new List<int>() { target.Value },
                    0,
                    (byte)eventCode,
                    props,
                    CacheOperations.DoNotCache
                );
            }
        }

        public static bool GetIsSoloPlay(this IPluginHost pluginHost) =>
            pluginHost.GameProperties.TryGetValue(
                GamePropertyKeys.IsSoloPlayWithPhoton,
                out object isSoloPlay
            ) && isSoloPlay is true;

        public static int GetQuestId(this IPluginHost pluginHost) =>
            pluginHost.GameProperties.GetInt(GamePropertyKeys.QuestId);

        /// <summary>
        /// Logs an error if a HTTP response was not successful.
        /// </summary>
        /// <param name="pluginHost">Instance of <see cref="IPluginHost"/>.</param>
        /// <param name="httpResponse">The HTTP response.</param>
        /// <param name="userState">The user state.</param>
        public static void LogIfFailedCallback(
            this IPluginHost pluginHost,
            IHttpResponse httpResponse,
            object userState
        )
        {
            if (httpResponse.Status == HttpRequestQueueResult.Success)
                return;

            string msg =
                $"Request to {httpResponse.Request.Url} failed with Photon status {httpResponse.Status} and HTTP status {httpResponse.HttpCode} ({httpResponse.Reason})";

            pluginHost.LogError(msg);
            pluginHost.BroadcastErrorInfoEvent(msg);
        }
    }
}
