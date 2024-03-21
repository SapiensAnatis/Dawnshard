using System.Reflection;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Features.Fort;
using DragaliaAPI.Features.Missions;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Services.Game;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Test.Utils;

namespace DragaliaAPI.Test.Services;

public class WeaponServiceTest
{
    private readonly Mock<IWeaponRepository> mockWeaponRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IFortRepository> mockFortRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;
    private readonly Mock<IMissionProgressionService> mockMissionProgressionService;

    private static Dictionary<Materials, int> InfernoApogeePassive1Map =
        new()
        {
            { Materials.Granite, 80 },
            { Materials.OldCloth, 30 },
            { Materials.FloatingYellowCloth, 7 },
            { Materials.UnearthlyLantern, 1 },
            { Materials.BlazeOrb, 8 }
        };

    private static Dictionary<Materials, int> MjolnirBuildup40Map =
        new() { { Materials.BronzeWhetstone, 5 }, { Materials.GoldWhetstone, 5 } };

    private static Dictionary<Materials, int> PrimalHexUnbind5Map =
        new()
        {
            { Materials.PrimalShadowwyrmsSphere, 8 },
            { Materials.PrimalShadowwyrmsGreatsphere, 5 },
            { Materials.PrimalShadowwyrmsAmethyst, 7 },
            { Materials.Orichalcum, 1 }
        };

    private static Dictionary<Materials, int> PrimalHexWeaponBonusMap =
        new()
        {
            { Materials.PrimalShadowwyrmsSphere, 25 },
            { Materials.PrimalShadowwyrmsGreatsphere, 25 },
            { Materials.PrimalShadowwyrmsAmethyst, 14 },
            { Materials.Orichalcum, 15 }
        };

    private static Dictionary<Materials, int> AmeNoMurakumoRefineMap =
        new()
        {
            { Materials.SoaringOnesMaskFragment, 40 },
            { Materials.LiberatedOnesMaskFragment, 30 },
            { Materials.RebelliousOnesCruelty, 10 },
            { Materials.RebelliousButterfliesSearingFire, 10 },
            { Materials.Orichalcum, 10 }
        };

    private readonly WeaponService weaponService;

    public WeaponServiceTest()
    {
        this.mockWeaponRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockFortRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);
        this.mockMissionProgressionService = new(MockBehavior.Strict);

