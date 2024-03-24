using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events;
using DragaliaAPI.Photon.Plugin.Shared;
using DragaliaAPI.Photon.Plugin.Shared.Constants;
using DragaliaAPI.Photon.Plugin.Shared.Helpers;
using DragaliaAPI.Photon.Shared.Enums;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic
{
    /// <summary>
    /// Sub-plugin for handling Dragalia game logic.
    /// </summary>
    public class GameLogicPlugin : PluginBase
    {
        private IPluginLogger logger;
        private RoomState roomState;
        private GoToIngameStateManager goToIngameStateManager;

        private readonly PluginConfiguration configuration;
        private readonly PluginStateService pluginStateService;
        private readonly Random random;
        private readonly Dictionary<int, ActorState> actorState;

        public override string Name => nameof(GameLogicPlugin);

        public GameLogicPlugin(
            PluginStateService pluginStateService,
            PluginConfiguration configuration
        )
        {
            this.configuration = configuration;
            this.pluginStateService = pluginStateService;

            this.random = new Random();
            this.actorState = new Dictionary<int, ActorState>(4);
            this.roomState = new RoomState();
        }

        public override bool SetupInstance(
            IPluginHost host,
            Dictionary<string, string> config,
            out string errorMsg
        )
        {
            this.logger = host.CreateLogger(this.Name);
            this.goToIngameStateManager = new GoToIngameStateManager(
                host,
                this.pluginStateService,
                this.configuration
            );

            return base.SetupInstance(host, config, out errorMsg);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            // Not 0 to allow for any outgoing GameLeave Redis requests to complete
            info.Request.EmptyRoomLiveTime = 500;

#if DEBUG
            this.logger.DebugFormat(
                "Room properties: {0}",
                JsonSerializer.Serialize(info.Request.GameProperties)
            );
#endif
            info.Request.ActorProperties.InitializeViewerId();

            int roomId = this.GenerateRoomId();
            info.Request.GameProperties.Add(GamePropertyKeys.RoomId, roomId);

            info.Continue();

            // https://doc.photonengine.com/server/current/plugins/plugins-faq#how_to_get_the_actor_number_in_plugin_callbacks_
            // This is only invalid if the room is recreated from an inactive state, which Dragalia doesn't do (hopefully!)
            const int actorNr = 1;
            this.actorState[actorNr] = new ActorState();

            long viewerId = info.Request.ActorProperties.GetLong(ActorPropertyKeys.PlayerId);

            if (
                this.configuration.EnableSecondaryServer
                && viewerId >= this.configuration.SecondaryViewerIdCriterion
            )
            {
                this.logger.Info("Using secondary server config");
                this.pluginStateService.IsUseSecondaryServer = true;
            }
            else
            {
                this.logger.Info("Using primary server config");
                this.pluginStateService.IsUseSecondaryServer = false;
            }

            if (
                info.Request.GameProperties.TryGetValue(
                    GamePropertyKeys.IsSoloPlayWithPhoton,
                    out object isSoloPlay
                ) && isSoloPlay is true
            )
            {
                this.logger.Info("Room is in solo play mode");
                this.roomState.IsSoloPlay = true;
            }

            this.roomState.QuestId = info.Request.GameProperties.GetInt(GamePropertyKeys.QuestId);
            this.roomState.IsRandomMatching = QuestHelper.GetIsRandomMatching(
                this.roomState.QuestId
            );
            MatchingTypes matchingType = (MatchingTypes)
                info.Request.GameProperties.GetInt(GamePropertyKeys.MatchingType);

            this.logger.InfoFormat(
                "Viewer ID {0} created room {1} with room ID {2}",
                info.Request.ActorProperties.GetInt(ActorPropertyKeys.ViewerId),
                this.PluginHost.GameId,
                roomId
            );

            this.pluginStateService.ShouldPublish =
                matchingType == MatchingTypes.Anyone && !this.roomState.IsRandomMatching;
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            int currentActorCount = this.PluginHost.GameActors.Count(x =>
                x.ActorNr != info.ActorNr
            );

            long viewerId = info.Request.ActorProperties.GetLong(ActorPropertyKeys.PlayerId);

            if (currentActorCount >= 4)
            {
                this.logger.WarnFormat(
                    "Rejecting join request -- game already has {0} actors",
                    currentActorCount
                );

                info.Fail();
                return;
            }

            if (this.goToIngameStateManager.MinGoToIngameState > 0)
            {
                this.logger.InfoFormat("Rejecting join request -- room is already in progress");

                info.Fail();
                return;
            }

            // ReSharper disable once SimplifyLinqExpressionUseAll
            if (!this.PluginHost.GameActors.Any(x => x.ActorNr == 1))
            {
                this.logger.InfoFormat("Rejecting join request -- room has no host");

                info.Fail();
                return;
            }

            if (
                this.pluginStateService.IsUseSecondaryServer
                && viewerId < this.configuration.SecondaryViewerIdCriterion
            )
            {
                this.logger.InfoFormat(
                    "Rejecting join request -- viewer id {0} below secondary criteria {1}",
                    viewerId,
                    this.configuration.SecondaryViewerIdCriterion
                );

                info.Fail();
                return;
            }

            if (
                !this.pluginStateService.IsUseSecondaryServer
                && viewerId > this.configuration.SecondaryViewerIdCriterion
            )
            {
                this.logger.InfoFormat(
                    "Rejecting join request -- viewer id {0} exceeds secondary criteria {1}",
                    viewerId,
                    this.configuration.SecondaryViewerIdCriterion
                );

                info.Fail();
                return;
            }

            info.Request.ActorProperties.InitializeViewerId();
            info.Continue();

            this.actorState[info.ActorNr] = new ActorState();

            this.logger.InfoFormat(
                "Viewer ID {0} joined game {1}",
                info.Request.ActorProperties.GetInt(ActorPropertyKeys.ViewerId),
                this.PluginHost.GameId
            );

            if (this.roomState.IsRandomMatching && this.roomState.RandomMatchingStartTimer == null)
            {
                this.logger.InfoFormat(
                    "Commencing random matching start timer for room {0}",
                    this.PluginHost.GameId
                );

                this.roomState.RandomMatchingStartTimer = this.PluginHost.CreateOneTimeTimer(
                    info,
                    () =>
                    {
                        this.logger.InfoFormat(
                            "Executing random matching start for room {0}",
                            this.PluginHost.GameId
                        );
                        this.goToIngameStateManager.SetGoToIngameInfo();
                    },
                    this.configuration.RandomMatchingStartDelayMs
                );
            }
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            // Get actor before continuing
            IActor actor = this.PluginHost.GameActors.FirstOrDefault(x =>
                x.ActorNr == info.ActorNr
            );

            if (info.ActorNr == -1)
            {
                // Actor was never really in the room, server disconnect, etc.
                return;
            }

            if (info.ActorNr == 1)
            {
                this.PluginHost.RaiseEvent(
                    Event.RoomBroken,
                    new RoomBroken() { Reason = RoomBroken.RoomBrokenType.HostDisconnected }
                );
            }

            if (!this.actorState.Remove(info.ActorNr))
            {
                this.logger.WarnFormat(
                    "Failed to remove actor nr {0} from actor state. Leave reason: {1}",
                    info.ActorNr,
                    info.Reason
                );
            }

            // It is not critical to update the Redis state, so don't crash the room if we can't find
            // the actor or certain properties attached to them.
            if (actor is null)
            {
                this.logger.WarnFormat(
                    "OnLeave: could not find actor {0} -- GameLeave request aborted",
                    info.ActorNr
                );
                return;
            }

            if (!actor.TryGetViewerId(out long viewerId))
            {
                this.logger.WarnFormat(
                    "OnLeave: failed to acquire viewer ID of actor {0}",
                    info.ActorNr
                );
                return;
            }

            this.logger.InfoFormat(
                "Actor {0} with viewer ID {1} left game {2}",
                info.ActorNr,
                viewerId,
                this.PluginHost.GameId
            );

            this.goToIngameStateManager.OnActorLeave(info);

            if (
                this.goToIngameStateManager.MinGoToIngameState > 0
                && this.actorState.Where(x => x.Key != info.ActorNr).All(x => x.Value.Ready)
            )
            {
                this.PluginHost.RaiseEvent(Event.StartQuest, new Dictionary<string, string>());
            }

            if (this.actorState.Count < 2 && this.roomState.RandomMatchingStartTimer != null)
            {
                this.logger.InfoFormat(
                    "Aborting random matching timer of room {0} because room only has 1 player",
                    this.PluginHost.GameId
                );

                this.PluginHost.StopTimer(this.roomState.RandomMatchingStartTimer);
                this.roomState.RandomMatchingStartTimer = null;
            }
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            info.Continue();

#if DEBUG
            this.logger.DebugFormat(
                "Actor {0} raised event: {1} (0x{2})",
                info.ActorNr,
                (Event)info.Request.EvCode,
                info.Request.EvCode.ToString("X")
            );
            this.logger.DebugFormat(
                "Event properties: {0}",
                JsonSerializer.Serialize(info.Request.Parameters)
            );
#endif

            switch ((Event)info.Request.EvCode)
            {
                case Event.Ready:
                    this.OnActorReady(info);
                    break;
                case Event.ClearQuestRequest:
                    this.OnClearQuestRequest(info);
                    break;
                case Event.GameSucceed:
                    this.OnGameSucceed(info);
                    break;
                case Event.FailQuestRequest:
                    this.OnFailQuestRequest(info);
                    break;
                case Event.Dead:
                    // !!! TODO: How does this behave with AI units?
                    this.actorState[info.ActorNr].Dead = true;
                    break;
            }
        }

        public override void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            if (
                info.Request.Properties.TryGetValue(
                    ActorPropertyKeys.GoToIngameState,
                    out object objValue
                ) && objValue is int value
            )
            {
                this.goToIngameStateManager.OnSetGoToIngameState(info, value);
            }

            if (!info.IsProcessed)
            {
                info.Continue();
            }
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            info.Continue();

#if DEBUG
            this.logger.DebugFormat("Actor {0} set properties", info.ActorNr);
            this.logger.Debug(JsonSerializer.Serialize(info.Request.Properties));
#endif
        }

        /// <summary>
        /// Handler for when the client calls <see cref="Event.FailQuestRequest"/>.
        /// </summary>
        /// <param name="info">Event call info.</param>
        private void OnFailQuestRequest(IRaiseEventCallInfo info)
        {
            this.actorState[info.ActorNr].Ready = false;

            this.PluginHost.SetProperties(
                info.ActorNr,
                new Hashtable() { { ActorPropertyKeys.GoToIngameState, 0 }, },
                null,
                false
            );

            FailQuestRequest request = info.DeserializeEvent<FailQuestRequest>();

            this.logger.DebugFormat(
                "Received FailQuestRequest with FailType {0}",
                request.FailType.ToString()
            );

            FailQuestResponse response = new FailQuestResponse()
            {
                ResultType =
                    request.FailType == FailQuestRequest.FailTypes.Timeup
                        ? FailQuestResponse.ResultTypes.Timeup
                        : FailQuestResponse.ResultTypes.Clear
            };

            this.PluginHost.RaiseEvent(Event.FailQuestResponse, response, info.ActorNr);

            if (
                this.PluginHost.GameActors.Count < this.roomState.StartActorCount
                || this.actorState.All(x => x.Value.Dead)
            )
            {
                // Return to lobby
                this.logger.DebugFormat("FailQuestRequest: returning to lobby");
                this.actorState[info.ActorNr] = new ActorState();

                this.PluginHost.SetProperties(
                    0,
                    new Hashtable()
                    {
                        { GamePropertyKeys.GoToIngameInfo, null },
                        { GamePropertyKeys.RoomId, -1 }
                    },
                    null,
                    true
                );
            }

            this.roomState = new RoomState(this.roomState);
            this.goToIngameStateManager = new GoToIngameStateManager(
                this.PluginHost,
                this.pluginStateService,
                this.configuration
            );
        }

        /// <summary>
        /// Handler for when a client calls <see cref="Event.GameSucceed"/>.
        /// </summary>
        /// <param name="info">Info from <see cref="OnRaiseEvent(IRaiseEventCallInfo)"/>.</param>
        private void OnGameSucceed(IRaiseEventCallInfo info)
        {
            this.logger.InfoFormat("Received GameSucceed from actor {0}", info.ActorNr);

            if (info.ActorNr == 1)
            {
                this.roomState = new RoomState(this.roomState);
                this.goToIngameStateManager = new GoToIngameStateManager(
                    this.PluginHost,
                    this.pluginStateService,
                    this.configuration
                );

                this.PluginHost.RaiseEvent(Event.GameSucceed, new { });
                this.PluginHost.SetProperties(
                    0,
                    new Hashtable() { [GamePropertyKeys.RoomId] = this.GenerateRoomId() },
                    null,
                    true
                );
            }
        }

        /// <summary>
        /// Custom handler for when an actor raises event 0x3 (Ready.)
        /// </summary>
        /// <param name="info">Info from <see cref="OnRaiseEvent(IRaiseEventCallInfo)"/>.</param>
        private void OnActorReady(IRaiseEventCallInfo info)
        {
            this.logger.InfoFormat("Received Ready event from actor {0}", info.ActorNr);
            this.actorState[info.ActorNr].Ready = true;

            if (this.actorState.All(x => x.Value.Ready))
            {
                this.PluginHost.RaiseEvent(Event.StartQuest, new Dictionary<string, string>());

                this.roomState.StartActorCount = this.PluginHost.GameActors.Count;
            }
        }

        /// <summary>
        /// Handler for when the client raises <see cref="Event.ClearQuestRequest"/>.
        /// </summary>
        /// <param name="info">Event call info.</param>
        private void OnClearQuestRequest(IRaiseEventCallInfo info)
        {
            // These properties must be set for the client to successfully rejoin the room.
            int roomId = this.PluginHost.GameProperties.GetInt(GamePropertyKeys.RoomId);

            if (roomId > 0)
            {
                this.PluginHost.SetProperties(
                    0,
                    new Hashtable()
                    {
                        { GamePropertyKeys.GoToIngameInfo, null },
                        { GamePropertyKeys.RoomId, -this.GenerateRoomId() }
                    },
                    null,
                    true
                );
            }

            this.actorState[info.ActorNr] = new ActorState();

            ClearQuestRequest evt = info.DeserializeEvent<ClearQuestRequest>();

            this.PostApiRequest(
                this.configuration.DungeonRecordMultiEndpoint,
                evt.RecordMultiRequest,
                info,
                this.ClearQuestRequestCallback,
                callAsync: false
            );

            if (this.ShouldRegisterTimeAttack())
            {
                this.logger.Info("Registering time attack clear");

                this.PostApiRequest(
                    this.configuration.TimeAttackEndpoint,
                    evt.RecordMultiRequest,
                    info,
                    this.PluginHost.LogIfFailedCallback,
                    callAsync: true
                );
            }
        }

        private bool ShouldRegisterTimeAttack()
        {
            if (!QuestHelper.GetIsRanked(this.roomState.QuestId))
            {
                this.logger.InfoFormat(
                    "Not registering TA clear -- quest {0} is not ranked",
                    this.roomState.QuestId
                );
                return false;
            }

            if (!this.roomState.IsSoloPlay && this.PluginHost.GameActors.Count() < 4)
            {
                this.logger.InfoFormat(
                    "Not registering TA clear -- game actor count {0} < 4",
                    this.PluginHost.GameActors.Count()
                );
                return false;
            }

            return true;
        }

        /// <summary>
        /// Callback for HTTP request sent in <see cref="OnClearQuestRequest(IRaiseEventCallInfo)"/>.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="userState">The user state.</param>
        private void ClearQuestRequestCallback(IHttpResponse response, object userState)
        {
            this.PluginHost.LogIfFailedCallback(response, userState);

            if (response.Status != HttpRequestQueueResult.Success)
                return;

            HttpRequestUserState typedUserState = (HttpRequestUserState)userState;

            this.PluginHost.RaiseEvent(
                Event.ClearQuestResponse,
                new ClearQuestResponse() { RecordMultiResponse = response.ResponseData },
                typedUserState.RequestActorNr
            );
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

            Uri baseUri;
            string bearerToken;

            if (this.pluginStateService.IsUseSecondaryServer)
            {
                baseUri = this.configuration.SecondaryApiServerUrl;
                bearerToken = this.configuration.SecondaryBearerToken;
            }
            else
            {
                baseUri = this.configuration.ApiServerUrl;
                bearerToken = this.configuration.BearerToken;
            }

            Uri requestUri = new Uri(baseUri, endpoint);

            this.logger.DebugFormat("PostApiRequest: {0}", requestUri.AbsoluteUri);

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
                    { "Authorization", $"Bearer {bearerToken}" },
                    { "RoomName", this.PluginHost.GameId },
                    {
                        "RoomId",
                        this.PluginHost.GameProperties.GetInt(GamePropertyKeys.RoomId).ToString()
                    }
                }
            };

            this.PluginHost.HttpRequest(req, info);
        }

        private int GenerateRoomId() => this.random.Next(100_0000, 1_000_0000);
    }
}
