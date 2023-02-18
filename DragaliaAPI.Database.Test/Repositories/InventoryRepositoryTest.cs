using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

public class InventoryRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IInventoryRepository inventoryRepository;

    public InventoryRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.inventoryRepository = new InventoryRepository(
            this.fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object,
            LoggerTestUtils.Create<InventoryRepository>()
        );

        AssertionOptions.AssertEquivalencyUsing(x => x.Excluding(x => x.Name.StartsWith("Owner")));
    }

    [Fact]
    public async Task AddMaterialQuantity_NoEntry_AddsQuantity()
    {
        await this.inventoryRepository.UpdateQuantity(
            DeviceAccountId,
            Materials.WaterwyrmsGreatsphere,
            10
        );

        (
            await this.fixture.ApiContext.PlayerMaterials.FindAsync(
                DeviceAccountId,
                Materials.WaterwyrmsGreatsphere
            )
        )!.Quantity
            .Should()
            .Be(10);
    }

    [Fact]
    public async Task AddMaterialQuantity_ExistingEntry_AddsQuantity()
    {
        await this.fixture.ApiContext.AddAsync(
            new DbPlayerMaterial()
            {
                DeviceAccountId = DeviceAccountId,
                MaterialId = Materials.FirestormPrism,
                Quantity = 0
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        await this.inventoryRepository.UpdateQuantity(
            DeviceAccountId,
            Materials.FirestormPrism,
            10
        );

        (
            await this.fixture.ApiContext.PlayerMaterials.FindAsync(
                DeviceAccountId,
                Materials.FirestormPrism
            )
        )!.Quantity
            .Should()
            .Be(10);
    }

    [Fact]
    public async Task AddMaterialQuantityRange_AddsQuantities()
    {
        await this.inventoryRepository.UpdateQuantity(
            new List<Materials>() { Materials.SunlightOre, Materials.SunlightStone },
            5
        );

        (
            await this.fixture.ApiContext.PlayerMaterials.FindAsync(
                DeviceAccountId,
                Materials.SunlightOre
            )
        )!.Quantity
            .Should()
            .Be(5);

        (
            await this.fixture.ApiContext.PlayerMaterials.FindAsync(
                DeviceAccountId,
                Materials.SunlightStone
            )
        )!.Quantity
            .Should()
            .Be(5);
    }

    [Fact]
    public async Task GetMaterial_FiltersByAccountId()
    {
        await this.fixture.ApiContext.PlayerMaterials.AddRangeAsync(
            new List<DbPlayerMaterial>()
            {
                new()
                {
                    DeviceAccountId = "other id",
                    MaterialId = Materials.AbaddonOrb,
                    Quantity = 5
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    MaterialId = Materials.AbaddonOrb,
                    Quantity = 50,
                }
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        (await this.inventoryRepository.GetMaterial(DeviceAccountId, Materials.AbaddonOrb))
            .Should()
            .NotBeNull()
            .And.BeEquivalentTo(
                new DbPlayerMaterial()
                {
                    DeviceAccountId = DeviceAccountId,
                    MaterialId = Materials.AbaddonOrb,
                    Quantity = 50
                },
                opts => opts.Excluding(x => x.Owner)
            );
    }

    [Fact]
    public async Task GetMaterials_FiltersByAccountId()
    {
        await this.fixture.ApiContext.PlayerMaterials.AddRangeAsync(
            new List<DbPlayerMaterial>()
            {
                new()
                {
                    DeviceAccountId = "other id 2",
                    MaterialId = Materials.AbaddonOrb,
                    Quantity = 5
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    MaterialId = Materials.TsunamiOrb,
                    Quantity = 50,
                },
                new()
                {
                    DeviceAccountId = DeviceAccountId,
                    MaterialId = Materials.InfernoOrb,
                    Quantity = 50,
                }
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        (await this.inventoryRepository.GetMaterials(DeviceAccountId).ToListAsync())
            .Should()
            .ContainEquivalentOf( // Savefile creation adds materials
                new DbPlayerMaterial()
                {
                    DeviceAccountId = DeviceAccountId,
                    MaterialId = Materials.TsunamiOrb,
                    Quantity = 50
                },
                opts => opts.Excluding(x => x.Owner)
            )
            .And.ContainEquivalentOf(
                new DbPlayerMaterial()
                {
                    DeviceAccountId = DeviceAccountId,
                    MaterialId = Materials.InfernoOrb,
                    Quantity = 50
                },
                opts => opts.Excluding(x => x.Owner)
            )
            .And.AllSatisfy(x => x.DeviceAccountId.Should().Be(DeviceAccountId));
    }

    [Fact]
    public async Task UpdateQuantity_UpdatesQuantity()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbPlayerMaterial>()
            {
                new()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    MaterialId = Materials.Valor,
                    Quantity = 5
                },
                new()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    MaterialId = Materials.Acclaim,
                    Quantity = 5
                }
            }
        );

        await this.inventoryRepository.UpdateQuantity(
            new Dictionary<Materials, int>() { { Materials.Valor, 1 }, { Materials.Acclaim, 3 } }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerMaterials
            .Single(
                x =>
                    x.DeviceAccountId == IdentityTestUtils.DeviceAccountId
                    && x.MaterialId == Materials.Valor
            )
            .Quantity.Should()
            .Be(6);

        this.fixture.ApiContext.PlayerMaterials
            .Single(
                x =>
                    x.DeviceAccountId == IdentityTestUtils.DeviceAccountId
                    && x.MaterialId == Materials.Acclaim
            )
            .Quantity.Should()
            .Be(8);
    }

    [Fact]
    public async Task UpdateQuantity_TooFewQuantity_Throws()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbPlayerMaterial>()
            {
                new()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    MaterialId = Materials.SummerEstelleSkin,
                    Quantity = 5
                },
            }
        );

        await this.inventoryRepository
            .Invoking(
                x =>
                    x.UpdateQuantity(
                        new Dictionary<Materials, int>() { { Materials.SummerEstelleSkin, -6 } }
                    )
            )
            .Should()
            .ThrowAsync<InvalidOperationException>();

        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerMaterials
            .Single(
                x =>
                    x.DeviceAccountId == IdentityTestUtils.DeviceAccountId
                    && x.MaterialId == Materials.SummerEstelleSkin
            )
            .Quantity.Should()
            .Be(5);
    }

    [Fact]
    public async Task CheckQuantity_SingleOverload_Success()
    {
        await this.fixture.AddToDatabase(
            new DbPlayerMaterial()
            {
                DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                Quantity = 5,
                MaterialId = Materials.Dragonfruit
            }
        );

        (await this.inventoryRepository.CheckQuantity(Materials.Dragonfruit, 5)).Should().BeTrue();
    }

    [Fact]
    public async Task CheckQuantity_SingleOverload_Fail()
    {
        await this.fixture.AddToDatabase(
            new DbPlayerMaterial()
            {
                DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                Quantity = 5,
                MaterialId = Materials.FafnirMedal
            }
        );

        (await this.inventoryRepository.CheckQuantity(Materials.FafnirMedal, 6)).Should().BeFalse();
    }

    [Fact]
    public async Task CheckQuantity_MultiOverload_AllValid_ReturnsTrue()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbPlayerMaterial>()
            {
                new()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    MaterialId = Materials.ValentinesGift,
                    Quantity = 5
                },
                new()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    MaterialId = Materials.QuantumCog,
                    Quantity = 5
                }
            }
        );

        (
            await this.inventoryRepository.CheckQuantity(
                new Dictionary<Materials, int>()
                {
                    { Materials.ValentinesGift, 5 },
                    { Materials.QuantumCog, 5 }
                }
            )
        )
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task CheckQuantity_MultiOverload_SomeMissing_ReturnsFalse()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbPlayerMaterial>()
            {
                new()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    MaterialId = Materials.ValeriosConviction,
                    Quantity = 5
                }
            }
        );

        (
            await this.inventoryRepository.CheckQuantity(
                new Dictionary<Materials, int>()
                {
                    { Materials.ValeriosConviction, 5 },
                    { Materials.ValeriosDevotion, 5 }
                }
            )
        )
            .Should()
            .BeFalse();
    }
}
