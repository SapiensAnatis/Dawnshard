using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.MasterAsset.Models;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class WeaponRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly WeaponRepository weaponRepository;

    public WeaponRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.weaponRepository = new WeaponRepository(
            this.fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object,
            LoggerTestUtils.Create<WeaponRepository>()
        );

        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
        CommonAssertionOptions.ApplyTimeOptions();
    }

    [Fact]
    public async Task GetWeaponBodies_FiltersByAccountId()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbWeaponBody>()
            {
                new() { ViewerId = 2, WeaponBodyId = WeaponBodies.SoldiersBrand },
                new() { ViewerId = 1, WeaponBodyId = WeaponBodies.AbsoluteAqua }
            }
        );

        (await this.weaponRepository.WeaponBodies.ToListAsync())
            .Should()
            .AllSatisfy(x => x.ViewerId.Should().Be(1));
    }

    [Fact]
    public async Task GetPassiveAbilities_FiltersOutAstralBane()
    {
        await this.fixture.AddRangeToDatabase(
            new List<DbWeaponPassiveAbility>()
            {
                new() { ViewerId = 1, WeaponPassiveAbilityId = 1010107 },
                new() { ViewerId = 1, WeaponPassiveAbilityId = 1010108 }
            }
        );

        this.weaponRepository.GetPassiveAbilities(WeaponBodies.Nothung)
            .Should()
            .NotContain(x => x.WeaponPassiveAbilityId == 1010108);
    }

    [Fact]
    public async Task Add_AddsToDatabase()
    {
        await this.weaponRepository.Add(WeaponBodies.Arondight);
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerWeapons.Single(x =>
            x.WeaponBodyId == WeaponBodies.Arondight && x.ViewerId == IdentityTestUtils.ViewerId
        )
            .Should()
            .BeEquivalentTo(
                new DbWeaponBody()
                {
                    ViewerId = IdentityTestUtils.ViewerId,
                    WeaponBodyId = WeaponBodies.Arondight
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
            }.Select(x => new DbWeaponBody()
            {
                ViewerId = IdentityTestUtils.ViewerId,
                WeaponBodyId = x
            })
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
                ViewerId = IdentityTestUtils.ViewerId,
                WeaponBodyId = WeaponBodies.Nothung
            }
        );

        (
            await this.weaponRepository.CheckOwnsWeapons(
                WeaponBodies.Nothung,
                WeaponBodies.Blazegambol,
                WeaponBodies.Brisingr
            )
        )
            .Should()
            .Be(false);
    }

    [Fact]
    public async Task AddPassiveAbility_AddsPassiveAbility()
    {
        await this.fixture.AddToDatabase(
            new DbWeaponBody()
            {
                ViewerId = IdentityTestUtils.ViewerId,
                WeaponBodyId = WeaponBodies.InfernoApogee
            }
        );

        int passiveId = MasterAsset
            .WeaponBody.Get(WeaponBodies.InfernoApogee)
            .GetPassiveAbilityId(1);
        WeaponPassiveAbility passiveAbility = MasterAsset.WeaponPassiveAbility.Get(passiveId);

        await this.weaponRepository.AddPassiveAbility(WeaponBodies.InfernoApogee, passiveAbility);
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerWeapons.Single(x =>
            x.WeaponBodyId == WeaponBodies.InfernoApogee
        )
            .UnlockWeaponPassiveAbilityNoList.Should()
            .BeEquivalentTo(Enumerable.Repeat(0, 14).Prepend(1));
        this.fixture.ApiContext.PlayerPassiveAbilities.Should()
            .ContainEquivalentOf(
                new DbWeaponPassiveAbility()
                {
                    ViewerId = IdentityTestUtils.ViewerId,
                    WeaponPassiveAbilityId = passiveId
                }
            );
    }

    [Fact]
    public async Task AddPassiveAbility_AbilityOwned_DoesNotThrow()
    {
        await this.fixture.AddToDatabase(
            new DbWeaponBody()
            {
                ViewerId = IdentityTestUtils.ViewerId,
                WeaponBodyId = WeaponBodies.RoaringWeald
            }
        );

        int passiveId = MasterAsset
            .WeaponBody.Get(WeaponBodies.RoaringWeald)
            .GetPassiveAbilityId(1);
        WeaponPassiveAbility passiveAbility = MasterAsset.WeaponPassiveAbility.Get(passiveId);

        await this
            .weaponRepository.Invoking(x =>
                x.AddPassiveAbility(WeaponBodies.RoaringWeald, passiveAbility)
            )
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task AddSkin_AddsSkin()
    {
        await this.weaponRepository.AddSkin(4);
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerWeaponSkins.Should()
            .ContainEquivalentOf(
                new DbWeaponSkin()
                {
                    ViewerId = IdentityTestUtils.ViewerId,
                    WeaponSkinId = 4,
                    IsNew = false,
                    GetTime = DateTimeOffset.UtcNow
                }
            );
    }

    [Fact]
    public async Task AddSkin_DuplicateSkins_NoPkException()
    {
        await this.weaponRepository.AddSkin(6);
        await this.fixture.ApiContext.SaveChangesAsync();

        Func<Task> act = async () =>
        {
            await this.weaponRepository.AddSkin(6);
            await this.fixture.ApiContext.SaveChangesAsync();
        };

        await act.Invoking(x => x.Invoke()).Should().NotThrowAsync();
    }
}
