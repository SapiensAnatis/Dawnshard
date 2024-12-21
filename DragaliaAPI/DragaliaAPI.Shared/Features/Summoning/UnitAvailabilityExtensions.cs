using System.Collections.Frozen;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset.Models;

namespace DragaliaAPI.Shared.Features.Summoning;

public static class UnitAvailabilityExtensions
{
    public static UnitAvailability GetAvailability(this Charas chara) =>
        CharaAvailabilityMap.GetValueOrDefault(chara, UnitAvailability.Permanent);

    public static UnitAvailability GetAvailability(this CharaData charaData) =>
        GetAvailability(charaData.Id);

    public static UnitAvailability GetAvailability(this DragonId dragon) =>
        DragonAvailabilityMap.GetValueOrDefault(dragon, UnitAvailability.Permanent);

    public static UnitAvailability GetAvailability(this DragonData dragonData) =>
        GetAvailability(dragonData.Id);

    private static readonly FrozenDictionary<Charas, UnitAvailability> CharaAvailabilityMap =
        new Dictionary<Charas, UnitAvailability>
        {
            // Story units
            [Charas.ThePrince] = UnitAvailability.Story,
            [Charas.Elisanne] = UnitAvailability.Story,
            [Charas.Ranzal] = UnitAvailability.Story,
            [Charas.Cleo] = UnitAvailability.Story,
            [Charas.Luca] = UnitAvailability.Story,
            [Charas.Alex] = UnitAvailability.Story,
            [Charas.Laxi] = UnitAvailability.Story,
            [Charas.Zena] = UnitAvailability.Story,
            [Charas.Chelle] = UnitAvailability.Story,

            // Seasonal limited units
            [Charas.HalloweenEdward] = UnitAvailability.Limited,
            [Charas.HalloweenElisanne] = UnitAvailability.Limited,
            [Charas.HalloweenAlthemia] = UnitAvailability.Limited,

            [Charas.DragonyuleCleo] = UnitAvailability.Limited,
            [Charas.DragonyuleNefaria] = UnitAvailability.Limited,
            [Charas.DragonyuleXander] = UnitAvailability.Limited,

            [Charas.Addis] = UnitAvailability.Limited,
            [Charas.Ieyasu] = UnitAvailability.Limited,
            [Charas.Sazanka] = UnitAvailability.Limited,

            [Charas.HalloweenMym] = UnitAvailability.Limited,
            [Charas.HalloweenLowen] = UnitAvailability.Limited,
            [Charas.HalloweenOdetta] = UnitAvailability.Limited,

            [Charas.DragonyuleXainfried] = UnitAvailability.Limited,
            [Charas.DragonyuleMalora] = UnitAvailability.Limited,

            [Charas.Nobunaga] = UnitAvailability.Limited,
            [Charas.Mitsuhide] = UnitAvailability.Limited,
            [Charas.Chitose] = UnitAvailability.Limited,

            [Charas.ValentinesMelody] = UnitAvailability.Limited,
            [Charas.ValentinesAddis] = UnitAvailability.Limited,

            [Charas.HalloweenAkasha] = UnitAvailability.Limited,
            [Charas.HalloweenMelsa] = UnitAvailability.Limited,

            [Charas.DragonyuleLily] = UnitAvailability.Limited,
            [Charas.DragonyuleVictor] = UnitAvailability.Limited,

            [Charas.Seimei] = UnitAvailability.Limited,
            [Charas.Yoshitsune] = UnitAvailability.Limited,

            [Charas.ValentinesChelsea] = UnitAvailability.Limited,

            [Charas.SummerMitsuhide] = UnitAvailability.Limited,
            [Charas.SummerIeyasu] = UnitAvailability.Limited,

            [Charas.HalloweenLaxi] = UnitAvailability.Limited,
            [Charas.HalloweenSylas] = UnitAvailability.Limited,

            [Charas.DragonyuleNevin] = UnitAvailability.Limited,
            [Charas.DragonyuleIlia] = UnitAvailability.Limited,

            [Charas.Shingen] = UnitAvailability.Limited,
            [Charas.Yukimura] = UnitAvailability.Limited,

            // Gala units
            [Charas.GalaSarisse] = UnitAvailability.Gala,
            [Charas.GalaRanzal] = UnitAvailability.Gala,
            [Charas.GalaMym] = UnitAvailability.Gala,
            [Charas.GalaCleo] = UnitAvailability.Gala,
            [Charas.GalaPrince] = UnitAvailability.Gala,
            [Charas.GalaElisanne] = UnitAvailability.Gala,
            [Charas.GalaLuca] = UnitAvailability.Gala,
            [Charas.GalaAlex] = UnitAvailability.Gala,
            [Charas.GalaLeif] = UnitAvailability.Gala,
            [Charas.GalaLaxi] = UnitAvailability.Gala,
            [Charas.GalaZena] = UnitAvailability.Gala,
            [Charas.GalaLeonidas] = UnitAvailability.Gala,
            [Charas.GalaChelle] = UnitAvailability.Gala,
            [Charas.GalaNotte] = UnitAvailability.Gala,
            [Charas.GalaMascula] = UnitAvailability.Gala,
            [Charas.GalaAudric] = UnitAvailability.Gala,
            [Charas.GalaZethia] = UnitAvailability.Gala,
            [Charas.GalaGatov] = UnitAvailability.Gala,
            [Charas.GalaEmile] = UnitAvailability.Gala,
            [Charas.GalaNedrick] = UnitAvailability.Gala,

            // Welfare units
            [Charas.Celliera] = UnitAvailability.Other,
            [Charas.Melsa] = UnitAvailability.Other,
            [Charas.Elias] = UnitAvailability.Other,
            [Charas.Botan] = UnitAvailability.Other,
            [Charas.SuFang] = UnitAvailability.Other,
            [Charas.Felicia] = UnitAvailability.Other,
            [Charas.XuanZang] = UnitAvailability.Other,
            [Charas.SummerEstelle] = UnitAvailability.Other,
            [Charas.MegaMan] = UnitAvailability.Other,
            [Charas.Hanabusa] = UnitAvailability.Other,
            [Charas.Aldred] = UnitAvailability.Other,
            [Charas.WuKong] = UnitAvailability.Other,
            [Charas.SummerAmane] = UnitAvailability.Other,
            [Charas.ForagerCleo] = UnitAvailability.Other,
            [Charas.Kuzunoha] = UnitAvailability.Other,
            [Charas.SophiePersona] = UnitAvailability.Other,
            [Charas.HumanoidMidgardsormr] = UnitAvailability.Other,
            [Charas.SummerPrince] = UnitAvailability.Other,
            [Charas.Izumo] = UnitAvailability.Other,
            [Charas.Alfonse] = UnitAvailability.Other,
            [Charas.Audric] = UnitAvailability.Other,
            [Charas.Sharena] = UnitAvailability.Other,
            [Charas.Mordecai] = UnitAvailability.Other,
            [Charas.Harle] = UnitAvailability.Other,
            [Charas.Origa] = UnitAvailability.Other,

            // Collab units
            [Charas.Marth] = UnitAvailability.Limited,
            [Charas.Fjorm] = UnitAvailability.Limited,
            [Charas.Veronica] = UnitAvailability.Limited,

            [Charas.HunterBerserker] = UnitAvailability.Limited,
            [Charas.HunterVanessa] = UnitAvailability.Limited,
            [Charas.HunterSarisse] = UnitAvailability.Limited,

            [Charas.Chrom] = UnitAvailability.Limited,
            [Charas.Peony] = UnitAvailability.Limited,
            [Charas.Tiki] = UnitAvailability.Limited,

            [Charas.Mona] = UnitAvailability.Limited,
            [Charas.Joker] = UnitAvailability.Limited,
            [Charas.Panther] = UnitAvailability.Limited,

            // Not playable
            [Charas.Empty] = UnitAvailability.Other,
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<DragonId, UnitAvailability> DragonAvailabilityMap =
        new Dictionary<DragonId, UnitAvailability>()
        {
            // Story
            [DragonId.Brunhilda] = UnitAvailability.Story,
            [DragonId.Mercury] = UnitAvailability.Story,
            [DragonId.Midgardsormr] = UnitAvailability.Story,
            [DragonId.Jupiter] = UnitAvailability.Story,
            [DragonId.Zodiark] = UnitAvailability.Story,

            // Seasonal limited units
            [DragonId.HalloweenSilke] = UnitAvailability.Limited,
            [DragonId.DragonyuleJeanne] = UnitAvailability.Limited,
            [DragonId.Marishiten] = UnitAvailability.Limited,
            [DragonId.HalloweenMaritimus] = UnitAvailability.Limited,
            [DragonId.Daikokuten] = UnitAvailability.Limited,
            [DragonId.GozuTenno] = UnitAvailability.Limited,
            [DragonId.SummerMarishiten] = UnitAvailability.Limited,
            [DragonId.FudoMyoo] = UnitAvailability.Limited,

            // Gala units
            [DragonId.GalaMars] = UnitAvailability.Gala,
            [DragonId.GalaCatSith] = UnitAvailability.Gala,
            [DragonId.GalaThor] = UnitAvailability.Gala,
            [DragonId.GalaRebornPoseidon] = UnitAvailability.Gala,
            [DragonId.GalaRebornZephyr] = UnitAvailability.Gala,
            [DragonId.GalaRebornJeanne] = UnitAvailability.Gala,
            [DragonId.GalaRebornAgni] = UnitAvailability.Gala,
            [DragonId.GalaRebornNidhogg] = UnitAvailability.Gala,
            [DragonId.GalaBeastVolk] = UnitAvailability.Gala,
            [DragonId.GalaBahamut] = UnitAvailability.Gala,
            [DragonId.GalaChronosNyx] = UnitAvailability.Gala,
            [DragonId.GalaBeastCiella] = UnitAvailability.Gala,
            [DragonId.GalaElysium] = UnitAvailability.Gala,

            // Welfare units
            [DragonId.Pele] = UnitAvailability.Other,
            [DragonId.Sylvia] = UnitAvailability.Other,
            [DragonId.Maritimus] = UnitAvailability.Other,
            [DragonId.Shishimai] = UnitAvailability.Other,
            [DragonId.PengLai] = UnitAvailability.Other,
            [DragonId.Phantom] = UnitAvailability.Other,
            [DragonId.Yulong] = UnitAvailability.Other,
            [DragonId.Erasmus] = UnitAvailability.Other,
            [DragonId.Ebisu] = UnitAvailability.Other,
            [DragonId.Rathalos] = UnitAvailability.Other,
            [DragonId.Barbatos] = UnitAvailability.Other,
            [DragonId.ParallelZodiark] = UnitAvailability.Other,

            // Collab units
            [DragonId.Fatalis] = UnitAvailability.Limited,
            [DragonId.DreadkingRathalos] = UnitAvailability.Limited,
            [DragonId.Arsene] = UnitAvailability.Limited,

            // Treasure trade / other means
            [DragonId.HighMidgardsormr] = UnitAvailability.Other,
            [DragonId.HighMercury] = UnitAvailability.Other,
            [DragonId.HighBrunhilda] = UnitAvailability.Other,
            [DragonId.HighJupiter] = UnitAvailability.Other,
            [DragonId.HighZodiark] = UnitAvailability.Other,

            [DragonId.GoldFafnir] = UnitAvailability.Other,
            [DragonId.SilverFafnir] = UnitAvailability.Other,
            [DragonId.BronzeFafnir] = UnitAvailability.Other,

            [DragonId.MiniMids] = UnitAvailability.Other,
            [DragonId.MiniMercs] = UnitAvailability.Other,
            [DragonId.MiniHildy] = UnitAvailability.Other,
            [DragonId.MiniJupi] = UnitAvailability.Other,
            [DragonId.MiniZodi] = UnitAvailability.Other,

            // Not playable
            [DragonId.Puppy] = UnitAvailability.Other,
            [DragonId.Empty] = UnitAvailability.Other,
        }.ToFrozenDictionary();
}
