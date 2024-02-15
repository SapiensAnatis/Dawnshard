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
        public IGamePlugin Create(
            IPluginHost gameHost,
            string pluginName,
            Dictionary<string, string> config,
            out string errorMsg
        )
        {
            PluginConfiguration configuration = new PluginConfiguration(config);
            PluginStateService stateService = new PluginStateService();

            GameLogicPlugin gameLogicPlugin = new GameLogicPlugin(stateService, configuration);
            StateManagerPlugin stateManagerPlugin = new StateManagerPlugin(
                stateService,
                configuration
            );
            DiscordPlugin discordPlugin = new DiscordPlugin(configuration);

            GluonPlugin gluonPlugin = new GluonPlugin(
                stateService,
                gameLogicPlugin,
                stateManagerPlugin,
                discordPlugin
            );

            if (gluonPlugin.SetupInstance(gameHost, config, out errorMsg))
                return gluonPlugin;

            return null;
        }
    }
}
