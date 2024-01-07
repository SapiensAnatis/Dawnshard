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
using DragaliaAPI.Photon.Shared.Models;
using MessagePack;
using Newtonsoft.Json;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
{
    /// <summary>
    /// Sub-plugin for handling Dragalia game logic.
    /// </summary>
    public class GameLogicPlugin : PluginBase
    {
        private const int EventDataKey = 245;
        private const int EventActorNrKey = 254;

        private static readonly MessagePackSerializerOptions MessagePackOptions =
            MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

        private IPluginLogger logger;
        private RoomState roomState;

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

            return base.SetupInstance(host, config, out errorMsg);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            // Not 0 to allow for any outgoing GameLeave Redis requests to complete
            info.Request.EmptyRoomLiveTime = 500;

#if DEBUG
            this.logger.DebugFormat(
                "Room properties: {0}",
                JsonConvert.SerializeObject(info.Request.GameProperties)
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
            bool isUseSecondaryServer;

            if (
                this.configuration.EnableSecondaryServer
                && viewerId >= this.configuration.SecondaryViewerIdCriterion
            )
            {
                this.logger.Info("Using secondary server config");
                isUseSecondaryServer = true;
            }
            else
            {
                this.logger.Info("Using primary server config");
                isUseSecondaryServer = false;
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

            this.logger.InfoFormat(
                "Viewer ID {0} created room {1} with room ID {2}",
                info.Request.ActorProperties.GetInt(ActorPropertyKeys.ViewerId),
                this.PluginHost.GameId,
                roomId
            );

            this.pluginStateService.ShouldPublishToStateManager = !this.roomState.IsRandomMatching;
            this.pluginStateService.IsUseSecondaryServer = isUseSecondaryServer;
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            int currentActorCount = this.PluginHost.GameActors.Count(
                x => x.ActorNr != info.ActorNr
            );

            long viewerId = info.Request.ActorProperties.GetLong(ActorPropertyKeys.PlayerId);

            if (currentActorCount >= 4)
            {
                this.logger.WarnFormat(
                    "Player attempted to join game which already had {0} actors",
                    currentActorCount
                );

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
                        this.SetGoToIngameInfo();
                    },
                    this.configuration.RandomMatchingStartDelayMs
                );
            }
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            // Get actor before continuing
            IActor actor = this.PluginHost.GameActors.FirstOrDefault(
                x => x.ActorNr == info.ActorNr
            );

            if (info.ActorNr == -1)
            {
                // Actor was never really in the room, server disconnect, etc.
                return;
            }

            if (info.ActorNr == 1)
            {
                this.RaiseEvent(
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

            if (this.roomState.MinGoToIngameState > 0)
            {
                int newMinGoToIngameState = this.PluginHost.GameActors.Where(
                    x => x.ActorNr != info.ActorNr
                )
                    .Select(x => x.Properties.GetIntOrDefault(ActorPropertyKeys.GoToIngameState))
                    .DefaultIfEmpty()
                    .Min();

                this.roomState.MinGoToIngameState = newMinGoToIngameState;
                this.OnSetGoToIngameState(info);

                if (this.actorState.Where(x => x.Key != info.ActorNr).All(x => x.Value.Ready))
                {
                    this.RaiseEvent(Event.StartQuest, new Dictionary<string, string>());
                }
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
                JsonConvert.SerializeObject(info.Request.Parameters)
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
                // Wait for everyone to reach a particular GoToIngameState value before doing anything.
                // But let the host set GoToIngameState = 1 unilaterally to signal the game start process.

                int minValue = this.PluginHost.GameActors.Where(x => x.ActorNr != info.ActorNr) // Exclude the value which we are in the BeforeSet handler for
                    .Select(x => x.Properties.GetIntOrDefault(ActorPropertyKeys.GoToIngameState))
                    .Concat(new[] { value }) // Fun fact: Enumerable.Append() was added in .NET 4.7.1
                    .Min();

                this.logger.InfoFormat(
                    "Received GoToIngameState {0} from actor {1}",
                    value,
                    info.ActorNr
                );

#if DEBUG
                this.logger.DebugFormat(
                    "Calculated minimum value: {0}, instance minimum value {1}",
                    minValue,
                    this.roomState.MinGoToIngameState
                );
#endif

                if (minValue > this.roomState.MinGoToIngameState)
                {
                    this.roomState.MinGoToIngameState = minValue;
                    this.OnSetGoToIngameState(info);
                }
                else if (value == 1 && info.ActorNr == 1)
                {
                    this.roomState.MinGoToIngameState = value;
                    this.OnSetGoToIngameState(info);
                }
                else if (value == 0 && this.roomState.IsSoloPlay)
                {
                    this.SetGoToIngameInfo();
                    this.RaiseEvent(Event.StartQuest, new Dictionary<string, object>());
                }
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
            this.logger.Debug(JsonConvert.SerializeObject(info.Request.Properties));
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

            this.RaiseEvent(Event.FailQuestResponse, response, info.ActorNr);

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
                this.RaiseEvent(Event.GameSucceed, new { });
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
                this.RaiseEvent(Event.StartQuest, new Dictionary<string, string>());

                this.roomState.StartActorCount = this.PluginHost.GameActors.Count;
            }
        }

        /// <summary>
        /// Custom handler for when an actor sets the GoToIngameState property.
        /// </summary>
        /// <remarks>
        /// Represents various stages of loading into a quest, during which events/properties need to be raised/set.
        /// </remarks>
        /// <param name="info">Call info.</param>
        private void OnSetGoToIngameState(ICallInfo info)
        {
            this.logger.InfoFormat(
                "OnSetGoToIngameState: updating with value {0}",
                this.roomState.MinGoToIngameState
            );

            switch (this.roomState.MinGoToIngameState)
            {
                case 1:
                    this.SetGoToIngameInfo();
                    break;
                case 2:
                    this.RequestHeroParam(info);
                    break;
                case 3:
                    this.RaisePartyEvent();
                    this.RaiseCharacterDataEvent();
                    break;
            }
        }

        /// <summary>
        /// Raise <see cref="Event.CharacterData"/> using cached <see cref="HeroParamData"/>.
        /// </summary>
        private void RaiseCharacterDataEvent()
        {
            foreach (IActor actor in this.PluginHost.GameActors)
            {
                if (!this.actorState.TryGetValue(actor.ActorNr, out ActorState state))
                {
                    this.logger.InfoFormat(
                        "Skipping actor {0} chara data -- not in actor state",
                        actor.ActorNr
                    );

                    continue;
                }

                foreach (List<HeroParam> heroParams in state.HeroParamData.HeroParamLists)
                {
                    CharacterData evt = new CharacterData()
                    {
                        playerId = actor.ActorNr,
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
                        heroParams = heroParams.Take(state.MemberCount).ToArray()
                    };

                    this.RaiseEvent(Event.CharacterData, evt);
                }
            }
        }

        /// <summary>
        /// Sets the GoToIngameInfo room property by gathering data from connected actors.
        /// </summary>
        private void SetGoToIngameInfo()
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
        /// Makes an outgoing request for <see cref="HeroParamData"/> for each player in the room.
        /// </summary>
        /// <param name="info">Call info.</param>
        private void RequestHeroParam(ICallInfo info)
        {
            IEnumerable<ActorInfo> heroParamRequest = this.PluginHost.GameActors.Select(
                x =>
                    new ActorInfo()
                    {
                        ActorNr = x.ActorNr,
                        ViewerId = x.GetViewerId(),
                        PartySlots = x.GetPartySlots()
                    }
            );

            Uri baseUri = this.pluginStateService.IsUseSecondaryServer
                ? this.configuration.SecondaryApiServerUrl
                : this.configuration.ApiServerUrl;

            Uri requestUri = new Uri(baseUri, "heroparam/batch");

            this.logger.DebugFormat("RequestHeroParam - {0}", requestUri.AbsoluteUri);

            HttpRequest req = new HttpRequest()
            {
                Url = requestUri.AbsoluteUri,
                ContentType = "application/json",
                Callback = HeroParamRequestCallback,
                Async = false,
                Accept = "application/json",
                DataStream = new MemoryStream(
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(heroParamRequest))
                ),
                Method = "POST",
            };

            this.PluginHost.HttpRequest(req, info);
        }

        /// <summary>
        /// HTTP request callback for the HeroParam request sent in <see cref="RequestHeroParam(ICallInfo)"/>.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="userState">The arguments passed from the calling function.</param>
        private void HeroParamRequestCallback(IHttpResponse response, object userState)
        {
            this.LogIfFailedCallback(response, userState);

            if (response.Status != HttpRequestQueueResult.Success)
                return;

            List<HeroParamData> responseObject = JsonConvert.DeserializeObject<List<HeroParamData>>(
                response.ResponseText
            );

            foreach (HeroParamData data in responseObject)
            {
                if (this.actorState.TryGetValue(data.ActorNr, out ActorState value))
                {
                    value.HeroParamData = data;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Event.Party"/> event.
        /// </summary>
        private void RaisePartyEvent()
        {
            Dictionary<int, int> memberCountTable = this.GetMemberCountTable();

            foreach (IActor actor in this.PluginHost.GameActors)
                this.actorState[actor.ActorNr].MemberCount = memberCountTable[actor.ActorNr];

            int rankingType = QuestHelper.GetIsRanked(this.roomState.QuestId) ? 1 : 0;

            PartyEvent evt = new PartyEvent()
            {
                MemberCountTable = memberCountTable,
                ReBattleCount = this.configuration.ReplayTimeoutSeconds,
                RankingType = rankingType,
            };

            this.RaiseEvent(Event.Party, evt);
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
                        { GamePropertyKeys.RoomId, -GenerateRoomId() }
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
                ClearQuestRequestCallback,
                callAsync: false
            );

            if (this.ShouldRegisterTimeAttack())
            {
                logger.Info("Registering time attack clear");

                this.PostApiRequest(
                    this.configuration.TimeAttackEndpoint,
                    evt.RecordMultiRequest,
                    info,
                    LogIfFailedCallback,
                    callAsync: true
                );
            }
        }

        private bool ShouldRegisterTimeAttack()
        {
            if (!QuestHelper.GetIsRanked(this.roomState.QuestId))
            {
                logger.InfoFormat(
                    "Not registering TA clear -- quest {0} is not ranked",
                    this.roomState.QuestId
                );
                return false;
            }

            if (!this.roomState.IsSoloPlay && this.PluginHost.GameActors.Count() < 4)
            {
                logger.InfoFormat(
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
            this.LogIfFailedCallback(response, userState);

            if (response.Status != HttpRequestQueueResult.Success)
                return;

            HttpRequestUserState typedUserState = (HttpRequestUserState)userState;

            this.RaiseEvent(
                Event.ClearQuestResponse,
                new ClearQuestResponse() { RecordMultiResponse = response.ResponseData },
                typedUserState.RequestActorNr
            );
        }

        /// <summary>
        /// Gets how many units each actor should control based on the number of ingame players.
        /// </summary>
        /// <returns>A dictionary of { [actorNr] = unitCount }</returns>
        private Dictionary<int, int> GetMemberCountTable()
        {
            bool isRaid =
                this.PluginHost.GameProperties.TryGetInt(GamePropertyKeys.QuestId, out int questId)
                && QuestHelper.GetIsRaid(questId);

            if (isRaid || this.roomState.IsSoloPlay)
            {
                // Use all available units
                return this.PluginHost.GameActors.ToDictionary(
                    x => x.ActorNr,
                    x => this.actorState[x.ActorNr].HeroParamCount
                );
            }

            return BuildMemberCountTable(
                this.PluginHost.GameActors.Join(
                    this.actorState,
                    actor => actor.ActorNr,
                    state => state.Key,
                    (actor, state) =>
                        new ValueTuple<int, int>(actor.ActorNr, state.Value.HeroParamCount)
                )
                    .ToList()
            );
        }

        /// <summary>
        /// Static unit-testable method to build the member count table.
        /// </summary>
        /// <param name="actorData">List of actors and how many hero params they have.</param>
        /// <returns>The member count table.</returns>
        public static Dictionary<int, int> BuildMemberCountTable(
            List<(int ActorNr, int HeroParamCount)> actorData
        )
        {
            Dictionary<int, int> result = actorData.ToDictionary(x => x.ActorNr, x => 1);

            if (result.Count == 4)
                return result;

            // Add first AI units
            foreach ((int actorNr, int heroParamCount) in actorData)
            {
                if (result.Sum(x => x.Value) >= 4)
                    break;

                result[actorNr] = Math.Min(result[actorNr] + 1, heroParamCount);
            }

            // Add second AI units
            foreach ((int actorNr, int heroParamCount) in actorData)
            {
                if (result.Sum(x => x.Value) >= 4)
                    break;

                result[actorNr] = Math.Min(result[actorNr] + 1, heroParamCount);
            }

            return result;
        }

        /// <summary>
        /// Helper method to raise events.
        /// </summary>
        /// <param name="eventCode">The event code to raise.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="target">The actor to target -- if null, all actors will be targeted.</param>
        private void RaiseEvent(Event eventCode, object eventData, int? target = null)
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

        private int GenerateRoomId() => this.random.Next(100_0000, 999_9999);

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
