using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using DragaliaAPI.Photon.Plugin.Shared;
using DragaliaAPI.Photon.Plugin.Shared.Constants;
using DragaliaAPI.Photon.Plugin.Shared.Helpers;
using DragaliaAPI.Photon.Shared.Requests;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Plugins.Discord
{
    public class DiscordPlugin : PluginBase
    {
        private static readonly Uri GameCreateEndpoint = new Uri("room_opened", UriKind.Relative);
        private static readonly Uri GameUpdateEndpoint = new Uri("room_updated", UriKind.Relative);
        private static readonly Uri GameCloseEndpoint = new Uri("room_closed", UriKind.Relative);

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly PluginConfiguration configuration;

        public DiscordPlugin(PluginConfiguration configuration)
        {
            this.configuration = configuration;
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

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            if (!info.Request.Properties.ContainsKey(GamePropertyKeys.EntryConditions))
                return;

            HttpRequest request = this.CreateRequest(
                GameUpdateEndpoint,
                new GameModifyConditionsRequest()
                {
                    GameName = this.PluginHost.GameId,
                    NewEntryConditions = DtoHelpers.CreateEntryConditions(info.Request.Properties),
                    Player = null
                }
            );

            this.PluginHost.HttpRequest(request, info);
        }

        public override void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            HttpRequest request = this.CreateRequest(
                GameCloseEndpoint,
                new GameModifyRequest { GameName = this.PluginHost.GameId, Player = null }
            );

            request.Async = false;

            this.PluginHost.HttpRequest(request, info);
        }

        private HttpRequest CreateRequest(Uri endpoint, object request)
        {
            Uri requestUri = new Uri(this.configuration.DiscordUrl, endpoint);

            string json = JsonSerializer.Serialize(request, JsonOptions);
            MemoryStream requestBody = new MemoryStream(Encoding.UTF8.GetBytes(json));

            return new HttpRequest()
            {
                Url = requestUri.AbsoluteUri,
                ContentType = "application/json",
                Callback = this.PluginHost.LogIfFailedCallback,
                Async = true,
                Accept = "application/json",
                DataStream = requestBody,
                Method = "POST",
                CustomHeaders = new Dictionary<string, string>()
                {
                    { "Authorization", $"Bearer {this.configuration.DiscordBearerToken}" },
                    { "RoomName", this.PluginHost.GameId },
                    {
                        "RoomId",
                        this.PluginHost.GameProperties.GetInt(GamePropertyKeys.RoomId).ToString()
                    }
                }
            };
        }
    }
}
