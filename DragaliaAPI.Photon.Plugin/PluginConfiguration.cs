using System;
using System.Collections.Generic;
using System.Text;
using DragaliaAPI.Photon.Plugin.Helpers;

namespace DragaliaAPI.Photon.Plugin
{
    /// <summary>
    /// Shared instance for immutable configuration to be shared between sub-plugins.
    /// </summary>
    public class PluginConfiguration
    {
        public Uri StateManagerUrl { get; }

        public Uri ApiServerUrl { get; }

        public Uri DungeonRecordMultiEndpoint { get; }

        public Uri TimeAttackEndpoint { get; }

        public int ReplayTimeoutSeconds { get; }

        public string BearerToken { get; }

        public bool EnableSecondaryServer { get; }

        public long SecondaryViewerIdCriterion { get; }

        public Uri SecondaryApiServerUrl { get; }

        public Uri SecondaryStateManagerUrl { get; }

        public string SecondaryBearerToken { get; }

        public int RandomMatchingStartDelayMs { get; }

        public PluginConfiguration(IDictionary<string, string> config)
        {
            this.ApiServerUrl = config.GetUri(nameof(this.ApiServerUrl), UriKind.Absolute);
            this.StateManagerUrl = config.GetUri(nameof(this.StateManagerUrl), UriKind.Absolute);

            this.DungeonRecordMultiEndpoint = config.GetUri(
                nameof(this.DungeonRecordMultiEndpoint),
                UriKind.Relative
            );

            this.TimeAttackEndpoint = config.GetUri(
                nameof(this.TimeAttackEndpoint),
                UriKind.Relative
            );

            this.BearerToken = config[nameof(BearerToken)];
            this.ReplayTimeoutSeconds = config.GetInt(nameof(ReplayTimeoutSeconds));

            if (
                !config.TryGetValue(
                    nameof(EnableSecondaryServer),
                    out string enableMultiServerString
                )
                || !bool.TryParse(enableMultiServerString, out bool enableMultiServer)
                || !enableMultiServer
            )
            {
                return;
            }

            this.EnableSecondaryServer = enableMultiServer;

            this.SecondaryViewerIdCriterion = config.GetLong(nameof(SecondaryViewerIdCriterion));

            this.SecondaryApiServerUrl = config.GetUri(
                nameof(SecondaryApiServerUrl),
                UriKind.Absolute
            );

            this.SecondaryStateManagerUrl = config.GetUri(
                nameof(SecondaryStateManagerUrl),
                UriKind.Absolute
            );

            this.SecondaryBearerToken = config[nameof(SecondaryBearerToken)];

            this.RandomMatchingStartDelayMs = config.GetInt(nameof(RandomMatchingStartDelayMs));
        }
    }
}
