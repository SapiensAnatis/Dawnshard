using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class FriendReplyRequest
{
    [Key("reply")]
    public FriendReplyType Reply { get; set; }
}
