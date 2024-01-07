using System.Collections.Generic;
using DragaliaAPI.Photon.Plugin.Models;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
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

            GluonPlugin gluonPlugin = new GluonPlugin(
                stateService,
                gameLogicPlugin,
                stateManagerPlugin
            );

            if (gluonPlugin.SetupInstance(gameHost, config, out errorMsg))
                return gluonPlugin;

            return null;
        }
    }
}
