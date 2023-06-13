using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DragaliaAPI.Photon.Plugin.Constants;
using DragaliaAPI.Photon.Plugin.Helpers;
using DragaliaAPI.Photon.Plugin.Models;
using DragaliaAPI.Photon.Plugin.Models.Events;
using DragaliaAPI.Photon.Shared;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.Shared.Requests;
using MessagePack;
using Newtonsoft.Json;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
{
    public class GluonPlugin : PluginBase
    {
        private IPluginLogger logger;
        private PluginConfiguration config;

        private static readonly MessagePackSerializerOptions MessagePackOptions =
            MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

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

        /// <summary>
        /// Photon handler for when a game is created.
        /// </summary>
        /// <param name="info">Event information.</param>
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            info.Request.ActorProperties.InitializeViewerId();

            Random rng = new Random();
            info.Request.GameProperties.Add(GamePropertyKeys.RoomId, rng.Next(100_0000, 999_9999));

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
                info
            );
        }

        /// <summary>
        /// Photon handler for when a player joins an existing game.
        /// </summary>
        /// <param name="info">Event information/</param>
        public override void OnJoin(IJoinGameCallInfo info)
        {
            info.Request.ActorProperties.InitializeViewerId();

            info.Continue();

            this.PostJsonRequest(
                this.config.GameJoinEndpoint,
                new GameModifyRequest
                {
                    GameName = this.PluginHost.GameId,
                    Player = DtoHelpers.CreatePlayer(info.Request.ActorProperties)
                },
                info
            );
        }

        /// <summary>
        /// Photon handler for when a player leaves a game.
        /// </summary>
        /// <param name="info">Event information.</param>
        public override void OnLeave(ILeaveGameCallInfo info)
        {
            // Get actor before continuing
            IActor actor = this.PluginHost.GameActors.FirstOrDefault(
                x => x.ActorNr == info.ActorNr
            );

            base.OnLeave(info);

            // It is not critical to update the Redis state, so don't crash the room if we can't find
            // the actor or certain properties attached to them.
            if (actor is null)
            {
                this.logger.InfoFormat(
                    "OnLeave: could not find actor {0} -- GameLeave request aborted",
                    info.ActorNr
                );
                return;
            }

            if (actor.IsHost())
            {
                this.RaiseEvent(
                    0x17,
                    new RoomBroken() { Reason = RoomBroken.RoomBrokenType.HostDisconnected }
                );
            }

            if (
                actor.TryGetViewerId(out int viewerId)
                && !(
                    actor.Properties.GetProperty(ActorPropertyKeys.RemovedFromRedis)?.Value is true
                )
            )
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

                // For some strange reason on completing a quest this appears to be raised twice for each actor.
                // Prevent duplicate requests by setting a flag.
                actor.Properties.SetProperty(ActorPropertyKeys.RemovedFromRedis, true);
            }
        }

        /// <summary>
        /// Photon handler for when a game is closed.
        /// </summary>
        /// <param name="info">Event information.</param>
        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            this.PostJsonRequest(
                this.config.GameCloseEndpoint,
                new GameModifyRequest { GameName = this.PluginHost.GameId, Player = null },
                info,
                false
            );

            info.Continue();
        }

        /// <summary>
        /// Photon handler for when an actor raises an event.
        /// </summary>
        /// <param name="info">Event information.</param>
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
#if DEBUG
            this.logger.DebugFormat(
                "Actor {0} raised event: 0x{1} ({2})",
                info.ActorNr,
                info.Request.EvCode.ToString("X"),
                info.Request.EvCode
            );
            this.logger.DebugFormat(
                "Event properties: {0}",
                JsonConvert.SerializeObject(info.Request.Parameters)
            );
#endif

            switch (info.Request.EvCode)
            {
                case EventCodes.Ready:
                    this.OnActorReady(info);
                    break;
                default:
                    break;
            }

            base.OnRaiseEvent(info);
        }

        /// <summary>
        /// Photon handler for when an actor sets properties.
        /// </summary>
        /// <param name="info">Event information.</param>
        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
