using System;
using System.Collections.Generic;
using System.Text;
using DragaliaAPI.Photon.Plugin.Helpers;

namespace DragaliaAPI.Photon.Plugin.Models
{
    internal class PluginConfiguration
    {
        public Uri BaseUrl { get; set; }

        public Uri GameCreateEndpoint { get; set; }

        public Uri GameCloseEndpoint { get; set; }

        public Uri GameJoinEndpoint { get; set; }

        public Uri GameLeaveEndpoint { get; set; }

        public Uri EntryConditionsEndpoint { get; set; }

        public Uri MatchingTypeEndpoint { get; set; }

        public string BearerToken { get; set; }

        public PluginConfiguration(IDictionary<string, string> config)
        {
            this.BaseUrl = config.GetUri(nameof(this.BaseUrl), UriKind.Absolute);
            this.GameCreateEndpoint = config.GetUri(
                nameof(this.GameCreateEndpoint),
                UriKind.Relative
            );
            this.GameJoinEndpoint = config.GetUri(nameof(this.GameJoinEndpoint), UriKind.Relative);
            this.GameCloseEndpoint = config.GetUri(
                nameof(this.GameCloseEndpoint),
                UriKind.Relative
            );
            this.GameLeaveEndpoint = config.GetUri(
                nameof(this.GameLeaveEndpoint),
                UriKind.Relative
            );
            this.EntryConditionsEndpoint = config.GetUri(
                nameof(this.EntryConditionsEndpoint),
                UriKind.Relative
            );
            this.MatchingTypeEndpoint = config.GetUri(
                nameof(this.MatchingTypeEndpoint),
                UriKind.Relative
            );

            this.BearerToken = config[nameof(BearerToken)];
        }
    }
}
