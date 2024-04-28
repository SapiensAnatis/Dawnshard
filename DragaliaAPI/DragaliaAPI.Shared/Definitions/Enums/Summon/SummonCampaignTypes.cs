namespace DragaliaAPI.Shared.Definitions.Enums.Summon;

/// <summary>
/// Types of Summoning Banners
/// </summary>
[Flags]
public enum SummonCampaignTypes
{
    Normal = 0,

    //UNKNOWN: These are just guesses
    Limited = 0b_0001,
    Beginner = 0b_0010,

    //UNKNOWN: Maybe for followup banners
    Consecution = 0b_0100,
}
