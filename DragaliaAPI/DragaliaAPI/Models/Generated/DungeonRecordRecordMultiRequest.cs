using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class DungeonRecordRecordMultiRequest
{
    /// <summary>
    /// Gets the list of viewer IDs of other players in the room.
    /// </summary>
    /// <remarks>
    /// This property is added by the Photon server while proxying the request.
    /// </remarks>
    [Key("connecting_viewer_id_list")]
    public IList<ulong> ConnectingViewerIdList { get; set; } = [];

    /// <summary>
    /// Gets the astral raid multiplier used when joining or creating the room.
    /// </summary>
    /// <remarks>
    /// This property is added by the Photon server while proxying the request.
    /// </remarks>
    [Key("astral_bet_count")]
    public int AstralBetCount { get; set; }

    /// <summary>
    /// Gets the number of units that the player used during this quest.
    /// </summary>
    /// <remarks>
    /// This property is added by the Photon server while proxying the request.
    /// </remarks>
    [Key("member_count")]
    public int MemberCount { get; set; }
}
