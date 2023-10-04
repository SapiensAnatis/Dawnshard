using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Features.TextLabel;

namespace DragaliaAPI.Shared.MasterAsset.Models;

public record AbilityData(
    int Id,
    AbilityTypes AbilityType1,
    double AbilityType1UpValue,
    double AbilityType2UpValue,
    int AbilityLimitedGroupId1,
    int EventId,
    int PartyPowerWeight,
    string Text
)
{
    public string GetFormattedText(UnitElement element, WeaponTypes weapon)
    {
        string output = Text;

        output = output.Replace("{ability_val0}", this.AbilityType1UpValue.ToString());
        output = output.Replace("{ability_val1}", this.AbilityType2UpValue.ToString());
        output = output.Replace("{element_owner}", TextLabelHelper.GetText(element));
        output = output.Replace("{weapon_owner}", TextLabelHelper.GetText(weapon));

        return output;
    }
}
