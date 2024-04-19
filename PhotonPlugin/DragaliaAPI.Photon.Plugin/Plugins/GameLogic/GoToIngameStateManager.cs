using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using DragaliaAPI.Photon.Plugin.Plugins.GameLogic.Events;
using DragaliaAPI.Photon.Plugin.Shared;
using DragaliaAPI.Photon.Plugin.Shared.Constants;
using DragaliaAPI.Photon.Plugin.Shared.Helpers;
using DragaliaAPI.Photon.Shared.Models;
using MessagePack;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Plugins.GameLogic
{
    internal sealed class GoToIngameStateManager
    {
        private static readonly MessagePackSerializerOptions MessagePackOptions =
            MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block);

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly IPluginHost pluginHost;
        private readonly PluginStateService pluginStateService;
        private readonly PluginConfiguration pluginConfiguration;
        private readonly IPluginLogger logger;
        private readonly Dictionary<int, HeroParamState> heroParamStorage;

        public int MinGoToIngameState { get; private set; }

        public GoToIngameStateManager(
            IPluginHost pluginHost,
            PluginStateService pluginStateService,
            PluginConfiguration pluginConfiguration
        )
        {
            this.pluginHost = pluginHost;
            this.pluginStateService = pluginStateService;
            this.pluginConfiguration = pluginConfiguration;

            this.heroParamStorage = new Dictionary<int, HeroParamState>(4);
            this.logger = this.pluginHost.CreateLogger(nameof(GoToIngameStateManager));
        }

        public void OnSetGoToIngameState(IBeforeSetPropertiesCallInfo info, int value)
        {
            int minValue = this
                .pluginHost.GameActors.Where(x => x.ActorNr != info.ActorNr) // Exclude the value which we are in the BeforeSet handler for
                .Select(x => x.Properties.GetIntOrDefault(ActorPropertyKeys.GoToIngameState))
                .Append(value)
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
                this.MinGoToIngameState
            );
#endif

            if (minValue > this.MinGoToIngameState)
            {
                this.MinGoToIngameState = minValue;
                this.OnMinStateChange(info);
            }
            else if (value == 1 && info.ActorNr == 1)
            {
                this.MinGoToIngameState = 1;
                this.OnMinStateChange(info);
            }
            else if (value == 0 && this.pluginHost.GetIsSoloPlay())
            {
                this.SetGoToIngameInfo();
                this.pluginHost.RaiseEvent(Event.StartQuest, new Dictionary<string, object>());
            }
        }

        public void OnActorLeave(ILeaveGameCallInfo info)
        {
            if (this.MinGoToIngameState <= 0)
                return;

            this.heroParamStorage.Remove(info.ActorNr);

            int newMinGoToIngameState = this
                .pluginHost.GameActors.Where(x => x.ActorNr != info.ActorNr)
                .Select(x => x.Properties.GetIntOrDefault(ActorPropertyKeys.GoToIngameState))
                .DefaultIfEmpty()
                .Min();

            if (this.MinGoToIngameState == newMinGoToIngameState)
                return;

            this.MinGoToIngameState = newMinGoToIngameState;
            this.OnMinStateChange(info);
        }

        /// <summary>
        /// Handler to perform various operations when the minimum GoToIngameState of a room is increased.
        /// </summary>
        /// <remarks>
        /// Represents various stages of loading into a quest, during which events/properties need to be raised/set.
        /// </remarks>
        /// <param name="info">Call info.</param>
        private void OnMinStateChange(ICallInfo info)
        {
            this.logger.InfoFormat(
                "OnSetGoToIngameState: updating with value {0}",
                this.MinGoToIngameState
            );

            switch (this.MinGoToIngameState)
            {
                case 1:
                    this.SetGoToIngameInfo();
                    break;
                case 2:
                    this.RequestHeroParam(info);
                    break;
                case 3:
                    this.RaisePartyEvent();
                    break;
                case 4:
                    this.RaiseCharacterDataEvent();
                    break;
            }
        }

        /// <summary>
        /// Sets the GoToIngameInfo room property by gathering data from connected actors.
        /// </summary>
        public void SetGoToIngameInfo()
        {
            IEnumerable<ActorData> actorData = this.pluginHost.GameActors.Select(
                x => new ActorData() { ActorId = x.ActorNr, ViewerId = (ulong)x.GetViewerId() }
            );

            GoToIngameState data = new GoToIngameState()
            {
                elements = actorData,
                brInitData = null
            };

            byte[] msgpack = MessagePackSerializer.Serialize(data, MessagePackOptions);

            this.pluginHost.SetProperties(
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
            IEnumerable<ActorInfo> heroParamRequest = this.pluginHost.GameActors.Select(
                x => new ActorInfo()
                {
                    ActorNr = x.ActorNr,
                    ViewerId = x.GetViewerId(),
                    PartySlots = x.GetPartySlots()
                }
            );

            Uri baseUri = this.pluginStateService.IsUseSecondaryServer
                ? this.pluginConfiguration.SecondaryApiServerUrl
                : this.pluginConfiguration.ApiServerUrl;

            Uri requestUri = new Uri(baseUri, "heroparam/batch");

            this.logger.DebugFormat("RequestHeroParam - {0}", requestUri.AbsoluteUri);

            HttpRequest req = new HttpRequest()
            {
                Url = requestUri.AbsoluteUri,
                ContentType = "application/json",
                Callback = this.HeroParamRequestCallback,
                Async = false,
                Accept = "application/json",
                DataStream = new MemoryStream(
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(heroParamRequest, JsonOptions))
                ),
                Method = "POST",
            };

            this.pluginHost.HttpRequest(req, info);
        }

        /// <summary>
        /// HTTP request callback for the HeroParam request sent in <see cref="RequestHeroParam(ICallInfo)"/>.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="userState">The arguments passed from the calling function.</param>
        private void HeroParamRequestCallback(IHttpResponse response, object userState)
        {
            this.pluginHost.LogIfFailedCallback(response, userState);

            if (response.Status != HttpRequestQueueResult.Success)
                return;

            List<HeroParamData> responseObject = JsonSerializer.Deserialize<List<HeroParamData>>(
                response.ResponseText,
                JsonOptions
            );

#if DEBUG
            this.logger.DebugFormat("Response text: {0}", response.ResponseText);
            this.logger.DebugFormat(
                "Deserialized response: {0}",
                JsonSerializer.Serialize(responseObject)
            );
#endif

            foreach (HeroParamData data in responseObject)
                this.heroParamStorage[data.ActorNr] = new HeroParamState(data);
        }

        /// <summary>
        /// Raises the <see cref="Event.Party"/> event.
        /// </summary>
        private void RaisePartyEvent()
        {
            Dictionary<int, int> memberCountTable = this.GetMemberCountTable();

            foreach (HeroParamState state in this.heroParamStorage.Values)
                state.UsedMemberCount = memberCountTable[state.ActorNr];

            int questId = this.pluginHost.GetQuestId();
            int rankingType = QuestHelper.GetIsRanked(questId) ? 1 : 0;

            PartyEvent evt = new PartyEvent()
            {
                MemberCountTable = memberCountTable,
                ReBattleCount = this.pluginConfiguration.ReplayTimeoutSeconds,
                RankingType = rankingType,
            };

            this.pluginHost.RaiseEvent(Event.Party, evt);
        }

        /// <summary>
        /// Gets how many units each actor should control based on the number of ingame players.
        /// </summary>
        /// <returns>A dictionary of { [actorNr] = unitCount }</returns>
        private Dictionary<int, int> GetMemberCountTable()
        {
            bool isRaid =
                this.pluginHost.GameProperties.TryGetInt(GamePropertyKeys.QuestId, out int questId)
                && QuestHelper.GetIsRaid(questId);

            if (isRaid || this.pluginHost.GetIsSoloPlay())
            {
                // Use all available units
                return this.heroParamStorage.ToDictionary(
                    x => x.Value.ActorNr,
                    x => x.Value.HeroParamCount
                );
            }

            return MemberCountHelper.BuildMemberCountTable(
                this.heroParamStorage.Select(x => new ValueTuple<int, int>(
                        x.Value.ActorNr,
                        x.Value.HeroParamCount
                    ))
                    .ToList()
            );
        }

        /// <summary>
        /// Raise <see cref="Event.CharacterData"/> using cached <see cref="HeroParamData"/>.
        /// </summary>
        private void RaiseCharacterDataEvent()
        {
            foreach (KeyValuePair<int, HeroParamState> kvp in this.heroParamStorage)
            {
                int actorNr = kvp.Key;
                HeroParamState state = kvp.Value;

                foreach (List<HeroParam> heroParams in state.Data.HeroParamLists)
                {
                    CharacterData evt = new CharacterData()
                    {
                        playerId = actorNr,
                        heroParamExs = heroParams
                            .Select(x => new HeroParamExData()
                            {
                                limitOverCount = x.ExAbilityLv,
                                sequenceNumber = x.Position
                            })
                            .ToArray(),
                        heroParams = heroParams.Take(state.UsedMemberCount).ToArray()
                    };

                    this.pluginHost.RaiseEvent(Event.CharacterData, evt);
                }
            }
        }
    }
}