        this.weaponService = new WeaponService(
            this.mockWeaponRepository.Object,
            this.mockInventoryRepository.Object,
            this.mockFortRepository.Object,
            this.mockUserDataRepository.Object,
            LoggerTestUtils.Create<WeaponService>(),
            this.mockMissionProgressionService.Object
        );
    }

    [Fact]
    public async Task ValidateCraft_AlreadyOwned_ReturnsFalse()
    {
        this.mockWeaponRepository.Setup(x => x.CheckOwnsWeapons(WeaponBodies.Abyssbringer))
            .ReturnsAsync(true);

        (await this.weaponService.ValidateCraft(WeaponBodies.Abyssbringer)).Should().BeFalse();

        this.mockWeaponRepository.VerifyAll();
    }

    [Fact]
    public async Task ValidateCraft_InsufficientPlantLevel_ReturnsFalse()
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(WeaponBodies.Gjallarhorn);

        this.mockWeaponRepository.Setup(x => x.CheckOwnsWeapons(WeaponBodies.Gjallarhorn))
            .ReturnsAsync(false);

        this.mockFortRepository.Setup(x =>
            x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel)
        )
            .ReturnsAsync(false);

        (await this.weaponService.ValidateCraft(WeaponBodies.Gjallarhorn)).Should().BeFalse();

        this.mockWeaponRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task ValidateCraft_MissingRequiredWeapon_ReturnsFalse()
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalAqua);

        this.mockWeaponRepository.Setup(x => x.CheckOwnsWeapons(WeaponBodies.PrimalAqua))
            .ReturnsAsync(false);

        this.mockFortRepository.Setup(x =>
            x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel)
        )
            .ReturnsAsync(true);

        this.mockWeaponRepository.Setup(x =>
            x.CheckOwnsWeapons(WeaponBodies.AbsoluteAqua, WeaponBodies.Empty)
        )
            .ReturnsAsync(false);

        (await this.weaponService.ValidateCraft(WeaponBodies.PrimalAqua)).Should().BeFalse();

        this.mockWeaponRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task ValidateCraft_MissingMaterials_ReturnsFalse()
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalCrimson);

        this.mockWeaponRepository.Setup(x => x.CheckOwnsWeapons(WeaponBodies.PrimalCrimson))
            .ReturnsAsync(false);

        this.mockFortRepository.Setup(x =>
            x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel)
        )
            .ReturnsAsync(true);

        this.mockWeaponRepository.Setup(x =>
            x.CheckOwnsWeapons(WeaponBodies.AbsoluteCrimson, WeaponBodies.Empty)
        )
            .ReturnsAsync(true);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(weaponData.CreateMaterialMap, y)
                )
            )
        )
            .ReturnsAsync(false);

        (await this.weaponService.ValidateCraft(WeaponBodies.PrimalCrimson)).Should().BeFalse();

        this.mockWeaponRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task ValidateCraft_MissingRupies_ReturnsFalse()
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalLightning);

        this.mockWeaponRepository.Setup(x => x.CheckOwnsWeapons(WeaponBodies.PrimalLightning))
            .ReturnsAsync(false);

        this.mockFortRepository.Setup(x =>
            x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel)
        )
            .ReturnsAsync(true);

        this.mockWeaponRepository.Setup(x =>
            x.CheckOwnsWeapons(WeaponBodies.AbsoluteLightning, WeaponBodies.Empty)
        )
            .ReturnsAsync(true);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(weaponData.CreateMaterialMap, y)
                )
            )
        )
            .ReturnsAsync(true);

        this.mockUserDataRepository.Setup(x => x.CheckCoin(weaponData.CreateCoin))
            .ReturnsAsync(false);

        (await this.weaponService.ValidateCraft(WeaponBodies.PrimalLightning)).Should().BeFalse();

        this.mockWeaponRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task ValidateCraft_AllRequirementsMet_ReturnsTrue()
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalHex);

        this.mockWeaponRepository.Setup(x => x.CheckOwnsWeapons(WeaponBodies.PrimalHex))
            .ReturnsAsync(false);

        this.mockFortRepository.Setup(x =>
            x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel)
        )
            .ReturnsAsync(true);

        this.mockWeaponRepository.Setup(x =>
            x.CheckOwnsWeapons(WeaponBodies.AbsoluteHex, WeaponBodies.Empty)
        )
            .ReturnsAsync(true);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(weaponData.CreateMaterialMap, y)
                )
            )
        )
            .ReturnsAsync(true);

        this.mockUserDataRepository.Setup(x => x.CheckCoin(weaponData.CreateCoin))
            .ReturnsAsync(true);

        (await this.weaponService.ValidateCraft(WeaponBodies.PrimalHex)).Should().BeTrue();

        this.mockWeaponRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task Craft_CallsExpectedMethods()
    {
        WeaponBody data = MasterAsset.WeaponBody.Get(WeaponBodies.WindrulersFang);

        this.mockMissionProgressionService.Setup(x =>
            x.OnWeaponEarned(
                WeaponBodies.WindrulersFang,
                UnitElement.Wind,
                6,
                WeaponSeries.PrimalDragon
            )
        );

        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(It.IsAny<Dictionary<Materials, int>>())
        )
            .Returns(Task.CompletedTask);

        this.mockUserDataRepository.Setup(x => x.UpdateCoin(-data.CreateCoin))
            .Returns(Task.CompletedTask);

        this.mockWeaponRepository.Setup(x => x.Add(WeaponBodies.WindrulersFang))
            .Returns(Task.CompletedTask);

        this.mockWeaponRepository.Setup(x => x.AddSkin((int)WeaponBodies.WindrulersFang))
            .Returns(Task.CompletedTask);

        await this.weaponService.Craft(WeaponBodies.WindrulersFang);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CheckOwned_CallsCheckOwnsWeapons(bool returnValue)
    {
        this.mockWeaponRepository.Setup(x => x.CheckOwnsWeapons(WeaponBodies.Trident))
            .ReturnsAsync(returnValue);

        (await this.weaponService.CheckOwned(WeaponBodies.Trident)).Should().Be(returnValue);

        this.mockWeaponRepository.VerifyAll();
    }

    #region TryBuildupPassive
    [Fact]
    public async Task TryBuildup_Passive_InvalidPassive_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalHex);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Passive,
                    BuildupPieceNo = 4000,
                    Step = 1
                }
            )
        ).Should().Be(ResultCode.WeaponBodyBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task TryBuildup_Passive_NotEnoughMaterials_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.InfernoApogee);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(InfernoApogeePassive1Map, y)
                )
            )
        )
            .ReturnsAsync(false);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Passive,
                    BuildupPieceNo = 1,
                    Step = 1
                }
            )
        ).Should().Be(ResultCode.CommonMaterialShort);

        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Passive_NotEnoughCoin_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.InfernoApogee);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(InfernoApogeePassive1Map, y)
                )
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(80_000)).ReturnsAsync(false);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Passive,
                    BuildupPieceNo = 1,
                    Step = 1
                }
            )
        ).Should().Be(ResultCode.CommonMaterialShort);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Passive_NotEnoughLimitBreaks_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.InfernoApogee);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(InfernoApogeePassive1Map, y)
                )
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(80_000)).ReturnsAsync(true);
        this.mockWeaponRepository.Setup(x => x.FindAsync(WeaponBodies.InfernoApogee))
            .ReturnsAsync(new DbWeaponBody() { ViewerId = 1, LimitBreakCount = 0, });

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Passive,
                    BuildupPieceNo = 1,
                    Step = 1
                }
            )
        ).Should().Be(ResultCode.WeaponBodyBuildupPieceShortLimitBreakCount);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Passive_Success_ReturnsSuccess()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.InfernoApogee);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(InfernoApogeePassive1Map, y)
                )
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(80_000)).ReturnsAsync(true);
        this.mockWeaponRepository.Setup(x => x.FindAsync(WeaponBodies.InfernoApogee))
            .ReturnsAsync(
                new DbWeaponBody()
                {
                    ViewerId = 1,
                    LimitBreakCount = 8,
                    WeaponBodyId = WeaponBodies.InfernoApogee
                }
            );
        this.mockWeaponRepository.Setup(x =>
            x.AddPassiveAbility(
                WeaponBodies.InfernoApogee,
                It.Is<WeaponPassiveAbility>(y => y.Id == body.GetPassiveAbilityId(1))
            )
        )
            .Returns(Task.CompletedTask);
        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(InfernoApogeePassive1Map.Invert(), y)
                )
            )
        )
            .Returns(Task.CompletedTask);
        this.mockUserDataRepository.Setup(x => x.UpdateCoin(-80_000)).Returns(Task.CompletedTask);
        this.mockWeaponRepository.Setup(x => x.AddSkin(30140105)).Returns(Task.CompletedTask);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Passive,
                    BuildupPieceNo = 1,
                    Step = 1
                }
            )
        ).Should().Be(ResultCode.Success);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    #endregion

    #region TryBuildupStats

    [Fact]
    public async Task TryBuildup_Stats_InvalidStats_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.Mjoelnir);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Stats,
                    BuildupPieceNo = 1,
                    Step = 222
                }
            )
        ).Should().Be(ResultCode.WeaponBodyBuildupPieceUnablePiece);

        this.mockInventoryRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Stats_NotEnoughMaterials_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.Mjoelnir);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y => ValidateMaterialMap(MjolnirBuildup40Map, y))
            )
        )
            .ReturnsAsync(false);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Stats,
                    BuildupPieceNo = 1,
                    Step = 40
                }
            )
        ).Should().Be(ResultCode.CommonMaterialShort);

        this.mockInventoryRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Stats_InvalidStep_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.Mjoelnir);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y => ValidateMaterialMap(MjolnirBuildup40Map, y))
            )
        )
            .ReturnsAsync(true);
        this.mockWeaponRepository.Setup(x => x.FindAsync(WeaponBodies.Mjoelnir))
            .ReturnsAsync(new DbWeaponBody() { ViewerId = 1, BuildupCount = 0 });

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Stats,
                    BuildupPieceNo = 1,
                    Step = 40
                }
            )
        ).Should().Be(ResultCode.WeaponBodyBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Stats_Success_ReturnsSuccess()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.Mjoelnir);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y => ValidateMaterialMap(MjolnirBuildup40Map, y))
            )
        )
            .ReturnsAsync(true);
        this.mockWeaponRepository.Setup(x => x.FindAsync(WeaponBodies.Mjoelnir))
            .ReturnsAsync(new DbWeaponBody() { ViewerId = 1, BuildupCount = 39 });
        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(MjolnirBuildup40Map.Invert(), y)
                )
            )
        )
            .Returns(Task.CompletedTask);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Stats,
                    BuildupPieceNo = 1,
                    Step = 40
                }
            )
        ).Should().Be(ResultCode.Success);

        this.mockInventoryRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }
    #endregion

    #region TryBuildupGeneric
    [Fact]
    public async Task TryBuildup_Generic_InvalidBuildup_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalHex);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Unbind,
                    BuildupPieceNo = 1,
                    Step = 4000
                }
            )
        ).Should().Be(ResultCode.WeaponBodyBuildupPieceUnablePiece);
    }

    [Fact]
    public async Task TryBuildup_Generic_NotEnoughMaterials_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalHex);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y => ValidateMaterialMap(PrimalHexUnbind5Map, y))
            )
        )
            .ReturnsAsync(false);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Unbind,
                    BuildupPieceNo = 1,
                    Step = 5
                }
            )
        ).Should().Be(ResultCode.CommonMaterialShort);

        this.mockInventoryRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Generic_NotEnoughCoin_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalHex);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y => ValidateMaterialMap(PrimalHexUnbind5Map, y))
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(7_500_000)).ReturnsAsync(false);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Unbind,
                    BuildupPieceNo = 1,
                    Step = 5
                }
            )
        ).Should().Be(ResultCode.CommonMaterialShort);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Generic_NotEnoughLimitBreaks_ReturnsError()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalHex);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(PrimalHexWeaponBonusMap, y)
                )
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(5_000_000)).ReturnsAsync(true);
        this.mockWeaponRepository.Setup(x => x.FindAsync(WeaponBodies.PrimalHex))
            .ReturnsAsync(new DbWeaponBody() { ViewerId = 1, LimitBreakCount = 0, });

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.WeaponBonus,
                    BuildupPieceNo = 1,
                    Step = 1
                }
            )
        ).Should().Be(ResultCode.WeaponBodyBuildupPieceShortLimitBreakCount);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Theory]
    [InlineData(BuildupPieceTypes.Copies, "EquipableCount", 2)]
    [InlineData(BuildupPieceTypes.Unbind, "LimitBreakCount", 2)]
    [InlineData(BuildupPieceTypes.Refine, "LimitOverCount", 1)]
    [InlineData(BuildupPieceTypes.WeaponBonus, "FortPassiveCharaWeaponBuildupCount", 1)]
    [InlineData(BuildupPieceTypes.CrestSlotType1, "AdditionalCrestSlotType1Count", 1)]
    [InlineData(BuildupPieceTypes.CrestSlotType3, "AdditionalCrestSlotType3Count", 2)]
    public async Task TryBuildup_Generic_StepError_ReturnsFailure(
        BuildupPieceTypes pieceType,
        string propName,
        int step
    )
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalHex);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(It.IsAny<Dictionary<Materials, int>>())
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(It.IsAny<long>())).ReturnsAsync(true);

        DbWeaponBody mockEntity =
            new()
            {
                ViewerId = 1,
                WeaponBodyId = WeaponBodies.PrimalHex,
                LimitBreakCount = 8
            };
        Type type = mockEntity.GetType();
        PropertyInfo prop = type.GetProperty(propName)!;
        prop.SetValue(mockEntity, step - 2, null);

        this.mockWeaponRepository.Setup(x => x.FindAsync(WeaponBodies.PrimalHex))
            .ReturnsAsync(mockEntity);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = pieceType,
                    BuildupPieceNo = 1,
                    Step = step
                }
            )
        ).Should().Be(ResultCode.WeaponBodyBuildupPieceStepError);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Generic_Success_ReturnsSuccess()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalHex);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y => ValidateMaterialMap(PrimalHexUnbind5Map, y))
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(7_500_000)).ReturnsAsync(true);
        this.mockWeaponRepository.Setup(x => x.FindAsync(WeaponBodies.PrimalHex))
            .ReturnsAsync(
                new DbWeaponBody()
                {
                    ViewerId = 1,
                    LimitBreakCount = 4,
                    WeaponBodyId = WeaponBodies.PrimalHex
                }
            );
        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(PrimalHexUnbind5Map.Invert(), y)
                )
            )
        )
            .Returns(Task.CompletedTask);
        this.mockUserDataRepository.Setup(x => x.UpdateCoin(-7_500_000))
            .Returns(Task.CompletedTask);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Unbind,
                    BuildupPieceNo = 1,
                    Step = 5
                }
            )
        ).Should().Be(ResultCode.Success);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Theory]
    [InlineData(WeaponBodies.Nothung, Materials.AdamantiteIngot)]
    [InlineData(WeaponBodies.OceanLord, Materials.DamascusIngot)]
    [InlineData(WeaponBodies.PlainAxe, Materials.SteelBrick)]
    public async Task TryBuildup_Generic_SpecialMaterial_Success_ReturnsSuccess(
        WeaponBodies id,
        Materials expectedMaterial
    )
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(id);
        Dictionary<Materials, int> expMaterialMap = new() { { expectedMaterial, 1 } };

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y => ValidateMaterialMap(expMaterialMap, y))
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(0)).ReturnsAsync(true);

        DbWeaponBody mockEntity =
            new()
            {
                ViewerId = 1,
                LimitBreakCount = 2,
                WeaponBodyId = id
            };
        this.mockWeaponRepository.Setup(x => x.FindAsync(id)).ReturnsAsync(mockEntity);

        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(expMaterialMap.Invert(), y)
                )
            )
        )
            .Returns(Task.CompletedTask);
        this.mockUserDataRepository.Setup(x => x.UpdateCoin(-0)).Returns(Task.CompletedTask);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Unbind,
                    BuildupPieceNo = 1,
                    Step = 3,
                    IsUseDedicatedMaterial = true
                }
            )
        ).Should().Be(ResultCode.Success);

        mockEntity.LimitBreakCount.Should().Be(3);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Theory]
    [InlineData(BuildupPieceTypes.Copies, "EquipableCount", 2)]
    [InlineData(BuildupPieceTypes.Unbind, "LimitBreakCount", 2)]
    [InlineData(BuildupPieceTypes.Refine, "LimitOverCount", 1)]
    [InlineData(BuildupPieceTypes.WeaponBonus, "FortPassiveCharaWeaponBuildupCount", 1)]
    [InlineData(BuildupPieceTypes.CrestSlotType1, "AdditionalCrestSlotType1Count", 1)]
    [InlineData(BuildupPieceTypes.CrestSlotType3, "AdditionalCrestSlotType3Count", 2)]
    public async Task TryBuildup_Generic_Success_ReturnsSuccessForAllProps(
        BuildupPieceTypes pieceType,
        string propName,
        int step
    )
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalHex);

        this.mockMissionProgressionService.Setup(x =>
            x.OnWeaponRefined(
                It.IsAny<int>(),
                It.IsAny<int>(),
                WeaponBodies.PrimalHex,
                body.ElementalType,
                body.Rarity,
                body.WeaponSeriesId
            )
        );

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(It.IsAny<Dictionary<Materials, int>>())
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(It.IsAny<long>())).ReturnsAsync(true);

        DbWeaponBody mockEntity =
            new()
            {
                ViewerId = 1,
                LimitBreakCount = 8,
                LimitOverCount = 1,
                WeaponBodyId = WeaponBodies.PrimalHex
            };
        Type type = mockEntity.GetType();
        PropertyInfo prop = type.GetProperty(propName)!;
        prop.SetValue(mockEntity, step - 1, null);

        this.mockWeaponRepository.Setup(x => x.FindAsync(WeaponBodies.PrimalHex))
            .ReturnsAsync(mockEntity);

        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(It.IsAny<Dictionary<Materials, int>>())
        )
            .Returns(Task.CompletedTask);
        this.mockUserDataRepository.Setup(x => x.UpdateCoin(It.IsAny<long>()))
            .Returns(Task.CompletedTask);

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = pieceType,
                    BuildupPieceNo = 1,
                    Step = step
                }
            )
        ).Should().Be(ResultCode.Success);

        prop.GetValue(mockEntity).Should().Be(step);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    [Fact]
    public async Task TryBuildup_Generic_Success_RewardsWeaponSkin()
    {
        WeaponBody body = MasterAsset.WeaponBody.Get(WeaponBodies.AmenoMurakumo);

        this.mockInventoryRepository.Setup(x =>
            x.CheckQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(AmeNoMurakumoRefineMap, y)
                )
            )
        )
            .ReturnsAsync(true);
        this.mockUserDataRepository.Setup(x => x.CheckCoin(2_500_000)).ReturnsAsync(true);
        this.mockWeaponRepository.Setup(x => x.FindAsync(WeaponBodies.AmenoMurakumo))
            .ReturnsAsync(
                new DbWeaponBody()
                {
                    ViewerId = 1,
                    LimitBreakCount = 8,
                    LimitOverCount = 1,
                    WeaponBodyId = WeaponBodies.AmenoMurakumo
                }
            );
        this.mockInventoryRepository.Setup(x =>
            x.UpdateQuantity(
                It.Is<IDictionary<Materials, int>>(y =>
                    ValidateMaterialMap(AmeNoMurakumoRefineMap.Invert(), y)
                )
            )
        )
            .Returns(Task.CompletedTask);
        this.mockUserDataRepository.Setup(x => x.UpdateCoin(-2_500_000))
            .Returns(Task.CompletedTask);
        this.mockWeaponRepository.Setup(x => x.AddSkin(30160203)).Returns(Task.CompletedTask);

        this.mockMissionProgressionService.Setup(x =>
            x.OnWeaponRefined(
                1,
                2,
                WeaponBodies.AmenoMurakumo,
                UnitElement.Water,
                6,
                WeaponSeries.Agito
            )
        );

        (
            await this.weaponService.TryBuildup(
                body,
                new AtgenBuildupWeaponBodyPieceList()
                {
                    BuildupPieceType = BuildupPieceTypes.Refine,
                    BuildupPieceNo = 1,
                    Step = 2,
                    IsUseDedicatedMaterial = true
                }
            )
        ).Should().Be(ResultCode.Success);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }

    #endregion

    /// <summary>
    /// Helper method to compare dictionaries by value instead of reference
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    private static bool ValidateMaterialMap(
        IDictionary<Materials, int> expected,
        IDictionary<Materials, int> input
    )
    {
        if (input.Count != expected.Count)
            return false;

        foreach ((Materials material, int quantity) in input)
        {
            if (expected[material] != quantity)
                return false;
        }

        return true;
    }
}
