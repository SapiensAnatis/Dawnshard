namespace DragaliaAPI.Shared.Definitions.Enums;

/*
 * Based on observation via the following SQLite query:
 *
 * SELECT TextLabel._Text, FortPlantDetail._EffectId FROM FortPlantDetail
 * JOIN FortPlantData ON FortPlantData._Id = FortPlantDetail._AssetGroup
 * JOIN TextLabel ON FortPlantData._Name = TextLabel._Id
 * GROUP BY FortPlantDetail._AssetGroup
 * ORDER BY FortPlantDetail._EffectId
*/

public enum FortEffectTypes
{
    None = 0,
    Weapon = 1,
    Element = 2,
    DragonDamage = 4,
    DragonStats = 6
}
