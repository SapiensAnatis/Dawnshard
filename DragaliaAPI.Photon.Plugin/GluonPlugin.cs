using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.Dto;
using DragaliaAPI.Photon.Plugin.Models;
using DragaliaAPI.Photon.Plugin.Models.Events;
using K4os.Compression.LZ4;
using MessagePack;
using Newtonsoft.Json;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
{
    public class GluonPlugin : PluginBase
    {
        private IPluginLogger logger;

        private PluginConfiguration config;

        public override string Name => nameof(GluonPlugin);

        public override bool SetupInstance(
            IPluginHost host,
            Dictionary<string, string> config,
            out string errorMsg
        )
        {
            this.logger = host.CreateLogger(this.Name);

            this.config = new PluginConfiguration(config);

            return base.SetupInstance(host, config, out errorMsg);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            /*
            this.logger.DebugFormat(
                "---------------------------------- OnCreateGame {0} by user {1}",
                info.Request.GameId,
                info.UserId
            );
            this.logger.DebugFormat("------------------------------------ actor properties start");
            LogHashtable(info.Request.ActorProperties);
            this.logger.DebugFormat("------------------------------------ actor properties end");
            this.logger.DebugFormat("------------------------------------ game properties start");
            LogHashtable(info.Request.GameProperties);
            this.logger.DebugFormat("------------------------------------ game properties end");

            this.logger.DebugFormat("------------------------------------ unk properties start");
            LogHashtable(info.CreateOptions["CustomProperties"] as IDictionary);
            this.logger.DebugFormat("------------------------------------ unk properties end");
            */

            Random rng = new Random();

            if (!info.Request.ActorProperties.ContainsKey("ViewerId"))
            {
                info.Request.ActorProperties.Add(
                    "ViewerId",
                    info.Request.ActorProperties["PlayerId"]
                );
            }

            info.Request.GameProperties.Add("RoomId", rng.Next(100_0000, 999_9999));

            info.Continue();

            this.PostJsonRequest(
                this.config.GameCreateEndpoint,
                new WebhookRequest()
                {
                    Player = DtoHelpers.CreatePlayer(info.Request.ActorProperties),
                    Game = DtoHelpers.CreateGame(info.Request.GameId, info.Request.GameProperties)
                },
                this.LogIfFailedCallback,
                info,
                callAsync: true
            );
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            /*
            this.logger.DebugFormat("------------------------------------  Joined game!");
            this.logger.DebugFormat("------------------------------------ actor properties start");
            LogHashtable(info.Request.ActorProperties);
            this.logger.DebugFormat("------------------------------------ actor properties end");
            this.logger.DebugFormat("------------------------------------ game properties start");
            LogHashtable(info.Request.GameProperties);
            this.logger.DebugFormat("------------------------------------ game properties end");
            */

            if (!info.Request.ActorProperties.ContainsKey("ViewerId"))
            {
                info.Request.ActorProperties.Add(
                    "ViewerId",
                    info.Request.ActorProperties["PlayerId"]
                );
            }

            info.Continue();

            int viewerId = info.Request.ActorProperties.GetInt("ViewerId");

            this.PostJsonRequest(
                this.config.GameJoinEndpoint,
                new WebhookRequest
                {
                    Game = DtoHelpers.CreateGame(
                        this.PluginHost.GameId,
                        info.Request.GameProperties
                    ),
                    Player = DtoHelpers.CreatePlayer(info.Request.ActorProperties)
                },
                this.LogIfFailedCallback,
                info,
                callAsync: true
            );
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            this.logger.DebugFormat("------------------------------------  Left game!");
            this.logger.DebugFormat("------------------------------------ parameters start");
            LogHashtable(info.Request.Parameters);
            this.logger.DebugFormat("------------------------------------ parameters end");

            object viewerIdObj = PluginHost.GameActors
                .First(x => x.ActorNr == info.ActorNr)
                .Properties.GetProperty("PlayerId")
                .Value;

            info.Continue();

            this.PostJsonRequest(
                this.config.GameLeaveEndpoint,
                new WebhookRequest
                {
                    Game = DtoHelpers.CreateGame(
                        this.PluginHost.GameId,
                        this.PluginHost.GameProperties
                    ),
                    Player = new PlayerDto() { ViewerId = int.Parse((string)viewerIdObj) }
                },
                this.LogIfFailedCallback,
                info,
                true
            );
        }

        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            this.logger.DebugFormat("------------------------------------  Closed game!");
            this.logger.DebugFormat("------------------------------------ parameters start");
            LogHashtable(info.Request.Parameters);
            this.logger.DebugFormat("------------------------------------ parameters end");

            info.Continue();

            this.PostJsonRequest(
                this.config.GameCloseEndpoint,
                new WebhookRequest
                {
                    Game = DtoHelpers.CreateGame(
                        this.PluginHost.GameId,
                        this.PluginHost.GameProperties
                    ),
                },
                this.LogIfFailedCallback,
                info,
                true
            );
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            this.logger.DebugFormat(
                "***************** Actor {0} raised event: 0x{1}",
                info.ActorNr,
                info.Request.EvCode.ToString("X")
            );
            this.logger.DebugFormat(
                "***************** Event properties: {0}",
                JsonConvert.SerializeObject(info.Request.Parameters)
            );

            switch (info.Request.EvCode)
            {
                case 3:
                    this.OnActorReady(info);
                    break;
                default:
                    break;
            }

            base.OnRaiseEvent(info);
        }

        public void OnActorReady(IRaiseEventCallInfo info)
        {
            this.logger.DebugFormat("Received Ready (3) event from actor {0}", info.ActorNr);

            this.PluginHost.SetProperties(
                info.ActorNr,
                new Hashtable { { "StartQuest", true } },
                null,
                true
            );

            foreach (IActor actor in this.PluginHost.GameActors)
            {
                actor.Properties.TryGetValue("StartQuest", out object value);
                this.logger.DebugFormat("Actor {0} StartQuest {1}", actor.ActorNr, value);

                if (!(value is bool boolValue && boolValue))
                    return;
            }

            this.logger.DebugFormat("All clients were ready, raising {0}", 0x15);

            this.RaiseEvent(0x15, new Dictionary<string, string> { });
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            this.logger.DebugFormat("Actor {0} set properties", info.ActorNr);
            this.LogHashtable(info.Request.Parameters);

            if (info.Request.Properties.TryGetInt("GoToIngameState", out int value))
            {
                switch (value)
                {
                    case 1:
                        this.SetGoToIngameInfo(info);
                        break;
                    case 2:
                        this.RaiseCharacterDataEvent(info);
                        break;
                    case 3:
                        this.RaisePartyEvent(info);
                        break;
                    default:
                        this.logger.InfoFormat("Unhandled GoToIngameState: {0}", value);
                        break;
                }

                this.RaiseEvent(
                    0x45,
                    new DebugInspectionRequest()
                    {
                        requestType = DebugInspectionRequest.RequestTypes.IngameState
                    }
                );
            }

            base.OnSetProperties(info);
        }

        private void SetGoToIngameInfo(ISetPropertiesCallInfo info)
        {
            List<ActorData> actorData = new List<ActorData>();

            foreach (IActor actor in this.PluginHost.GameActors)
            {
                if (!actor.Properties.TryGetInt("PlayerId", out int viewerId))
                {
                    throw new InvalidOperationException(
                        $"Actor {actor.ActorNr}: failed to get viewer id!"
                    );
                }

                actorData.Add(
                    new ActorData() { ActorId = actor.ActorNr, ViewerId = (ulong)viewerId }
                );
            }

            GoToIngameState data = new GoToIngameState()
            {
                elements = actorData,
                brInitData = null
            };

            byte[] msgpack = LZ4MessagePackSerializer.Serialize(data);

            this.PluginHost.SetProperties(
                0,
                new Hashtable() { { "GoToIngameInfo", msgpack } },
                null,
                true
            );

            //this.PostJsonRequest(
            //    this.config.GameCloseEndpoint,
            //    new WebhookRequest
            //    {
            //        Game = DtoHelpers.CreateGame(
            //            this.PluginHost.GameId,
            //            this.PluginHost.GameProperties
            //        ),
            //    },
            //    this.LogIfFailedCallback,
            //    info,
            //    true
            //);
        }

        private void RaiseCharacterDataEvent(ISetPropertiesCallInfo info)
        {
            foreach (IActor actor in this.PluginHost.GameActorsActive)
            {
                CharacterData evt = new CharacterData()
                {
                    playerId = actor.ActorNr,
                    heroParamExs = new HeroParamExData[2]
                    {
                        new HeroParamExData() { sequenceNumber = 0 },
                        new HeroParamExData() { sequenceNumber = 1 }
                    },
                    unusedHeroParams = new HeroParam[1]
                    {
                        new HeroParam(10150202, 100, 30160204, 100, 20050113, 100)
                        {
                            isFriend = true
                        },
                    },
                    heroParams = new HeroParam[1]
                    {
                        new HeroParam(10150202, 100, 30160204, 100, 20050113, 100)
                        {
                            isFriend = true,
                            position = 1,
                        },
                    }
                };

                this.RaiseEvent(0x14, evt, info.ActorNr);
            }
        }

        private void RaisePartyEvent(ISetPropertiesCallInfo info)
        {
            PartyEvent evt = new PartyEvent()
            {
                memberCountTable = new Dictionary<int, int>()
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 }
                }
            };

            this.RaiseEvent(0x3e, evt);
        }

        public void RaiseEvent(byte eventCode, object eventData, int? target = null)
        {
            byte[] serializedEvent = LZ4MessagePackSerializer.Serialize(eventData);
            Dictionary<byte, object> props = new Dictionary<byte, object>()
            {
                { 245, serializedEvent },
                { 254, 0 } // Server actor number
            };

            this.logger.DebugFormat(
                "!!!!!!!!!!!!!! Raising event 0x{0} with data {1}",
                eventCode.ToString("X"),
                JsonConvert.SerializeObject(eventData)
            );

            if (target is null)
            {
                this.BroadcastEvent(eventCode, props);
            }
            else
            {
                this.logger.DebugFormat("!!!!!!!!!!!!!! Targeting actor {0}", target);
                this.PluginHost.BroadcastEvent(
                    new List<int>() { target.Value },
                    0,
                    eventCode,
                    props,
                    CacheOperations.DoNotCache
                );
            }
        }

        private void LogHashtable(IDictionary table)
        {
            if (table is null)
            {
                this.logger.Debug("Table was null");
                return;
            }

            foreach (object key in table.Keys)
            {
                if (table[key] is byte[] roomData)
                {
                    this.logger.DebugFormat(
                        "---------------------------------- yoyoyo room data just dropped {0}",
                        Convert.ToBase64String(roomData)
                    );
                }
                else if (table[key] is string[] unk250)
                {
                    this.logger.DebugFormat(
                        "---------------------------------- yoyoyo game data just dropped {0}",
                        string.Join(",", unk250)
                    );
                }
                else if (table[key] is Hashtable h)
                {
                    this.LogHashtable(h);
                }
                else
                {
                    this.logger.DebugFormat(
                        "---------------------------------- Name: {0}, value: {1}",
                        key,
                        table[key]
                    );
                }
            }
        }

        private void LogIfFailedCallback(IHttpResponse httpResponse, object userState)
        {
            if (
                !this.TryGetForwardResponse(
                    httpResponse,
                    out WebhookResponse _,
                    out string errorMsg
                )
            )
            {
                this.ReportError(
                    string.Format(
                        "Failed to forward request to {0} : {1}",
                        httpResponse.Request.Url,
                        errorMsg
                    )
                );
            }
        }

        private void PostJsonRequest(
            Uri endpoint,
            object forwardRequest,
            HttpRequestCallback callback,
            ICallInfo info,
            bool callAsync = false
        )
        {
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

            string url = new Uri(this.config.BaseUrl, endpoint).AbsoluteUri;

            HttpRequest request = new HttpRequest
            {
                Url = url,
                Method = "POST",
                Accept = "application/json",
                ContentType = "application/json",
                Callback = callback,
                CustomHeaders = new Dictionary<string, string>()
                {
                    { "Authorization", $"Basic {this.config.BasicAuthCredentials}" }
                },
                DataStream = stream,
                Async = callAsync
            };

            this.PluginHost.LogDebug(string.Format("PostJsonRequest: {0} - {1}", url, json));

            this.PluginHost.HttpRequest(request, info);
        }

        private void ReportError(string msg)
        {
            this.PluginHost.LogError(msg);
            this.PluginHost.BroadcastErrorInfoEvent(msg);
        }

        private void SaveCallback(IHttpResponse request, object userState)
        {
            ICloseGameCallInfo info = (ICloseGameCallInfo)request.CallInfo;

            if (request.Status == HttpRequestQueueResult.Success)
            {
                this.PluginHost.LogDebug(
                    string.Format(
                        "Http request to {0} - done.",
                        info.ActorCount > 0 ? "save" : "close"
                    )
                );
                info.Continue();
                return;
            }

            string msg = string.Format(
                "Failed save game request on {0} : {1}",
                this.config.GameCloseEndpoint,
                request.Reason
            );
            this.ReportError(msg);
            info.Continue();
        }

        private void CloseGameCallback(IHttpResponse response, object userState)
        {
            this.LogIfFailedCallback(response, userState);
            response.CallInfo.Continue();
        }

        private static readonly char[] trimBomAndZeroWidth = { '\uFEFF', '\u200B' };

        private bool TryGetForwardResponse(
            IHttpResponse httpResponse,
            out WebhookResponse response,
            out string errorMsg
        )
        {
            response = null;

            try
            {
                if (httpResponse.Status == HttpRequestQueueResult.Success)
                {
                    if (string.IsNullOrEmpty(httpResponse.ResponseText))
                    {
                        errorMsg = string.Format("Missing Response.");
                        return false;
                    }

                    string responseData = Encoding.UTF8
                        .GetString(httpResponse.ResponseData)
                        .Trim(trimBomAndZeroWidth);
                    this.PluginHost.LogDebug("TryGetForwardResponse:" + responseData);

                    if (!responseData.StartsWith("{") && !responseData.EndsWith("}"))
                    {
                        errorMsg = string.Format("Response is not valid json '{0}'.", responseData);
                        return false;
                    }

                    response = JsonConvert.DeserializeObject<WebhookResponse>(responseData);
                    if (response != null)
                    {
                        if (response.ResultCode == 0)
                        {
                            errorMsg = string.Empty;
                            return true;
                        }

                        if (response.ResultCode == 255 && string.IsNullOrEmpty(response.Message))
                        {
                            errorMsg = string.Format("Unexpected Response '{0}'.", responseData);
                            return false;
                        }

                        errorMsg = string.Format(
                            "Error response ResultCode='{0}' Message='{1}'.",
                            response.ResultCode,
                            response.Message
                        );
                    }
                    else
                    {
                        // since we prevalidate, we don't seam to get here
                        errorMsg = string.Format("Unexpected Response '{0}'.", responseData);
                    }
                }
                else
                {
                    errorMsg = string.Format(
                        "'{0}' httpcode={1} webstatus={2} response='{3}', HttpQueueResult={4}.",
                        httpResponse.Reason,
                        httpResponse.HttpCode,
                        httpResponse.WebStatus,
                        httpResponse.ResponseText,
                        httpResponse.Status
                    );
                }
            }
            catch (Exception ex)
            {
                // since we prevalidate, we don't seam to get here
                errorMsg =
                    $"{ex.Message}, data:{BitConverter.ToString(httpResponse.ResponseData, 0, Math.Min(1024, httpResponse.ResponseData.Length))}";
            }

            return false;
        }

        private static string GetConfigValue(IDictionary<string, string> config, string key)
        {
            if (!config.TryGetValue(key, out string value))
                throw new ArithmeticException($"Missing {key} config parameter!");

            return value;
        }
    }
}
