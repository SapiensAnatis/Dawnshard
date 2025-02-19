﻿using System.Collections.Generic;
using DragaliaAPI.Photon.Plugin.Plugins.Discord;
using DragaliaAPI.Photon.Plugin.Plugins.GameLogic;
using DragaliaAPI.Photon.Plugin.Plugins.StateManager;
using DragaliaAPI.Photon.Plugin.Shared;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Plugins.Gluon
{
    /// <summary>
    /// Core plugin which invokes sub-plugins.
    /// </summary>
    public class GluonPlugin : PluginBase
    {
        private readonly PluginStateService pluginStateService;
        private readonly GameLogicPlugin gameLogicPlugin;
        private readonly StateManagerPlugin stateManagerPlugin;
        private readonly DiscordPlugin? discordPlugin;

        public override string Name => nameof(GluonPlugin);

        public GluonPlugin(
            PluginStateService pluginStateService,
            GameLogicPlugin gameLogicPlugin,
            StateManagerPlugin stateManagerPlugin,
            DiscordPlugin? discordPlugin
        )
        {
            this.pluginStateService = pluginStateService;
            this.gameLogicPlugin = gameLogicPlugin;
            this.stateManagerPlugin = stateManagerPlugin;
            this.discordPlugin = discordPlugin;
        }

        public override bool SetupInstance(
            IPluginHost host,
            Dictionary<string, string> config,
            out string errorMsg
        )
        {
            this.PluginHost = host;

            if (!this.stateManagerPlugin.SetupInstance(host, config, out errorMsg))
            {
                return false;
            }
            if (!this.gameLogicPlugin.SetupInstance(host, config, out errorMsg))
            {
                return false;
            }
            if (
                this.discordPlugin is not null
                && !this.discordPlugin.SetupInstance(host, config, out errorMsg)
            )
            {
                return false;
            }

            return true;
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            this.gameLogicPlugin.OnCreateGame(info);
            this.stateManagerPlugin.OnCreateGame(info);

            if (this.pluginStateService.IsPubliclyVisible)
            {
                this.discordPlugin?.OnCreateGame(info);
            }

            if (!info.IsProcessed)
            {
                info.Continue();
            }
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            this.gameLogicPlugin.OnJoin(info);
            this.stateManagerPlugin.OnJoin(info);

            if (!info.IsProcessed)
            {
                info.Continue();
            }
        }

        public override void OnLeave(ILeaveGameCallInfo info)
        {
            this.gameLogicPlugin.OnLeave(info);
            this.stateManagerPlugin.OnLeave(info);

            if (!info.IsProcessed)
            {
                base.OnLeave(info);
            }
        }

        public override void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            // This can't use OnCloseGame with StateManagerPlugin, as there can only be one synchronous outbound HTTP request
            // per event handler. (Unless we chain them together using callbacks...)

            if (this.pluginStateService.IsPubliclyVisible)
            {
                this.discordPlugin?.BeforeCloseGame(info);
            }

            if (!info.IsProcessed)
            {
                info.Continue();
            }
        }

        public override void OnCloseGame(ICloseGameCallInfo info)
        {
            // GameLogicPlugin has no override for closing a game
            this.stateManagerPlugin.OnCloseGame(info);

            if (!info.IsProcessed)
            {
                info.Continue();
            }
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            this.gameLogicPlugin.OnRaiseEvent(info);
            this.stateManagerPlugin.OnRaiseEvent(info);

            if (!info.IsProcessed)
            {
                info.Continue();
            }
        }

        public override void BeforeSetProperties(IBeforeSetPropertiesCallInfo info)
        {
            this.gameLogicPlugin.BeforeSetProperties(info);

            if (!info.IsProcessed)
            {
                info.Continue();
            }
        }

        public override void OnSetProperties(ISetPropertiesCallInfo info)
        {
            this.gameLogicPlugin.OnSetProperties(info);
            this.stateManagerPlugin.OnSetProperties(info);

            if (this.pluginStateService.IsPubliclyVisible)
            {
                this.discordPlugin?.OnSetProperties(info);
            }

            if (!info.IsProcessed)
            {
                info.Continue();
            }
        }
    }
}
