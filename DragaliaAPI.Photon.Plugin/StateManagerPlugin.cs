using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DragaliaAPI.Photon.Plugin.Constants;
using DragaliaAPI.Photon.Plugin.Helpers;
using DragaliaAPI.Photon.Plugin.Models;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Photon.Shared.Requests;
using Newtonsoft.Json;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
{
    /// <summary>
    /// Sub-plugin for communicating with the PhotonStateManager API for the purposes
    /// of keeping the list of rooms up to date.
    /// </summary>
    public class StateManagerPlugin : PluginBase
    {
        private static readonly Uri GameCreateEndpoint = new Uri(
            "Event/GameCreate",
            UriKind.Relative
        );

        private static readonly Uri GameCloseEndpoint = new Uri(
            "Event/GameClose",
            UriKind.Relative
        );

        private static readonly Uri GameJoinEndpoint = new Uri("Event/GameJoin", UriKind.Relative);

        private static readonly Uri GameLeaveEndpoint = new Uri(
            "Event/GameLeave",
            UriKind.Relative
        );

        private static readonly Uri EntryConditionsEndpoint = new Uri(
            "Event/EntryConditions",
            UriKind.Relative
        );

        private static readonly Uri MatchingTypeEndpoint = new Uri(
            "Event/MatchingType",
            UriKind.Relative
        );

        private static readonly Uri RoomIdEndpoint = new Uri("Event/RoomId", UriKind.Relative);

        private static readonly Uri VisibleEndpoint = new Uri("Event/Visible", UriKind.Relative);

        private static readonly JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
            };

        private readonly PluginStateService pluginStateService;
        private readonly PluginConfiguration configuration;

        private IPluginLogger logger;
        private bool roomHidden;

        public override string Name => nameof(StateManagerPlugin);

        private Uri StateManagerUrl =>
            this.pluginStateService.IsUseSecondaryServer
                ? this.configuration.SecondaryStateManagerUrl
                : this.configuration.StateManagerUrl;

        private string BearerToken =>
            this.pluginStateService.IsUseSecondaryServer
                ? this.configuration.SecondaryBearerToken
                : this.configuration.BearerToken;

        public StateManagerPlugin(
            PluginStateService pluginStateService,
            PluginConfiguration configuration
        )
        {
            this.pluginStateService = pluginStateService;
            this.configuration = configuration;
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
            // https://doc.photonengine.com/server/current/plugins/plugins-faq#how_to_get_the_actor_number_in_plugin_callbacks_
            int actorNr = 1;

            HttpRequest request = this.CreateRequest(
                GameCreateEndpoint,
                new GameCreateRequest()
                {
                    Game = DtoHelpers.CreateGame(
                        this.PluginHost.GameId,
                        info.Request.GameProperties
                    ),
                    Player = DtoHelpers.CreatePlayer(actorNr, info.Request.ActorProperties)
                }
            );

            this.PluginHost.HttpRequest(request, info);
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            HttpRequest request = this.CreateRequest(
                GameJoinEndpoint,
                new GameModifyRequest
                {
                    GameName = this.PluginHost.GameId,
                    Player = DtoHelpers.CreatePlayer(info.ActorNr, info.Request.ActorProperties)
                }
            );

            this.PluginHost.HttpRequest(request, info);
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            if (info.ActorNr == -1)
            {
                // Actor was never really in the room, server disconnect, etc.
                return;
            }

            IActor actor = this.PluginHost.GameActors.FirstOrDefault(
                x => x.ActorNr == info.ActorNr
            );

            if (actor is null)
            {
                this.logger.WarnFormat(
                    "OnLeave: could not find actor {0} -- GameLeave request aborted",
                    info.ActorNr
                );
                return;
            }

            HttpRequest request = this.CreateRequest(
                GameLeaveEndpoint,
                new GameModifyRequest
                {
                    GameName = this.PluginHost.GameId,
                    Player = DtoHelpers.CreatePlayer(info.ActorNr, actor.Properties.GetProperties())
                }
            );

            this.PluginHost.HttpRequest(request, info);
        }

        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            HttpRequest request = this.CreateRequest(
                GameCloseEndpoint,
                new GameModifyRequest { GameName = this.PluginHost.GameId, Player = null }
            );

            request.Async = false;

            this.PluginHost.HttpRequest(request, info);
        }

        public override void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            // Need to override to avoid calling info.Continue() twice
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            if (info.Request.Properties.ContainsKey(GamePropertyKeys.EntryConditions))
                this.OnSetEntryConditions(info);

            if (info.Request.Properties.ContainsKey(GamePropertyKeys.MatchingType))
                this.OnSetMatchingType(info);

            if (info.Request.Properties.ContainsKey(ActorPropertyKeys.GoToIngameState))
                this.OnSetGoToIngameState(info);
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            if (info.Request.EvCode == (int)Event.GameSucceed && info.ActorNr == 1)
            {
                this.OnGameSucceed(info);
                this.OnSetRoomId(info);
            }
        }

        private void OnGameSucceed(IRaiseEventCallInfo info)
        {
            HttpRequest request = this.CreateRequest(
                VisibleEndpoint,
                new GameModifyVisibleRequest
                {
                    NewVisibility = true,
                    GameName = this.PluginHost.GameId,
                    Player = null
                }
            );

            this.PluginHost.HttpRequest(request, info);

            this.roomHidden = false;
        }

        private void OnSetMatchingType(ISetPropertiesCallInfo info)
        {
            MatchingTypes newType = (MatchingTypes)
                info.Request.Properties.GetInt(GamePropertyKeys.MatchingType);

            HttpRequest request = this.CreateRequest(
                MatchingTypeEndpoint,
                new GameModifyMatchingTypeRequest()
                {
                    GameName = this.PluginHost.GameId,
                    NewMatchingType = newType,
                    Player = null
                }
            );

            this.PluginHost.HttpRequest(request, info);
        }

        private void OnSetEntryConditions(ISetPropertiesCallInfo info)
        {
            EntryConditions newEntryConditions = DtoHelpers.CreateEntryConditions(
                info.Request.Properties
            );

            if (newEntryConditions is null)
                return;

            HttpRequest request = this.CreateRequest(
                EntryConditionsEndpoint,
                new GameModifyConditionsRequest()
                {
                    GameName = this.PluginHost.GameId,
                    NewEntryConditions = newEntryConditions,
                    Player = null
                }
            );

            this.PluginHost.HttpRequest(request, info);
        }

        private void OnSetGoToIngameState(ISetPropertiesCallInfo info)
        {
            IEnumerable<int> goToIngameState = this.PluginHost.GameActors.Select(
                x => x.Properties.GetIntOrDefault(ActorPropertyKeys.GoToIngameState)
            );

            bool shouldHideRoom = goToIngameState.All(x => x > 1) && !this.roomHidden;

            if (!shouldHideRoom)
                return;

            HttpRequest request = this.CreateRequest(
                VisibleEndpoint,
                new GameModifyVisibleRequest
                {
                    NewVisibility = false,
                    GameName = this.PluginHost.GameId,
                    Player = null
                }
            );

            this.PluginHost.HttpRequest(request, info);

            this.roomHidden = true;
        }

        private void OnSetRoomId(IRaiseEventCallInfo info)
        {
            int roomId = (int)this.PluginHost.GameProperties[GamePropertyKeys.RoomId];

            HttpRequest request = this.CreateRequest(
                RoomIdEndpoint,
                new GameModifyRoomIdRequest
                {
                    NewRoomId = roomId,
                    GameName = this.PluginHost.GameId,
                    Player = null
                }
            );

            this.PluginHost.HttpRequest(request, info);
        }

        private HttpRequest CreateRequest(Uri endpoint, object request)
        {
            Uri requestUri = new Uri(this.StateManagerUrl, endpoint);

            string json = JsonConvert.SerializeObject(request, JsonSerializerSettings);
            MemoryStream requestBody = new MemoryStream(Encoding.UTF8.GetBytes(json));

            return new HttpRequest()
            {
                Url = requestUri.AbsoluteUri,
                ContentType = "application/json",
                Callback = this.LogIfFailedCallback,
                Async = true,
                Accept = "application/json",
                DataStream = requestBody,
                Method = "POST",
                CustomHeaders = new Dictionary<string, string>()
                {
                    { "Authorization", $"Bearer {this.BearerToken}" },
                    { "RoomName", this.PluginHost.GameId },
                    {
                        "RoomId",
                        this.PluginHost.GameProperties.GetInt(GamePropertyKeys.RoomId).ToString()
                    }
                }
            };
        }

        private void LogIfFailedCallback(IHttpResponse httpResponse, object userState)
        {
            if (httpResponse.Status != HttpRequestQueueResult.Success)
            {
                this.ReportError(
                    $"Request to {httpResponse.Request.Url} failed with Photon status {httpResponse.Status} and HTTP status {httpResponse.HttpCode} ({httpResponse.Reason})"
                );
            }
        }

        private void ReportError(string msg)
        {
            this.PluginHost.LogError(msg);
            this.PluginHost.BroadcastErrorInfoEvent(msg);
        }
    }
}
