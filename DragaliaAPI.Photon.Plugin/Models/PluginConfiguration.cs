using System;
using System.Collections.Generic;
using System.Text;
using DragaliaAPI.Photon.Plugin.Helpers;

namespace DragaliaAPI.Photon.Plugin.Models
{
    internal class PluginConfiguration
    {
        public Uri StateManagerUrl { get; }

        public Uri ApiServerUrl { get; }

        public Uri DungeonRecordMultiEndpoint { get; }

        public int ReplayTimeoutSeconds { get; }

        public string BearerToken { get; }

        public PluginConfiguration(IDictionary<string, string> config)
        {
            this.ApiServerUrl = config.GetUri(nameof(this.ApiServerUrl), UriKind.Absolute);
            this.StateManagerUrl = config.GetUri(nameof(this.StateManagerUrl), UriKind.Absolute);

            this.DungeonRecordMultiEndpoint = config.GetUri(
                nameof(this.DungeonRecordMultiEndpoint),
                UriKind.Relative
            );

            this.BearerToken = config[nameof(BearerToken)];
            this.ReplayTimeoutSeconds = config.GetInt(nameof(ReplayTimeoutSeconds));
        }
    }
}
