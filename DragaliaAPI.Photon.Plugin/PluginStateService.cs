using System;
using DragaliaAPI.Photon.Plugin.Models;

namespace DragaliaAPI.Photon.Plugin
{
    /// <summary>
    /// Shared instance for mutable state to be exchanged between sub-plugins.
    /// </summary>
    public class PluginStateService
    {
        public bool IsUseSecondaryServer { get; set; }

        public bool ShouldPublishToStateManager { get; set; }
    }
}
