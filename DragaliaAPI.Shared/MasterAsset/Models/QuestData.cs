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

    public bool IsEventBossBattle
    {
        get
        {
            int idSuffix = this.Id % 1000;

            return this.EventKindType switch
            {
                EventKindType.Build => idSuffix is 301 or 302 or 303,
                EventKindType.Raid => idSuffix is 201 or 202 or 203,
                _ => false
            };
        }
    }

    public bool IsEventChallengeBattle
    {
        get
        {
            int idSuffix = this.Id % 1000;

            return this.EventKindType switch
            {
                EventKindType.Build => idSuffix is 501 or 502,
                _ => false
            };
        }
    }

    public bool IsEventTrial
    {
        get
        {
            int idSuffix = this.Id % 1000;

            return this.EventKindType switch
            {
                EventKindType.Build => idSuffix is 701 or 702,
                _ => false
            };
        }
    }

    public EventKindType EventKindType =>
        MasterAsset.EventData.TryGetValue(this.Gid, out EventData? eventData)
            ? eventData.EventKindType
            : EventKindType.None;

    public bool IsEventQuest => GroupType == QuestGroupType.Event;

    public bool CanPlayCoOp => this.PayStaminaMulti > 0;
}
