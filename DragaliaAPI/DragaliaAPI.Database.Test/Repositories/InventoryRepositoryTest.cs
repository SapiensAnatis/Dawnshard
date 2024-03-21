using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class InventoryRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly InventoryRepository inventoryRepository;

    public InventoryRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.inventoryRepository = new InventoryRepository(
            this.fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object,
            LoggerTestUtils.Create<InventoryRepository>()
        );

        AssertionOptions.AssertEquivalencyUsing(x => x.Excluding(y => y.Name.StartsWith("Owner")));
    }

    [Fact]
    public async Task AddMaterialQuantity_NoEntry_AddsQuantity()
    {
        await this.inventoryRepository.UpdateQuantity(Materials.WaterwyrmsGreatsphere, 10);

        (
            await this.fixture.ApiContext.PlayerMaterials.FindAsync(
                ViewerId,
                Materials.WaterwyrmsGreatsphere
            )
        )!
            .Quantity.Should()
            .Be(10);
    }

    [Fact]
    public async Task AddMaterialQuantity_ExistingEntry_AddsQuantity()
    {
        await this.fixture.ApiContext.AddAsync(
            new DbPlayerMaterial()
            {
                ViewerId = ViewerId,
                MaterialId = Materials.FirestormPrism,
                Quantity = 0
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        await this.inventoryRepository.UpdateQuantity(Materials.FirestormPrism, 10);

        (
            await this.fixture.ApiContext.PlayerMaterials.FindAsync(
                ViewerId,
                Materials.FirestormPrism
            )
        )!
            .Quantity.Should()
            .Be(10);
    }

    [Fact]
    public async Task AddMaterialQuantityRange_AddsQuantities()
    {
        await this.inventoryRepository.UpdateQuantity(
            new List<Materials>() { Materials.SunlightOre, Materials.SunlightStone },
            5
        );

        (await this.fixture.ApiContext.PlayerMaterials.FindAsync(ViewerId, Materials.SunlightOre))!
            .Quantity.Should()
            .Be(5);

        (
            await this.fixture.ApiContext.PlayerMaterials.FindAsync(
                ViewerId,
                Materials.SunlightStone
            )
        )!
            .Quantity.Should()
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
                    ViewerId = ViewerId + 2,
                    MaterialId = Materials.AbaddonOrb,
                    Quantity = 5
                },
                new()
                {
                    ViewerId = ViewerId,
                    MaterialId = Materials.AbaddonOrb,
                    Quantity = 50,
                }
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        (await this.inventoryRepository.GetMaterial(Materials.AbaddonOrb))
            .Should()
            .NotBeNull()
            .And.BeEquivalentTo(
                new DbPlayerMaterial()
                {
                    ViewerId = ViewerId,
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
                    ViewerId = ViewerId + 4,
                    MaterialId = Materials.AbaddonOrb,
                    Quantity = 5
                },
                new()
                {
                    ViewerId = ViewerId,
                    MaterialId = Materials.TsunamiOrb,
                    Quantity = 50,
                },
                new()
                {
                    ViewerId = ViewerId,
                    MaterialId = Materials.InfernoOrb,
                    Quantity = 50,
                }
            }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        (await this.inventoryRepository.Materials.ToListAsync())
            .Should()
            .ContainEquivalentOf( // Savefile creation adds materials
                new DbPlayerMaterial()
                {
                    ViewerId = ViewerId,
                    MaterialId = Materials.TsunamiOrb,
                    Quantity = 50
                },
                opts => opts.Excluding(x => x.Owner)
            )
            .And.ContainEquivalentOf(
                new DbPlayerMaterial()
                {
                    ViewerId = ViewerId,
                    MaterialId = Materials.InfernoOrb,
                    Quantity = 50
                },
                opts => opts.Excluding(x => x.Owner)
            )
            .And.AllSatisfy(x => x.ViewerId.Should().Be(ViewerId));
    }

    [Fact]
    public async Task UpdateQuantity_UpdatesQuantity()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbPlayerMaterial>()
            {
                new()
                {
                    ViewerId = IdentityTestUtils.ViewerId,
                    MaterialId = Materials.Valor,
                    Quantity = 5
                },
                new()
                {
                    ViewerId = IdentityTestUtils.ViewerId,
                    MaterialId = Materials.Acclaim,
                    Quantity = 5
                }
            }
        );

        await this.inventoryRepository.UpdateQuantity(
            new Dictionary<Materials, int>() { { Materials.Valor, 1 }, { Materials.Acclaim, 3 } }
        );

        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerMaterials.Single(x =>
            x.ViewerId == IdentityTestUtils.ViewerId && x.MaterialId == Materials.Valor
        )
            .Quantity.Should()
            .Be(6);

        this.fixture.ApiContext.PlayerMaterials.Single(x =>
            x.ViewerId == IdentityTestUtils.ViewerId && x.MaterialId == Materials.Acclaim
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
                    ViewerId = IdentityTestUtils.ViewerId,
                    MaterialId = Materials.SummerEstelleSkin,
                    Quantity = 5
                },
            }
        );

        await this
            .inventoryRepository.Invoking(x =>
                x.UpdateQuantity(
                    new Dictionary<Materials, int>() { { Materials.SummerEstelleSkin, -6 } }
                )
            )
            .Should()
            .ThrowAsync<InvalidOperationException>();

        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerMaterials.Single(x =>
            x.ViewerId == IdentityTestUtils.ViewerId && x.MaterialId == Materials.SummerEstelleSkin
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
                ViewerId = IdentityTestUtils.ViewerId,
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
                ViewerId = IdentityTestUtils.ViewerId,
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
                    ViewerId = IdentityTestUtils.ViewerId,
                    MaterialId = Materials.ValentinesGift,
                    Quantity = 5
                },
                new()
                {
                    ViewerId = IdentityTestUtils.ViewerId,
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
                    ViewerId = IdentityTestUtils.ViewerId,
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
