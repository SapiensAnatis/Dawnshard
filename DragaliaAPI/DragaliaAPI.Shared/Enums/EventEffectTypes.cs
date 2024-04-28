namespace DragaliaAPI.Shared.Definitions.Enums;

/*
 * Based on observation via the following SQLite query:
 *
 * SELECT TextLabel._Text, FortPlantDetail._EventEffectType FROM FortPlantDetail
 * JOIN FortPlantData ON FortPlantData._Id = FortPlantDetail._AssetGroup
 * JOIN TextLabel ON FortPlantData._Name = TextLabel._Id
 * WHERE FortPlantDetail._EventEffectType != 0
*/

public enum EventEffectTypes
{
    None = 0,
    EventDamageBoost = 1,
}
