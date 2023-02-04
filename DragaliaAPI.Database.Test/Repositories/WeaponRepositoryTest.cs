using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Test.Repositories;

public class WeaponRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IWeaponRepository weaponRepository;

    public WeaponRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.weaponRepository = new WeaponRepository(
            this.fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object
        );

        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task GetWeaponBodies_FiltersByAccountId()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbWeaponBody>()
            {
                new()
                {
                    DeviceAccountId = "other id",
                    WeaponBodyId = Shared.Definitions.Enums.WeaponBodies.SoldiersBrand
                },
                new()
                {
                    DeviceAccountId = "id",
                    WeaponBodyId = Shared.Definitions.Enums.WeaponBodies.AbsoluteAqua
                }
            }
        );

        (await this.weaponRepository.WeaponBodies.ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be("id"));
    }

    [Fact]
    public async Task Add_AddsToDatabase()
    {
        await this.weaponRepository.Add(WeaponBodies.Arondight);
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerWeapons
            .Single(
                x =>
                    x.WeaponBodyId == WeaponBodies.Arondight
                    && x.DeviceAccountId == IdentityTestUtils.DeviceAccountId
            )
            .Should()
            .BeEquivalentTo(
                new DbWeaponBody()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    WeaponBodyId = WeaponBodies.Arondight
                }
            );
    }

    [Fact]
    public async Task AddSkin_AddsToDatabase()
    {
        await this.weaponRepository.AddSkin((int)WeaponBodies.PrimalAqua);
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerWeaponSkins
            .Single(
                x =>
                    x.WeaponSkinId == (int)WeaponBodies.PrimalAqua
                    && x.DeviceAccountId == IdentityTestUtils.DeviceAccountId
            )
            .Should()
            .BeEquivalentTo(
                new DbWeaponSkin()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    WeaponSkinId = (int)WeaponBodies.PrimalAqua
                }
            );
    }

    [Fact]
    public async Task CheckOwnsWeapons_AllWeaponsOwned()
    {
        await this.fixture.AddRangeToDatabase(
            new[]
            {
                WeaponBodies.Abyssbringer,
                WeaponBodies.Blitzfang,
                WeaponBodies.Camelot
            }.Select(
                x =>
                    new DbWeaponBody()
                    {
                        DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                        WeaponBodyId = x
                    }
            )
        );

        (
            await this.weaponRepository.CheckOwnsWeapons(
                WeaponBodies.Abyssbringer,
                WeaponBodies.Blitzfang,
                WeaponBodies.Camelot
            )
        )
            .Should()
            .Be(true);
    }

    [Fact]
    public async Task CheckOwnsWeapons_SomeNotOwned()
    {
        await this.fixture.AddToDatabase(
            new DbWeaponBody()
            {
                DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                WeaponBodyId = WeaponBodies.Nothung
            }
        );

        (
            await this.weaponRepository.CheckOwnsWeapons(
                WeaponBodies.Nothung,
                WeaponBodies.Blazegambol,
                WeaponBodies.Brísingr
            )
        )
            .Should()
            .Be(false);
    }
}
