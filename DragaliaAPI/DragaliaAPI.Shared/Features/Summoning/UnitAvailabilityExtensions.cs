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

    public static UnitAvailability GetAvailability(this Dragons dragon) =>
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

    private static readonly FrozenDictionary<Dragons, UnitAvailability> DragonAvailabilityMap =
        new Dictionary<Dragons, UnitAvailability>()
        {
            // Story
            [Dragons.Brunhilda] = UnitAvailability.Story,
            [Dragons.Mercury] = UnitAvailability.Story,
            [Dragons.Midgardsormr] = UnitAvailability.Story,
            [Dragons.Jupiter] = UnitAvailability.Story,
            [Dragons.Zodiark] = UnitAvailability.Story,

            // Seasonal limited units
            [Dragons.HalloweenSilke] = UnitAvailability.Limited,
            [Dragons.DragonyuleJeanne] = UnitAvailability.Limited,
            [Dragons.Marishiten] = UnitAvailability.Limited,
            [Dragons.HalloweenMaritimus] = UnitAvailability.Limited,
            [Dragons.Daikokuten] = UnitAvailability.Limited,
            [Dragons.GozuTenno] = UnitAvailability.Limited,
            [Dragons.SummerMarishiten] = UnitAvailability.Limited,
            [Dragons.FudoMyoo] = UnitAvailability.Limited,

            // Gala units
            [Dragons.GalaMars] = UnitAvailability.Gala,
            [Dragons.GalaCatSith] = UnitAvailability.Gala,
            [Dragons.GalaThor] = UnitAvailability.Gala,
            [Dragons.GalaRebornPoseidon] = UnitAvailability.Gala,
            [Dragons.GalaRebornZephyr] = UnitAvailability.Gala,
            [Dragons.GalaRebornJeanne] = UnitAvailability.Gala,
            [Dragons.GalaRebornAgni] = UnitAvailability.Gala,
            [Dragons.GalaRebornNidhogg] = UnitAvailability.Gala,
            [Dragons.GalaBeastVolk] = UnitAvailability.Gala,
            [Dragons.GalaBahamut] = UnitAvailability.Gala,
            [Dragons.GalaChronosNyx] = UnitAvailability.Gala,
            [Dragons.GalaBeastCiella] = UnitAvailability.Gala,
            [Dragons.GalaElysium] = UnitAvailability.Gala,

            // Welfare units
            [Dragons.Pele] = UnitAvailability.Other,
            [Dragons.Sylvia] = UnitAvailability.Other,
            [Dragons.Maritimus] = UnitAvailability.Other,
            [Dragons.Shishimai] = UnitAvailability.Other,
            [Dragons.PengLai] = UnitAvailability.Other,
            [Dragons.Phantom] = UnitAvailability.Other,
            [Dragons.Yulong] = UnitAvailability.Other,
            [Dragons.Erasmus] = UnitAvailability.Other,
            [Dragons.Ebisu] = UnitAvailability.Other,
            [Dragons.Rathalos] = UnitAvailability.Other,
            [Dragons.Barbatos] = UnitAvailability.Other,
            [Dragons.ParallelZodiark] = UnitAvailability.Other,

            // Collab units
            [Dragons.Fatalis] = UnitAvailability.Limited,
            [Dragons.DreadkingRathalos] = UnitAvailability.Limited,
            [Dragons.Arsene] = UnitAvailability.Limited,

            // Treasure trade / other means
            [Dragons.HighMidgardsormr] = UnitAvailability.Other,
            [Dragons.HighMercury] = UnitAvailability.Other,
            [Dragons.HighBrunhilda] = UnitAvailability.Other,
            [Dragons.HighJupiter] = UnitAvailability.Other,
            [Dragons.HighZodiark] = UnitAvailability.Other,

            [Dragons.GoldFafnir] = UnitAvailability.Other,
            [Dragons.SilverFafnir] = UnitAvailability.Other,
            [Dragons.BronzeFafnir] = UnitAvailability.Other,

            [Dragons.MiniMids] = UnitAvailability.Other,
            [Dragons.MiniMercs] = UnitAvailability.Other,
            [Dragons.MiniHildy] = UnitAvailability.Other,
            [Dragons.MiniJupi] = UnitAvailability.Other,
            [Dragons.MiniZodi] = UnitAvailability.Other,

            // Not playable
            [Dragons.Puppy] = UnitAvailability.Other,
            [Dragons.Empty] = UnitAvailability.Other
        }.ToFrozenDictionary();
}
