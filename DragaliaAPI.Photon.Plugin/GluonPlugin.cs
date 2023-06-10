using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DragaliaAPI.Photon.Dto;
using DragaliaAPI.Photon.Dto.Game;
using DragaliaAPI.Photon.Dto.Requests;
using DragaliaAPI.Photon.Plugin.Constants;
using DragaliaAPI.Photon.Plugin.Models;
using DragaliaAPI.Photon.Plugin.Models.Events;
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

            if (!info.Request.ActorProperties.ContainsKey("ViewerId"))
            {
                info.Request.ActorProperties.Add(
                    "ViewerId",
                    info.Request.ActorProperties["PlayerId"]
                );
            }

            Random rng = new Random();
            info.Request.GameProperties.Add("RoomId", rng.Next(100_0000, 999_9999));

            base.OnCreateGame(info);

            this.PostJsonRequest(
                this.config.GameCreateEndpoint,
                new GameCreateRequest()
                {
                    Game = DtoHelpers.CreateGame(
                        this.PluginHost.GameId,
                        info.Request.GameProperties
                    ),
                    Player = DtoHelpers.CreatePlayer(info.Request.ActorProperties)
                },
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

            this.PostJsonRequest(
                this.config.GameJoinEndpoint,
                new GameModifyRequest
                {
                    GameName = this.PluginHost.GameId,
                    Player = DtoHelpers.CreatePlayer(info.Request.ActorProperties)
                },
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

            if (info.ActorNr == 1)
            {
                this.RaiseEvent(
                    0x17,
                    new RoomBroken() { Reason = RoomBroken.RoomBrokenType.HostDisconnected }
                );
            }

            IActor actor = this.PluginHost.GameActors.FirstOrDefault(
                x => x.ActorNr == info.ActorNr
            );

            info.Continue();

            if (!(actor is null) && actor.Properties.TryGetInt("PlayerId", out int viewerId))
            {
                this.PostJsonRequest(
                    this.config.GameLeaveEndpoint,
                    new GameModifyRequest
                    {
                        GameName = this.PluginHost.GameId,
                        Player = new Player() { ViewerId = viewerId }
                    },
                    info,
                    true
                );
            }
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
                new GameModifyRequest { GameName = this.PluginHost.GameId, Player = null },
                info,
                false
            );
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            this.logger.DebugFormat(
                "***************** Actor {0} raised event: 0x{1} ({2})",
                info.ActorNr,
                info.Request.EvCode.ToString("X"),
                (EventCode)info.Request.EvCode
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
                /* case 0x41: // ClearTimer
                     // Causes desync
                     this.OnClearTimer(info);
                     break;
                 case 0x60:
                     this.OnGameStepEvent(info);
                     break;*/
                default:
                    break;
            }

            base.OnRaiseEvent(info);
        }

        private void OnGameStepEvent(IRaiseEventCallInfo info)
        {
            if (
                !info.Request.Parameters.TryGetValue(245, out object value)
                || !(value is byte[] serializedEvent)
            )
            {
                return;
            }

            GameStepEvent evt = MessagePackSerializer.Deserialize<GameStepEvent>(
                serializedEvent,
                MessagePackSerializerOptions.Standard.WithCompression(
                    MessagePackCompression.Lz4Block
                )
            );

            this.logger.DebugFormat(";;;;;;;;;;;;;;; GameStepEvent StepType: {0}", evt.step);

            if (evt.step == GameStepEvent.StepTypes.JoinBeginPartySwitch)
            {
                this.RaiseEvent(
                    0x60,
                    new GameStepEvent() { step = GameStepEvent.StepTypes.JoinBeginPartySwitch }
                );
            }
        }

        private void OnClearTimer(IRaiseEventCallInfo info)
        {
            this.logger.DebugFormat(
                "OnClearTimer: {0}",
                JsonConvert.SerializeObject(info.Request.Parameters)
            );
            this.RaiseEvent(0x2, new GameEndEvent()); // GameEnd
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

            if (info.Request.Properties.ContainsKey(ActorPropertyKeys.GoToIngameState))
                this.OnSetGoToIngameState(info);

            if (info.Request.Properties.ContainsKey(GamePropertyKeys.EntryConditions))
                this.OnSetEntryConditions(info);

            base.OnSetProperties(info);
        }

        private void OnSetEntryConditions(ISetPropertiesCallInfo info)
        {
            EntryConditions newEntryConditions = DtoHelpers.CreateEntryConditions(
                info.Request.Properties
            );

            if (newEntryConditions is null)
                return;

            this.PostJsonRequest(
                this.config.EntryConditionsEndpoint,
                new GameModifyConditionsRequest()
                {
                    GameName = this.PluginHost.GameId,
                    NewEntryConditions = newEntryConditions,
                    Player = null
                },
                info,
                callAsync: false
            );
        }

        private void OnSetGoToIngameState(ISetPropertiesCallInfo info)
        {
            int value = info.Request.Properties.GetInt(ActorPropertyKeys.GoToIngameState);

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

            byte[] msgpack = MessagePackSerializer.Serialize(
                data,
                MessagePackSerializerOptions.Standard.WithCompression(
                    MessagePackCompression.Lz4Block
                )
            );

            this.PluginHost.SetProperties(
                0,
                new Hashtable() { { "GoToIngameInfo", msgpack } },
                null,
                true
            );
        }

        private void RaiseCharacterDataEvent(ISetPropertiesCallInfo info)
        {
            foreach (IActor actor in this.PluginHost.GameActorsActive)
            {
                if (!actor.Properties.TryGetInt("PlayerId", out int viewerId))
                {
                    this.ReportError($"Failed to get viewer id for actor {actor.ActorNr}");
                }

                int[] partySlots = (int[])actor.Properties.GetProperty("UsePartySlot").Value;
                this.logger.DebugFormat("PartySlots: {0}", JsonConvert.SerializeObject(partySlots));

                foreach (int slot in partySlots)
                {
                    HttpRequest req = new HttpRequest()
                    {
                        Url = $"http://localhost:5000/heroparam/{viewerId}?partySlot1={slot}",
                        ContentType = "application/json",
                        Callback = OnHeroParamResponse,
                        Async = true,
                        Accept = "application/json",
                        UserState = new HttpRequestUserState()
                        {
                            OwnerActorNr = actor.ActorNr,
                            RequestActorNr = info.ActorNr
                        },
                    };

                    this.PluginHost.HttpRequest(req, info);
                }
            }
        }

        private void OnHeroParamResponse(IHttpResponse response, object userState)
        {
            if (response.Status != 0)
                this.ReportError($"HeroParam request failed with code {response.Status}");

            List<HeroParam> heroParams = JsonConvert.DeserializeObject<List<HeroParam>>(
                response.ResponseText
            );

            HttpRequestUserState typedUserState = (HttpRequestUserState)userState;

            CharacterData evt = new CharacterData()
            {
                playerId = typedUserState.OwnerActorNr,
                heroParamExs = heroParams
                    .Select(
                        x =>
                            new HeroParamExData()
                            {
                                limitOverCount = x.exAbilityLv,
                                sequenceNumber = x.position
                            }
                    )
                    .ToArray(),
            };

            HeroParam[] selectedHeroParams = heroParams
                .Take(GetMemberCount(typedUserState.OwnerActorNr))
                .ToArray();

            if (typedUserState.UnusedHeroParam)
                evt.unusedHeroParams = selectedHeroParams;
            else
                evt.heroParams = selectedHeroParams;

            this.RaiseEvent(0x14, evt, typedUserState.RequestActorNr);
        }

        private void RaisePartyEvent(ISetPropertiesCallInfo info)
        {
            PartyEvent evt = new PartyEvent()
            {
                memberCountTable = this.PluginHost.GameActors.ToDictionary(
                    x => x.ActorNr,
                    x => GetMemberCount(x.ActorNr)
                )
            };

            this.RaiseEvent(0x3e, evt);

            // Close the game now that it has started
            this.PostJsonRequest(
                this.config.GameCloseEndpoint,
                new GameModifyRequest { GameName = this.PluginHost.GameId, Player = null },
                info,
                true
            );
        }

        public int GetMemberCount(int actorNr)
        {
            int count = this.PluginHost.GameActors.Count;

            if (count < 4 && actorNr == 1)
            {
                return 2;
            }

            if (count < 3 && actorNr == 2)
            {
                return 2;
            }

            return 1;
        }

        public void RaiseEvent(byte eventCode, object eventData, int? target = null)
        {
            byte[] serializedEvent = MessagePackSerializer.Serialize(
                eventData,
                MessagePackSerializerOptions.Standard.WithCompression(
                    MessagePackCompression.Lz4Block
                )
            );
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
                        "---------------------------------- Name: {0}, byte blob: {1}",
                        key,
                        Convert.ToBase64String(roomData)
                    );
                }
                else if (table[key] is string[] unk250)
                {
                    this.logger.DebugFormat(
                        "---------------------------------- Name: {0}, string array: {1}",
                        key,
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

        private void EnsureSuccessCallback(IHttpResponse httpResponse, object userState)
        {
            if (httpResponse.Status != HttpRequestQueueResult.Success)
            {
                this.ReportError(
                    $"Request to {httpResponse.Request.Url} failed with status {httpResponse.HttpCode} ({httpResponse.Reason})"
                );
            }
        }

        private void EnsureSuccessCallbackSync(IHttpResponse response, object userState)
        {
            this.EnsureSuccessCallback(response, userState);
            response.CallInfo.Continue();
        }

        private void PostJsonRequest(
            Uri endpoint,
            object forwardRequest,
            ICallInfo info,
            bool callAsync = true
        )
        {
            HttpRequestCallback callback = this.EnsureSuccessCallback;

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

        private static string GetConfigValue(IDictionary<string, string> config, string key)
        {
            if (!config.TryGetValue(key, out string value))
                throw new ArithmeticException($"Missing {key} config parameter!");

            return value;
        }
    }
}
