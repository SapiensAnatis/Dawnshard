using System;
using System.Collections.Generic;
using DragaliaAPI.Photon.Plugin.Shared.Helpers;

namespace DragaliaAPI.Photon.Plugin.Shared
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

        public bool EnableDiscordIntegration { get; }

        public Uri DiscordUrl { get; }

        public string DiscordBearerToken { get; }

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

            this.BearerToken = config[nameof(this.BearerToken)];
            this.ReplayTimeoutSeconds = config.GetInt(nameof(this.ReplayTimeoutSeconds));

            this.RandomMatchingStartDelayMs = config.GetInt(
                nameof(this.RandomMatchingStartDelayMs)
            );

            if (
                config.TryGetBool(nameof(this.EnableSecondaryServer), out bool enableMultiServer)
                && enableMultiServer
            )
            {
                this.EnableSecondaryServer = true;

                this.SecondaryViewerIdCriterion = config.GetLong(
                    nameof(this.SecondaryViewerIdCriterion)
                );

                this.SecondaryApiServerUrl = config.GetUri(
                    nameof(this.SecondaryApiServerUrl),
                    UriKind.Absolute
                );

                this.SecondaryStateManagerUrl = config.GetUri(
                    nameof(this.SecondaryStateManagerUrl),
                    UriKind.Absolute
                );

                this.SecondaryBearerToken = config[nameof(this.SecondaryBearerToken)];
            }

            if (
                config.TryGetBool(nameof(this.EnableDiscordIntegration), out bool enableDiscord)
                && enableDiscord
            )
            {
                this.EnableDiscordIntegration = true;
                this.DiscordUrl = config.GetUri(nameof(this.DiscordUrl), UriKind.Absolute);
                this.DiscordBearerToken = config[nameof(this.DiscordBearerToken)];
            }
        }
    }
}
