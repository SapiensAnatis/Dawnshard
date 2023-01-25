using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using FluentAssertions.Extensions;
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
        this.inventoryRepository = new InventoryRepository(this.fixture.ApiContext);

        AssertionOptions.AssertEquivalencyUsing(x => x.Excluding(x => x.Name.StartsWith("Owner")));
    }

    [Fact]
    public async Task AddMaterialQuantity_NoEntry_AddsQuantity()
    {
        await this.inventoryRepository.AddMaterialQuantity(
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

        await this.inventoryRepository.AddMaterialQuantity(
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
        await this.inventoryRepository.AddMaterialQuantity(
            DeviceAccountId,
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
}
