using System.Collections.Generic;
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
            GluonPlugin plugin = new GluonPlugin();
            if (plugin.SetupInstance(gameHost, config, out errorMsg))
            {
                return plugin;
            }
            return null;
        }
    }
}
