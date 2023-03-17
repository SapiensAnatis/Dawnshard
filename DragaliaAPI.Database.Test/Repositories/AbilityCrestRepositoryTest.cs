using System.Security.Cryptography;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Test.Utils;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class AbilityCrestRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IAbilityCrestRepository abilityCrestRepository;

    public AbilityCrestRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;

        this.abilityCrestRepository = new AbilityCrestRepository(
            this.fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object,
            LoggerTestUtils.Create<AbilityCrestRepository>()
        );

        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task Add_AddsToDatabase()
    {
        await this.abilityCrestRepository.Add(AbilityCrests.ADogsDay);
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerAbilityCrests
            .Single(
                x =>
                    x.AbilityCrestId == AbilityCrests.ADogsDay
                    && x.DeviceAccountId == IdentityTestUtils.DeviceAccountId
            )
            .Should()
            .BeEquivalentTo(
                new DbAbilityCrest()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    AbilityCrestId = AbilityCrests.ADogsDay
                }
            );
    }

    [Fact]
    public async Task FindAsync_FindsAbilityCrestAsExpected()
    {
        await this.abilityCrestRepository.Add(AbilityCrests.FlashofGenius);
        await this.fixture.ApiContext.SaveChangesAsync();

        (await this.abilityCrestRepository.FindAsync(AbilityCrests.FlashofGenius))
            .Should()
            .BeEquivalentTo(
                new DbAbilityCrest()
                {
                    DeviceAccountId = IdentityTestUtils.DeviceAccountId,
                    AbilityCrestId = AbilityCrests.FlashofGenius
                }
            );

        (await this.abilityCrestRepository.FindAsync(AbilityCrests.TheBridalDragon))
            .Should()
            .BeNull();
    }
}
