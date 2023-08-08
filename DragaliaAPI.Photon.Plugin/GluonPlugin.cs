using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DragaliaAPI.Photon.Plugin.Constants;
using DragaliaAPI.Photon.Plugin.Helpers;
using DragaliaAPI.Photon.Plugin.Models;
using DragaliaAPI.Photon.Plugin.Models.Events;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.Shared.Requests;
using MessagePack;
using Newtonsoft.Json;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
{
    /// <summary>
    /// Main plugin game logic.
    /// </summary>
    public partial class GluonPlugin : PluginBase
    {
        private IPluginLogger logger;
        private PluginConfiguration config;
        private Random rdm;

        private Dictionary<int, ActorState> actorState;
        private RoomState roomState;

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
            this.rdm = new Random();

            this.actorState = new Dictionary<int, ActorState>(4);
            this.roomState = new RoomState();

            return base.SetupInstance(host, config, out errorMsg);
        }

        #region Photon Handlers

        /// <summary>
        /// Photon handler for when a game is created.
        /// </summary>
        /// <param name="info">Event information.</param>
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            info.Request.ActorProperties.InitializeViewerId();

            int roomId = this.GenerateRoomId();
            info.Request.GameProperties.Add(GamePropertyKeys.RoomId, roomId);

#if DEBUG
            this.logger.DebugFormat(
                "Room properties: {0}",
                JsonConvert.SerializeObject(info.Request.GameProperties)
            );
#endif

            // https://doc.photonengine.com/server/current/plugins/plugins-faq#how_to_get_the_actor_number_in_plugin_callbacks_
            // This is only invalid if the room is recreated from an inactive state, which Dragalia doesn't do (hopefully!)
            const int actorNr = 1;
            this.actorState[actorNr] = new ActorState();

            info.Continue();

            this.logger.InfoFormat(
                "Viewer ID {0} created room {1} with room ID {2}",
                info.Request.ActorProperties.GetInt(ActorPropertyKeys.ViewerId),
                this.PluginHost.GameId,
                roomId
            );

            this.PostStateManagerRequest(
                GameCreateEndpoint,
                new GameCreateRequest()
                {
                    Game = DtoHelpers.CreateGame(
                        this.PluginHost.GameId,
                        info.Request.GameProperties
                    ),
                    Player = DtoHelpers.CreatePlayer(actorNr, info.Request.ActorProperties)
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
            this.actorState[info.ActorNr] = new ActorState();

            this.logger.InfoFormat(
                "Viewer ID {0} joined game {1}",
                info.Request.ActorProperties.GetInt(ActorPropertyKeys.ViewerId),
                this.PluginHost.GameId
            );

            this.PostStateManagerRequest(
                GameJoinEndpoint,
                new GameModifyRequest
                {
                    GameName = this.PluginHost.GameId,
                    Player = DtoHelpers.CreatePlayer(info.ActorNr, info.Request.ActorProperties)
                },
                info
            );

            info.Continue();
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

#if DEBUG
            if (!actor.Properties.TryGetValue("DeactivationTime", out object deactivationTime))
                deactivationTime = "null";

            this.logger.DebugFormat(
                "Leave info -- Actor: {0}, Details: {1}, IsInactive {2}, DeactivationTime: {3}",
                info.ActorNr,
                info.Details,
                info.IsInactive,
                deactivationTime
            );
#endif

            if (info.ActorNr == 1)
            {
                this.RaiseEvent(
                    Event.Codes.RoomBroken,
                    new RoomBroken() { Reason = RoomBroken.RoomBrokenType.HostDisconnected }
                );

                this.SetRoomVisibility(info, false);
            }

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

            if (
                actor.TryGetViewerId(out int viewerId)
                && this.actorState.TryGetValue(info.ActorNr, out ActorState actorState)
                && !actorState.RemovedFromRedis
            )
            {
                this.logger.InfoFormat(
                    "Viewer ID {0} left game {1}",
                    viewerId,
                    this.PluginHost.GameId
                );

                this.PostStateManagerRequest(
                    GameLeaveEndpoint,
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
                actorState.RemovedFromRedis = true;
            }
        }

        /// <summary>
        /// Photon handler for when a game is closed.
        /// </summary>
        /// <param name="info">Event information.</param>
        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            this.PostStateManagerRequest(
                GameCloseEndpoint,
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
            info.Continue();

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
                case Event.Codes.Ready:
                    this.OnActorReady(info);
                    break;
                case Event.Codes.ClearQuestRequest:
                    this.OnClearQuestRequest(info);
                    break;
                case Event.Codes.GameSucceed:
                    this.OnGameSucceed(info);
                    break;
                case Event.Codes.FailQuestRequest:
                    this.OnFailQuestRequest(info);
                    break;
                case Event.Codes.Dead:
                    this.actorState[info.ActorNr].Dead = true;
                    break;
                default:
                    break;
            }
        }

        private void OnFailQuestRequest(IRaiseEventCallInfo info)
        {
            this.roomState = new RoomState();

            FailQuestRequest request = info.DeserializeEvent<FailQuestRequest>();

            this.logger.DebugFormat(
                "Received FailQuestRequest with FailType {0}",
                request.FailType.ToString()
            );

            // I assumed this would need to be POSTed to /dungeon/fail, but the event doesn't contain
            // a request body with dungeon_key... so the API server couldn't really do anything.
            FailQuestResponse response = new FailQuestResponse()
            {
                ResultType =
                    request.FailType == FailQuestRequest.FailTypes.Timeup
                        ? FailQuestResponse.ResultTypes.Timeup
                        : FailQuestResponse.ResultTypes.Clear
            };

            this.RaiseEvent(Event.Codes.FailQuestResponse, response, info.ActorNr);

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
                        // { GamePropertyKeys.RoomId, -1 } TODO: Show 'play again with the same players?' screen on failed retry after wipe
                    },
                    null,
                    true
                );

                this.SetRoomVisibility(info, true);
            }
        }

        private void OnGameSucceed(IRaiseEventCallInfo info)
        {
            this.logger.InfoFormat("Received GameSucceed from actor {0}", info.ActorNr);

            if (info.ActorNr == 1)
            {
                this.roomState = new RoomState();
                this.RaiseEvent(Event.Codes.GameSucceed, new { });
                this.SetRoomId(info, this.GenerateRoomId());
                this.SetRoomVisibility(info, true);
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

                int minValue = this.PluginHost.GameActors
                    .Where(x => x.ActorNr != info.ActorNr) // Exclude the value which we are in the BeforeSet handler for
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
            }

            if (!info.IsProcessed)
            {
                info.Continue();
            }
        }

        /// <summary>
        /// Photon handler for when an actor sets properties.
        /// </summary>
        /// <param name="info">Event information.</param>
        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            base.OnSetProperties(info);

