namespace DragaliaAPI.Shared.MasterAsset.Models;

public record DragonRarity(
    int Id,
    int MaxLimitLevel,
    int LimitLevel00,
    int LimitLevel01,
    int LimitLevel02,
    int LimitLevel03,
    int LimitLevel04,
    int LimitLevel05,
    int SkillLearningLevel01,
    int Sell,
    int BuildupBaseExp,
    int BuildupLevelExp,
    int MaxHpPlusCount,
    int MaxAtkPlusCount,
    int RarityBasePartyPower,
    int LimitBreakCountPartyPowerWeight
);
