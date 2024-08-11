using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.Event;
using DragaliaAPI.MessagePack;
using MessagePack;

namespace DragaliaAPI.Models.Generated;

public partial class EventSummonGetDataRequest : IEventRequest;

public partial class EventSummonExecRequest : IEventRequest;

public partial class EventSummonResetRequest : IEventRequest;

public partial class EventTradeGetListResponse
{
    // This can't be initialized as [] or else the game will delete all your materials
    // https://github.com/SapiensAnatis/Dawnshard/pull/687
    [Key("material_list")]
    public IEnumerable<MaterialList> MaterialList { get; set; }
}

public partial class EventTradeTradeResponse
{
    [Key("material_list")]
    public IEnumerable<MaterialList> MaterialList { get; set; }
}

public partial class RaidEventEntryRequest : IEventRequest
{
    int IEventRequest.EventId => this.RaidEventId;
}

public partial class EventOverrides : IEventRequest
{
    int IEventRequest.EventId => this.RaidEventId;
}

public partial class RaidEventGetEventDataRequest : IEventRequest
{
    int IEventRequest.EventId => this.RaidEventId;
}

public partial class BuildEventRewardList : IEventRewardList<BuildEventRewardList>
{
    public static BuildEventRewardList FromDatabase(DbPlayerEventReward reward) =>
        new(reward.EventId, reward.RewardId);
}

public partial class RaidEventRewardList : IEventRewardList<RaidEventRewardList>
{
    public static RaidEventRewardList FromDatabase(DbPlayerEventReward reward) =>
        new(reward.EventId, reward.RewardId);
}

public partial class UserEventLocationRewardList : IEventRewardList<UserEventLocationRewardList>
{
    public static UserEventLocationRewardList FromDatabase(DbPlayerEventReward reward) =>
        new(reward.EventId, reward.RewardId);
}

public partial class AtgenBoxSummonDetail
{
    [Key("reset_item_flag")]
    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    public bool ResetItemFlag { get; set; }
}

public partial class AtgenBoxSummonData
{
    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [Key("reset_possible")]
    public bool ResetPossible { get; set; }
}

public partial class AtgenBoxSummonResult
{
    [MessagePackFormatter(typeof(BoolToIntFormatter))]
    [Key("reset_possible")]
    public bool ResetPossible { get; set; }
}
