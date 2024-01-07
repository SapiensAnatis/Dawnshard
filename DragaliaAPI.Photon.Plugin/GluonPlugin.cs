using System.Collections.Generic;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
{
    /// <summary>
    /// Core plugin which invokes sub-plugins.
    /// </summary>
    public class GluonPlugin : PluginBase
    {
        private readonly PluginStateService pluginStateService;
        private readonly GameLogicPlugin gameLogicPlugin;
        private readonly StateManagerPlugin stateManagerPlugin;

        public override string Name => nameof(GluonPlugin);

        public GluonPlugin(
            PluginStateService pluginStateService,
            GameLogicPlugin gameLogicPlugin,
            StateManagerPlugin stateManagerPlugin
        )
        {
            this.pluginStateService = pluginStateService;
            this.gameLogicPlugin = gameLogicPlugin;
            this.stateManagerPlugin = stateManagerPlugin;
        }

        public override bool SetupInstance(
            IPluginHost host,
            Dictionary<string, string> config,
            out string errorMsg
        )
        {
            this.PluginHost = host;

            return this.stateManagerPlugin.SetupInstance(host, config, out errorMsg)
                && this.gameLogicPlugin.SetupInstance(host, config, out errorMsg);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            this.gameLogicPlugin.OnCreateGame(info);

            if (this.pluginStateService.ShouldPublishToStateManager)
                this.stateManagerPlugin.OnCreateGame(info);

            if (!info.IsProcessed)
                info.Continue();
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            this.gameLogicPlugin.OnJoin(info);

            if (this.pluginStateService.ShouldPublishToStateManager)
                this.stateManagerPlugin.OnJoin(info);

            if (!info.IsProcessed)
                info.Continue();
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            this.gameLogicPlugin.OnLeave(info);

            if (this.pluginStateService.ShouldPublishToStateManager)
                this.stateManagerPlugin.OnLeave(info);

            if (!info.IsProcessed)
                base.OnLeave(info);
        }

        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            // GameLogicPlugin has no override for closing a game
            if (this.pluginStateService.ShouldPublishToStateManager)
                this.stateManagerPlugin.OnCloseGame(info);

            if (!info.IsProcessed)
                info.Continue();
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            this.gameLogicPlugin.OnRaiseEvent(info);

            if (this.pluginStateService.ShouldPublishToStateManager)
                this.stateManagerPlugin.OnRaiseEvent(info);

            if (!info.IsProcessed)
                info.Continue();
        }

        public override void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            this.gameLogicPlugin.BeforeSetProperties(info);

            if (this.pluginStateService.ShouldPublishToStateManager)
                this.stateManagerPlugin.BeforeSetProperties(info);

            if (!info.IsProcessed)
                info.Continue();
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            this.gameLogicPlugin.OnSetProperties(info);

            if (this.pluginStateService.ShouldPublishToStateManager)
                this.stateManagerPlugin.OnSetProperties(info);

            if (!info.IsProcessed)
                info.Continue();
        }
    }
}
