using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DragaliaAPI.Photon.Plugin.Shared;
using DragaliaAPI.Photon.Plugin.Shared.Constants;
using DragaliaAPI.Photon.Plugin.Shared.Helpers;
using DragaliaAPI.Photon.Shared.Requests;
using Newtonsoft.Json;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Plugins.Discord
{
    public class DiscordPlugin : PluginBase
    {
        private static readonly Uri GameCreateEndpoint = new Uri("room_opened", UriKind.Relative);
        private static readonly Uri GameUpdateEndpoint = new Uri("room_updated", UriKind.Relative);
        private static readonly Uri GameCloseEndpoint = new Uri("room_closed", UriKind.Relative);

        private readonly PluginConfiguration configuration;
        private IPluginLogger logger;

        public DiscordPlugin(PluginConfiguration configuration)
        {
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

            string json = JsonConvert.SerializeObject(request);
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
                    { "Authorization", $"Bearer {this.configuration.DiscordBearerToken}" },
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
