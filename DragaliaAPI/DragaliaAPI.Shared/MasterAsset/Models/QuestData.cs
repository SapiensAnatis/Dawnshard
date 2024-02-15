using System.Text.Json.Serialization;
using DragaliaAPI.Photon.Shared.Enums;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Json;
using DragaliaAPI.Shared.MasterAsset.Models.Event;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record QuestData(
    int Id,
    int Gid,
    QuestGroupType GroupType,
    QuestPlayModeTypes QuestPlayModeType,
    UnitElement LimitedElementalType,
    UnitElement LimitedElementalType2,
    int LimitedWeaponTypePatternId,
    [property: JsonConverter(typeof(BoolIntJsonConverter))] bool IsPayForceStaminaSingle,
    int PayStaminaSingle,
    int CampaignStaminaSingle,
    int PayStaminaMulti,
    int CampaignStaminaMulti,
    DungeonTypes DungeonType,
    VariationTypes VariationType,
    string Scene01,
    string AreaName01,
    string Scene02,
    string AreaName02,
    string Scene03,
    string AreaName03,
    string Scene04,
    string AreaName04,
    string Scene05,
    string AreaName05,
    string Scene06,
    string AreaName06,
    int RebornLimit,
    int ContinueLimit,
    int Difficulty,
    PayTargetType PayEntityTargetType,
    EntityTypes PayEntityType,
    int PayEntityId,
    int PayEntityQuantity,
    EntityTypes HoldEntityType,
    int HoldEntityId,
    int HoldEntityQuantity,
    [property: JsonConverter(typeof(BoolIntJsonConverter))] bool IsSumUpTotalDamage
)
{
    private int IdSuffix => this.Id % 1000;

    public IEnumerable<AreaInfo> AreaInfo =>
        new List<AreaInfo>()
        {
            new(this.Scene01, this.AreaName01),
            new(this.Scene02, this.AreaName02),
            new(this.Scene03, this.AreaName03),
            new(this.Scene04, this.AreaName04),
            new(this.Scene05, this.AreaName05),
            new(this.Scene06, this.AreaName06),
        }.Where(x => !string.IsNullOrEmpty(x.ScenePath) && !string.IsNullOrEmpty(x.AreaName));

    public bool IsEventRegularBattle =>
        this.EventKindType switch
        {
            EventKindType.Build => this.IdSuffix is 301 or 302 or 303 or 401, // Boss battle (or EX boss battle)
            EventKindType.Raid => this.IdSuffix is 201 or 202 or 203, // Boss battle
            EventKindType.Earn => this.IdSuffix is 201 or 202 or 203 or 401, // Invasion quest
            _ => false
        };

    public bool IsEventChallengeBattle =>
        this.EventKindType switch
        {
            EventKindType.Build => this.IdSuffix is 501 or 502,
            _ => false
        };

    public bool IsEventTrial =>
        this.EventKindType switch
        {
            EventKindType.Build => this.IdSuffix is 701 or 702,
            EventKindType.Earn => this.IdSuffix is 301 or 302 or 303,
            _ => false
        };

    public bool IsEventExBattle =>
        this.EventKindType switch
        {
            EventKindType.Build => this.IdSuffix is 401,
            _ => false,
        };

    public EventKindType EventKindType =>
        MasterAsset.EventData.TryGetValue(this.Gid, out EventData? eventData)
            ? eventData.EventKindType
            : EventKindType.None;

    public bool IsEventQuest => GroupType == QuestGroupType.Event;

    public bool CanPlayCoOp => this.PayStaminaMulti > 0;
}
