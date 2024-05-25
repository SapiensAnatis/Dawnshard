namespace DragaliaAPI.Photon.Plugin.Shared
{
    /// <summary>
    /// Shared instance for mutable state to be exchanged between sub-plugins.
    /// </summary>
    public class PluginStateService
    {
        /// <summary>
        /// Gets or sets a value indicating whether the secondary server is in use by this room.
        /// </summary>
        public bool IsUseSecondaryServer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether room events should be pushed to Discord.
        /// </summary>
        public bool IsPubliclyVisible { get; set; }
    }
}
