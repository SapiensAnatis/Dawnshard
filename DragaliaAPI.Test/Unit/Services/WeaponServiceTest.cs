using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Services;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Test.Utils;

namespace DragaliaAPI.Test.Unit.Services;

public class WeaponServiceTest
{
    private readonly Mock<IWeaponRepository> mockWeaponRepository;
    private readonly Mock<IInventoryRepository> mockInventoryRepository;
    private readonly Mock<IFortRepository> mockFortRepository;
    private readonly Mock<IUserDataRepository> mockUserDataRepository;

    private readonly IWeaponService weaponService;

    public WeaponServiceTest()
    {
        this.mockWeaponRepository = new(MockBehavior.Strict);
        this.mockInventoryRepository = new(MockBehavior.Strict);
        this.mockFortRepository = new(MockBehavior.Strict);
        this.mockUserDataRepository = new(MockBehavior.Strict);

        this.weaponService = new WeaponService(
            this.mockWeaponRepository.Object,
            this.mockInventoryRepository.Object,
            this.mockFortRepository.Object,
            this.mockUserDataRepository.Object,
            LoggerTestUtils.Create<WeaponService>()
        );
    }

    [Fact]
    public async Task ValidateCraft_AlreadyOwned_ReturnsFalse()
    {
        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.Abyssbringer))
            .ReturnsAsync(true);

        (await this.weaponService.ValidateCraft(WeaponBodies.Abyssbringer)).Should().BeFalse();

        this.mockWeaponRepository.VerifyAll();
    }

    [Fact]
    public async Task ValidateCraft_InsufficientPlantLevel_ReturnsFalse()
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(WeaponBodies.Gjallarhorn);

        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.Gjallarhorn))
            .ReturnsAsync(false);

        this.mockFortRepository
            .Setup(x => x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel))
            .ReturnsAsync(false);

        (await this.weaponService.ValidateCraft(WeaponBodies.Gjallarhorn)).Should().BeFalse();

        this.mockWeaponRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task ValidateCraft_MissingRequiredWeapon_ReturnsFalse()
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalAqua);

        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.PrimalAqua))
            .ReturnsAsync(false);

        this.mockFortRepository
            .Setup(x => x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel))
            .ReturnsAsync(true);

        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.AbsoluteAqua, WeaponBodies.Empty))
            .ReturnsAsync(false);

        (await this.weaponService.ValidateCraft(WeaponBodies.PrimalAqua)).Should().BeFalse();

        this.mockWeaponRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
    }

    [Fact]
    public async Task ValidateCraft_MissingMaterials_ReturnsFalse()
    {
        WeaponBody weaponData = MasterAsset.WeaponBody.Get(WeaponBodies.PrimalCrimson);

        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.PrimalCrimson))
            .ReturnsAsync(false);

        this.mockFortRepository
            .Setup(x => x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel))
            .ReturnsAsync(true);

        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.AbsoluteCrimson, WeaponBodies.Empty))
            .ReturnsAsync(true);

        this.mockInventoryRepository
            .Setup(x => x.CheckQuantity(weaponData.CreateMaterialMap))
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

        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.PrimalLightning))
            .ReturnsAsync(false);

        this.mockFortRepository
            .Setup(x => x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel))
            .ReturnsAsync(true);

        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.AbsoluteLightning, WeaponBodies.Empty))
            .ReturnsAsync(true);

        this.mockInventoryRepository
            .Setup(x => x.CheckQuantity(weaponData.CreateMaterialMap))
            .ReturnsAsync(true);

        this.mockUserDataRepository
            .Setup(x => x.CheckCoin(weaponData.CreateCoin))
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

        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.PrimalHex))
            .ReturnsAsync(false);

        this.mockFortRepository
            .Setup(x => x.CheckPlantLevel(FortPlants.Smithy, weaponData.NeedFortCraftLevel))
            .ReturnsAsync(true);

        this.mockWeaponRepository
            .Setup(x => x.CheckOwnsWeapons(WeaponBodies.AbsoluteHex, WeaponBodies.Empty))
            .ReturnsAsync(true);

        this.mockInventoryRepository
            .Setup(x => x.CheckQuantity(weaponData.CreateMaterialMap))
            .ReturnsAsync(true);

        this.mockUserDataRepository
            .Setup(x => x.CheckCoin(weaponData.CreateCoin))
            .ReturnsAsync(true);

        (await this.weaponService.ValidateCraft(WeaponBodies.PrimalHex)).Should().BeTrue();

        this.mockWeaponRepository.VerifyAll();
        this.mockFortRepository.VerifyAll();
        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
    }

    [Fact]
    public async Task Add_CallsExpectedMethods()
    {
        WeaponBody data = MasterAsset.WeaponBody.Get(WeaponBodies.WindrulersFang);

        this.mockInventoryRepository
            .Setup(x => x.UpdateQuantity(It.IsAny<Dictionary<Materials, int>>()))
            .Returns(Task.CompletedTask);

        this.mockUserDataRepository
            .Setup(x => x.UpdateCoin(-data.CreateCoin))
            .Returns(Task.CompletedTask);

        this.mockWeaponRepository
            .Setup(x => x.Add(WeaponBodies.WindrulersFang))
            .Returns(Task.CompletedTask);

        this.mockWeaponRepository
            .Setup(x => x.AddSkin((int)WeaponBodies.WindrulersFang))
            .Returns(Task.CompletedTask);

        await this.weaponService.Craft(WeaponBodies.WindrulersFang);

        this.mockInventoryRepository.VerifyAll();
        this.mockUserDataRepository.VerifyAll();
        this.mockWeaponRepository.VerifyAll();
    }
}
