using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Photon.Plugin.Helpers;
using DragaliaAPI.Photon.Plugin.Models;
using MessagePack;
using Newtonsoft.Json;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
{
    /// <summary>
    /// Hardcoded plugin configuration.
    /// </summary>
    public partial class GluonPlugin
    {
        private static readonly Uri GameCreateEndpoint = new Uri(
            "Event/GameCreate",
            UriKind.Relative
        );

        private static readonly Uri GameCloseEndpoint = new Uri(
            "Event/GameClose",
            UriKind.Relative
        );

        private static readonly Uri GameJoinEndpoint = new Uri("Event/GameJoin", UriKind.Relative);

        private static readonly Uri GameLeaveEndpoint = new Uri(
            "Event/GameLeave",
            UriKind.Relative
        );

        private static readonly Uri EntryConditionsEndpoint = new Uri(
            "Event/EntryConditions",
            UriKind.Relative
        );

        private static readonly Uri MatchingTypeEndpoint = new Uri(
            "Event/MatchingType",
            UriKind.Relative
        );

        private static readonly Uri RoomIdEndpoint = new Uri("Event/RoomId", UriKind.Relative);

        private static readonly Uri VisibleEndpoint = new Uri("Event/Visible", UriKind.Relative);
    }
}