#if DEBUG
            this.logger.DebugFormat("Actor {0} set properties", info.ActorNr);
            this.logger.Debug(JsonConvert.SerializeObject(info.Request.Properties));
#endif

            if (info.Request.Properties.ContainsKey(ActorPropertyKeys.GoToIngameState))
                this.OnSetGoToIngameState(info);

            if (info.Request.Properties.ContainsKey(GamePropertyKeys.EntryConditions))
                this.OnSetEntryConditions(info);

            if (info.Request.Properties.ContainsKey(GamePropertyKeys.MatchingType))
                this.OnSetMatchingType(info);

            base.OnSetProperties(info);
        }

        /// <summary>
        /// Custom handler for when an actor raises event 0x3 (Ready.)
        /// </summary>
        /// <param name="info">Info from <see cref="OnRaiseEvent(IRaiseEventCallInfo)"/>.</param>
        public void OnActorReady(IRaiseEventCallInfo info)
        {
            this.logger.DebugFormat("Received Ready event from actor {0}", info.ActorNr);

            this.PluginHost.SetProperties(
                info.ActorNr,
                new Hashtable { { ActorPropertyKeys.StartQuest, true } },
                null,
                true
            );

            if (this.PluginHost.GameActors.All(x => x.IsReady()))
            {
                this.logger.DebugFormat(
                    "All clients were ready, raising {0}",
                    EventCodes.StartQuest
                );
                this.RaiseEvent(EventCodes.StartQuest, new Dictionary<string, string> { });
            }
        }

        /// <summary>
        /// Custom handler for when an actor sets the MatchingType property (i.e. public/private room).
        /// </summary>
        /// <param name="info">Info from <see cref="OnSetProperties(ISetPropertiesCallInfo)"/>.</param>
        private void OnSetMatchingType(ISetPropertiesCallInfo info)
        {
            MatchingTypes newType = (MatchingTypes)
                info.Request.Properties.GetInt(GamePropertyKeys.MatchingType);

            this.PostJsonRequest(
                this.config.MatchingTypeEndpoint,
                new GameModifyMatchingTypeRequest()
                {
                    GameName = this.PluginHost.GameId,
                    NewMatchingType = newType,
                    Player = null
                },
                info
            );
        }

        /// <summary>
        /// Custom handler for when an actor sets the RoomEntryCondition property (i.e. allowed weapon/element types).
        /// </summary>
        /// <param name="info"></param>
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
                info
            );
        }

        /// <summary>
        /// Custom handler for when an actor sets the GoToIngameState property.
        /// </summary>
        /// <remarks>
        /// Represents various stages of loading into a quest, during which events/properties need to be raised/set.
        /// </remarks>
        /// <param name="info">Info from <see cref="OnSetProperties(ISetPropertiesCallInfo)"/>.</param>
        private void OnSetGoToIngameState(ISetPropertiesCallInfo info)
        {
            int value = info.Request.Properties.GetInt(ActorPropertyKeys.GoToIngameState);

            switch (value)
            {
                case 1:
                    this.SetGoToIngameInfo(info);
                    if (info.ActorNr == 1)
                        this.HideGameAfterStart(info);

                    break;
                case 2:
                    this.RaiseCharacterDataEvent(info);
                    break;
                case 3:
                    this.RaisePartyEvent(info);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the GoToIngameInfo room property by gathering data from connected actors.
        /// </summary>
        /// <param name="info">Info from <see cref="OnSetProperties(ISetPropertiesCallInfo)"/>.</param>
        private void SetGoToIngameInfo(ISetPropertiesCallInfo info)
        {
            IEnumerable<ActorData> actorData = this.PluginHost.GameActors.Select(
                x => new ActorData() { ActorId = x.ActorNr, ViewerId = (ulong)x.GetViewerId() }
            );

            GoToIngameState data = new GoToIngameState()
            {
                elements = actorData,
                brInitData = null
            };

            byte[] msgpack = MessagePackSerializer.Serialize(data, MessagePackOptions);

            this.PluginHost.SetProperties(
                0,
                new Hashtable() { { GamePropertyKeys.GoToIngameInfo, msgpack } },
                null,
                true
            );
        }

        /// <summary>
        /// Raises the CharacterData event by making requests to the main API server for party information.
        /// </summary>
        /// <param name="info">Info from <see cref="OnSetProperties(ISetPropertiesCallInfo)"/>.</param>
        private void RaiseCharacterDataEvent(ISetPropertiesCallInfo info)
        {
            foreach (IActor actor in this.PluginHost.GameActorsActive)
            {
                int viewerId = actor.GetViewerId();
                int[] partySlots = actor.GetPartySlots();

                foreach (int slot in partySlots)
                {
                    Uri requestUri = new Uri(
                        this.config.ApiServerUrl,
                        $"heroparam/{viewerId}/{slot}"
                    );

                    HttpRequest req = new HttpRequest()
                    {
                        Url = requestUri.AbsoluteUri,
                        ContentType = "application/json",
                        Callback = OnHeroParamResponse,
                        Async = true,
                        Accept = "application/json",
                        UserState = new HeroParamRequestState()
                        {
                            OwnerActorNr = actor.ActorNr,
                            RequestActorNr = info.ActorNr
                        },
                    };

                    this.PluginHost.HttpRequest(req, info);
                }
            }
        }

        /// <summary>
        /// HTTP request callback for the HeroParam request sent in <see cref="RaiseCharacterDataEvent(ISetPropertiesCallInfo)"/>.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="userState">The arguments passed from the calling function.</param>
        private void OnHeroParamResponse(IHttpResponse response, object userState)
        {
            LogIfFailedCallback(response, userState);

            List<HeroParam> heroParams = JsonConvert.DeserializeObject<List<HeroParam>>(
                response.ResponseText
            );

            HeroParamRequestState typedUserState = (HeroParamRequestState)userState;

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
                heroParams = heroParams.Take(GetMemberCount(typedUserState.OwnerActorNr)).ToArray()
            };

            this.RaiseEvent(0x14, evt, typedUserState.RequestActorNr);
        }

        /// <summary>
        /// Raises the Party event containing information about how many characters each player owns.
        /// </summary>
        /// <param name="info">Info from <see cref="OnSetProperties(ISetPropertiesCallInfo)"/>.</param>
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
        }

        /// <summary>
        /// Send a request to the Redis API to hide a game as it is now started.
        /// </summary>
        /// <param name="info">Info from <see cref="OnSetProperties(ISetPropertiesCallInfo)"/>.</param>
        private void HideGameAfterStart(ISetPropertiesCallInfo info)
        {
            this.PostJsonRequest(
                this.config.MatchingTypeEndpoint,
                new GameModifyMatchingTypeRequest
                {
                    NewMatchingType = MatchingTypes.NoDisplay,
                    GameName = this.PluginHost.GameId,
                    Player = null
                },
                info,
                true
            );
        }

        /// <summary>
        /// Gets how many units an actor should control based on the number of ingame players.
        /// </summary>
        /// <param name="actorNr">The actor number.</param>
        /// <returns>The number of units they own.</returns>
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

        /// <summary>
        /// Helper method to raise events.
        /// </summary>
        /// <param name="eventCode">The event code to raise.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="target">The actor to target -- if null, all actors will be targeted.</param>
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
                "Raising event 0x{0} with data {1}",
                eventCode.ToString("X"),
                JsonConvert.SerializeObject(eventData)
            );

            if (target is null)
            {
                this.BroadcastEvent(eventCode, props);
            }
            else
            {
                this.logger.DebugFormat("Event will target actor {0}", target);
                this.PluginHost.BroadcastEvent(
                    new List<int>() { target.Value },
                    0,
                    eventCode,
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
        private void PostJsonRequest(
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

            this.PluginHost.LogDebug(string.Format("PostJsonRequest: {0} - {1}", url, json));

            this.PluginHost.HttpRequest(request, info);
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
    }
}
