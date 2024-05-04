using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Definitions.Enums.Summon;
using DragaliaAPI.Shared.Features.Summoning;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Features.Summoning;

/// <summary>
/// Contains definitions for banners that are always present regardless of configuration.
/// </summary>
public static class SystemBanners
{
    /// <summary>
    /// Gets the banner for use in the tutorial reroll summon.
    /// </summary>
    public static Banner RerollBanner { get; } =
        new()
        {
            Id = SummonConstants.RedoableSummonBannerId,
            IsGala = true,
            SummonType = SummonTypes.TutorialRedoable,
            Start = DateTimeOffset.MinValue,
            End = DateTimeOffset.MaxValue,
        };

    /// <summary>
    /// Gets the banner for use with the regular 5★ Aventurer Summon voucher.
    /// </summary>
    /// <remarks>
    /// See: <see href="https://dragalialost.wiki/w/5%E2%98%85_Adventurer_Summon#Christmas%20Update"/>
    /// </remarks>
    public static Banner AdventurerSummonBanner { get; } =
        new()
        {
            Id = MasterAsset.SummonTicket[SummonTickets.AdventurerSummon].SummonId,
            RequiredTicketId = SummonTickets.AdventurerSummon,
            SummonType = SummonTypes.CharaSsr,
            Start = DateTimeOffset.MinValue,
            End = DateTimeOffset.MaxValue,
            OverrideCharaPool =
            [
                Charas.Naveed,
                Charas.Mikoto,
                Charas.Ezelith,
                Charas.Xander,
                Charas.Xainfried,
                Charas.Lily,
                Charas.Hawk,
                Charas.Louise,
                Charas.Maribelle,
                Charas.Julietta,
                Charas.Lucretia,
                Charas.Hildegarde,
                Charas.Nefaria
            ]
        };

    /// <summary>
    /// Gets the banner for use with the regular 5★ Dragon Summon voucher.
    /// </summary>
    /// <remarks>
    /// See: <see href="https://dragalialost.wiki/w/5%E2%98%85_Dragon_Summon"/>
    /// </remarks>
    public static Banner DragonSummonBanner { get; } =
        new()
        {
            Id = MasterAsset.SummonTicket[SummonTickets.DragonSummon].SummonId,
            RequiredTicketId = SummonTickets.DragonSummon,
            SummonType = SummonTypes.DragonSsr,
            Start = DateTimeOffset.MinValue,
            End = DateTimeOffset.MaxValue,
            OverrideDragonPool =
            [
                Dragons.Agni,
                Dragons.Cerberus,
                Dragons.Prometheus,
                Dragons.KonohanaSakuya,
                Dragons.Arctos,
                Dragons.Poseidon,
                Dragons.Leviathan,
                Dragons.Siren,
                Dragons.Simurgh,
                Dragons.Zephyr,
                Dragons.Garuda,
                Dragons.LongLong,
                Dragons.Pazuzu,
                Dragons.Freyja,
                Dragons.Vayu,
                Dragons.JeannedArc,
                Dragons.Gilgamesh,
                Dragons.Cupid,
                Dragons.Liger,
                Dragons.Takemikazuchi,
                Dragons.PopStarSiren,
                Dragons.Nidhogg,
                Dragons.Nyarlathotep,
                Dragons.Shinobi
            ]
        };

    /// <summary>
    /// Gets the banner for use with the enhanced 5★ Aventurer Summon voucher.
    /// </summary>
    /// <remarks>
    /// See: <see href="https://dragalialost.wiki/w/5%E2%98%85_Adventurer_Summon%2B"/>
    /// </remarks>
    public static Banner AdventurerSummonPlusBanner { get; } =
        new()
        {
            Id = SummonConstants.AdventurerSummonPlusBannerId,
            RequiredTicketId = SummonTickets.AdventurerSummonPlus,
            SummonType = SummonTypes.CharaSsrUpdate,
            Start = DateTimeOffset.MinValue,
            End = DateTimeOffset.MaxValue,
            OverrideCharaPool = Enum.GetValues<Charas>()
                .Where(x => x != Charas.Empty)
                .Where(x =>
                {
                    CharaData charaData = MasterAsset.CharaData[x];
                    return charaData.Rarity == 5
                        && charaData.GetAvailability() == UnitAvailability.Permanent;
                })
                .ToList()
        };

    /// <summary>
    /// Gets the banner for use with the enhanced 5★ Dragon Summon voucher.
    /// </summary>
    /// <remarks>
    /// See: <see href="https://dragalialost.wiki/w/5%E2%98%85_Dragon_Summon%2B"/>
    /// </remarks>
    public static Banner DragonSummonPlusBanner { get; } =
        new()
        {
            Id = SummonConstants.DragonSummonPlusBannerId,
            RequiredTicketId = SummonTickets.DragonSummonPlus,
            SummonType = SummonTypes.DragonSsrUpdate,
            Start = DateTimeOffset.MinValue,
            End = DateTimeOffset.MaxValue,
            OverrideDragonPool = Enum.GetValues<Dragons>()
                .Where(x => x != Dragons.Empty)
                .Where(x =>
                {
                    DragonData dragonData = MasterAsset.DragonData[x];
                    return dragonData.Rarity == 5
                        && dragonData.GetAvailability() == UnitAvailability.Permanent;
                })
                .ToList()
        };
}