#if DEBUG
            this.logger.DebugFormat("Actor {0} set properties", info.ActorNr);
            this.logger.Debug(JsonConvert.SerializeObject(info.Request.Properties));
#endif

            if (info.Request.Properties.ContainsKey(GamePropertyKeys.EntryConditions))
                this.OnSetEntryConditions(info);

            if (info.Request.Properties.ContainsKey(GamePropertyKeys.MatchingType))
                this.OnSetMatchingType(info);
        }

        #endregion

        #region Game Logic

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
                this.logger.Info("All clients were ready, raising StartQuest");

                this.RaiseEvent(Event.Codes.StartQuest, new Dictionary<string, string> { });

                this.roomState.StartActorCount = this.PluginHost.GameActors.Count;
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

            this.PostStateManagerRequest(
                MatchingTypeEndpoint,
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

            this.PostStateManagerRequest(
                EntryConditionsEndpoint,
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
        private void OnSetGoToIngameState(IBeforeSetPropertiesCallInfo info)
        {
            this.logger.InfoFormat(
                "OnSetGoToIngameState: updating with value {0}",
                this.roomState.MinGoToIngameState
            );

            switch (this.roomState.MinGoToIngameState)
            {
                case 1:
                    this.SetGoToIngameInfo();
                    this.SetRoomVisibility(info, false);
                    break;
                case 2:
                    this.RequestHeroParam(info);
                    break;
                case 3:
                    this.RaisePartyEvent();
                    this.RaiseCharacterDataEvent();
                    break;
                default:
                    break;
            }
        }

        private void RaiseCharacterDataEvent()
        {
            foreach (IActor actor in this.PluginHost.GameActors)
            {
                ActorState actorState = this.actorState[actor.ActorNr];

                foreach (
                    IEnumerable<HeroParam> heroParams in actorState.HeroParamData.HeroParamLists
                )
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
                        heroParams = heroParams.Take(actorState.MemberCount).ToArray()
                    };

                    this.RaiseEvent(Event.Codes.CharacterData, evt);
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
        /// Raises the CharacterData event by making requests to the main API server for party information.
        /// </summary>
        /// <param name="info">Info from <see cref="OnSetProperties(ISetPropertiesCallInfo)"/>.</param>
        private void RequestHeroParam(IBeforeSetPropertiesCallInfo info)
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

            Uri requestUri = new Uri(this.config.ApiServerUrl, $"heroparam/batch");

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
        /// HTTP request callback for the HeroParam request sent in <see cref="RequestHeroParam(ISetPropertiesCallInfo)"/>.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="userState">The arguments passed from the calling function.</param>
        private void HeroParamRequestCallback(IHttpResponse response, object userState)
        {
            this.LogIfFailedCallback(response, userState);

            List<HeroParamData> responseObject = JsonConvert.DeserializeObject<List<HeroParamData>>(
                response.ResponseText
            );

            foreach (HeroParamData data in responseObject)
                this.actorState[data.ActorNr].HeroParamData = data;
        }

        /// <summary>
        /// Raises the Party event containing information about how many characters each player owns.
        /// </summary>
        /// <param name="info">Info from <see cref="OnSetProperties(ISetPropertiesCallInfo)"/>.</param>
        private void RaisePartyEvent()
        {
            Dictionary<int, int> memberCountTable = this.GetMemberCountTable();

            foreach (IActor actor in this.PluginHost.GameActors)
                this.actorState[actor.ActorNr].MemberCount = memberCountTable[actor.ActorNr];

            PartyEvent evt = new PartyEvent()
            {
                MemberCountTable = memberCountTable,
                ReBattleCount = this.config.ReplayTimeoutSeconds
            };

            this.RaiseEvent(Event.Codes.Party, evt);
        }

        /// <summary>
        /// Update the RoomId of a room and send a request to the Redis API to update it there.
        /// </summary>
        /// <param name="info">Call info from handler.</param>
        /// <param name="roomId">The new room ID.</param>
        private void SetRoomId(ICallInfo info, int roomId)
        {
            this.logger.DebugFormat("Setting room ID to {0}", roomId);

            this.PluginHost.SetProperties(
                0,
                new Hashtable { { GamePropertyKeys.RoomId, roomId } },
                null,
                broadcast: true
            );

            this.PostStateManagerRequest(
                RoomIdEndpoint,
                new GameModifyRoomIdRequest
                {
                    NewRoomId = roomId,
                    GameName = this.PluginHost.GameId,
                    Player = null
                },
                info,
                callAsync: true
            );
        }

        /// <summary>
        /// Make a call to the Redis API to set the room's visibility.
        /// </summary>
        /// <param name="info">Call info from handler.</param>
        /// <param name="visible">The new visibility.</param>
        private void SetRoomVisibility(ICallInfo info, bool visible)
        {
            this.logger.DebugFormat("Setting room visibility to {0}", visible);

            this.PostStateManagerRequest(
                VisibleEndpoint,
                new GameModifyVisibleRequest
                {
                    NewVisibility = visible,
                    GameName = this.PluginHost.GameId,
                    Player = null
                },
                info,
                callAsync: true
            );
        }

        private void OnClearQuestRequest(IRaiseEventCallInfo info)
        {
            // These properties must be set for the client to successfully rejoin the room.
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

            this.actorState[info.ActorNr] = new ActorState();

            ClearQuestRequest evt = info.DeserializeEvent<ClearQuestRequest>();

            this.PostApiRequest(
                this.config.DungeonRecordMultiEndpoint,
                evt.RecordMultiRequest,
                info,
                ClearQuestRequestCallback,
                callAsync: false
            );
        }

        private void ClearQuestRequestCallback(IHttpResponse response, object userState)
        {
            this.LogIfFailedCallback(response, userState);

            HttpRequestUserState typedUserState = (HttpRequestUserState)userState;

            this.RaiseEvent(
                Event.Codes.ClearQuestResponse,
                new ClearQuestResponse() { RecordMultiResponse = response.ResponseData },
                typedUserState.RequestActorNr
            );
        }

        /// <summary>
        /// Gets how many units an actor should control based on the number of ingame players.
        /// </summary>
        /// <param name="actorNr">The actor number.</param>
        /// <returns>The number of units they own.</returns>
        private Dictionary<int, int> GetMemberCountTable()
        {
            if (
                this.PluginHost.GameProperties.TryGetInt(GamePropertyKeys.QuestId, out int questId)
                && QuestHelper.GetDungeonType(questId) == DungeonTypes.Raid
            )
            {
                // Everyone uses all of their units in a raid
                return this.PluginHost.GameActors.ToDictionary(
                    x => x.ActorNr,
                    x => this.actorState[x.ActorNr].HeroParamCount
                );
            }

            return BuildMemberCountTable(
                this.PluginHost.GameActors.Select(
                    x =>
                        new ValueTuple<int, int>(
                            x.ActorNr,
                            this.actorState[x.ActorNr].HeroParamCount
                        )
                )
            );
            ;
        }

        public static Dictionary<int, int> BuildMemberCountTable(
            IEnumerable<(int ActorNr, int HeroParamCount)> actorData
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

        #endregion
    }
}
