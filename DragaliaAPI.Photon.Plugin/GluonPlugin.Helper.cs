using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.Plugin.Constants;
using DragaliaAPI.Photon.Plugin.Helpers;
using DragaliaAPI.Photon.Plugin.Models;
using MessagePack;
using Newtonsoft.Json;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
{
    /// <summary>
    /// Support plugin methods.
    /// </summary>
    public partial class GluonPlugin
    {
        private const int EventDataKey = 245;
        private const int EventActorNrKey = 254;

        /// <summary>
        /// Helper method to raise events.
        /// </summary>
        /// <param name="eventCode">The event code to raise.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="target">The actor to target -- if null, all actors will be targeted.</param>
        public void RaiseEvent(Event eventCode, object eventData, int? target = null)
        {
            byte[] serializedEvent = MessagePackSerializer.Serialize(eventData, MessagePackOptions);
            Dictionary<byte, object> props = new Dictionary<byte, object>()
            {
                { EventDataKey, serializedEvent },
                { EventActorNrKey, 0 }
            };

            this.logger.InfoFormat("Raising event {0} (0x{1})", eventCode, eventCode.ToString("X"));
#if DEBUG
            this.logger.DebugFormat("Event data: {0}", JsonConvert.SerializeObject(eventData));
#endif

            if (target is null)
            {
                this.BroadcastEvent((byte)eventCode, props);
            }
            else
            {
                this.logger.DebugFormat("Event will target actor {0}", target);
                this.PluginHost.BroadcastEvent(
                    new List<int>() { target.Value },
                    0,
                    (byte)eventCode,
                    props,
                    CacheOperations.DoNotCache
                );
            }
        }

        /// <summary>
        /// Helper method to POST a JSON request body to the Redis API.
        /// </summary>
        /// <param name="endpoint">The endpoint to send a request to.</param>
        /// <param name="forwardRequest">The request object.</param>
        /// <param name="info">The event info from the current event callback.</param>
        /// <param name="callAsync">Whether or not to suspend execution of the room while awaiting a response.</param>
        private void PostStateManagerRequest(
            Uri endpoint,
            object forwardRequest,
            ICallInfo info,
            bool callAsync = true
        )
        {
            HttpRequestCallback callback = this.LogIfFailedCallback;

            MemoryStream stream = new MemoryStream();
            string json = JsonConvert.SerializeObject(
                forwardRequest,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.None,
                }
            );
            byte[] data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);

            string url = new Uri(this.config.StateManagerUrl, endpoint).AbsoluteUri;

            HttpRequest request = new HttpRequest
            {
                Url = url,
                Method = "POST",
                Accept = "application/json",
                ContentType = "application/json",
                Callback = callback,
                CustomHeaders = new Dictionary<string, string>()
                {
                    { "Authorization", $"Bearer {this.config.BearerToken}" }
                },
                DataStream = stream,
                Async = callAsync
            };

            this.PluginHost.LogDebug(
                string.Format("PostStateManagerRequest: {0} - {1}", url, json)
            );

            this.PluginHost.HttpRequest(request, info);
        }

        /// <summary>
        /// Helper method to POST a msgpack request to the main API server.
        /// </summary>
        /// <param name="endpoint">The endpoint to send a request to.</param>
        /// <param name="requestBody">The msgpack request body.</param>
        /// <param name="info">The event info from the current event callback.</param>
        /// <param name="callback">The callback to trigger on a response.</param>
        /// <param name="callAsync">Whether or not to suspend execution of the room while awaiting a response.</param>
        private void PostApiRequest(
            Uri endpoint,
            byte[] requestBody,
            IRaiseEventCallInfo info,
            HttpRequestCallback callback,
            bool callAsync = true
        )
        {
            IActor actor = this.PluginHost.GameActors.First(x => x.ActorNr == info.ActorNr);

            Uri requestUri = new Uri(this.config.ApiServerUrl, endpoint);

            HttpRequest req = new HttpRequest()
            {
                Url = requestUri.AbsoluteUri,
                ContentType = "application/octet-stream",
                Callback = callback,
                Async = callAsync,
                Accept = "application/octet-stream",
                DataStream = new MemoryStream(requestBody),
                UserState = new HttpRequestUserState() { RequestActorNr = info.ActorNr },
                Method = "POST",
                CustomHeaders = new Dictionary<string, string>()
                {
                    { "Auth-ViewerId", actor.GetViewerId().ToString() },
                    { "Authorization", $"Bearer {this.config.BearerToken}" },
                    { "RoomName", this.PluginHost.GameId },
                    {
                        "RoomId",
                        this.PluginHost.GameProperties.GetInt(GamePropertyKeys.RoomId).ToString()
                    }
                }
            };

            this.PluginHost.HttpRequest(req, info);
        }

        /// <summary>
        /// Logs an error if a HTTP response was not successful.
        /// </summary>
        /// <param name="httpResponse">The HTTP response.</param>
        /// <param name="userState">The user state.</param>
        private void LogIfFailedCallback(IHttpResponse httpResponse, object userState)
        {
            if (httpResponse.Status != HttpRequestQueueResult.Success)
            {
                this.ReportError(
                    $"Request to {httpResponse.Request.Url} failed with Photon status {httpResponse.Status} and HTTP status {httpResponse.HttpCode} ({httpResponse.Reason})"
                );
            }
        }

        /// <summary>
        /// Report an error.
        /// </summary>
        /// <param name="msg">The error message.</param>
        private void ReportError(string msg)
        {
            this.PluginHost.LogError(msg);
            this.PluginHost.BroadcastErrorInfoEvent(msg);
        }

        private int GenerateRoomId()
        {
            return this.rdm.Next(100_0000, 999_9999);
        }
    }
}
