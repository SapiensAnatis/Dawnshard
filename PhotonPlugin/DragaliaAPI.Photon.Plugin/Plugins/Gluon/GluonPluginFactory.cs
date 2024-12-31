using System;
using System.Collections.Generic;
using DragaliaAPI.Photon.Plugin.Plugins.Discord;
using DragaliaAPI.Photon.Plugin.Plugins.GameLogic;
using DragaliaAPI.Photon.Plugin.Plugins.StateManager;
using DragaliaAPI.Photon.Plugin.Shared;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Plugins.Gluon
{
    public class GluonPluginFactory : IPluginFactory
    {
        public IGamePlugin? Create(
            IPluginHost gameHost,
            string pluginName,
            Dictionary<string, string> config,
            out string errorMsg
        )
        {
            Dictionary<string, string> configWithEnv = new(config.Count);

            foreach (KeyValuePair<string, string> kvp in config)
            {
                string value = Environment.GetEnvironmentVariable(kvp.Key) ?? kvp.Value;
                configWithEnv.Add(kvp.Key, value);
            }

            PluginConfiguration configuration = new PluginConfiguration(configWithEnv);
            PluginStateService stateService = new PluginStateService();

            GameLogicPlugin gameLogicPlugin = new GameLogicPlugin(stateService, configuration);

            StateManagerPlugin stateManagerPlugin = new StateManagerPlugin(
                stateService,
                configuration
            );

            DiscordPlugin? discordPlugin = configuration.EnableDiscordIntegration
                ? new DiscordPlugin(configuration)
                : null;

            GluonPlugin gluonPlugin = new GluonPlugin(
                stateService,
                gameLogicPlugin,
                stateManagerPlugin,
                discordPlugin
            );

            if (gluonPlugin.SetupInstance(gameHost, configWithEnv, out errorMsg))
            {
                return gluonPlugin;
            }

            return null;
        }
    }
}
