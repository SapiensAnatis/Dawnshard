using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Microsoft.Extensions.Logging;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class AbilityCrestRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly AbilityCrestRepository abilityCrestRepository;
    private readonly Mock<ILogger<AbilityCrestRepository>> logger;

    public AbilityCrestRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.logger = new Mock<ILogger<AbilityCrestRepository>>();

        this.abilityCrestRepository = new AbilityCrestRepository(
            this.fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object,
            this.logger.Object
        );

        CommonAssertionOptions.ApplyTimeOptions();
        CommonAssertionOptions.ApplyIgnoreOwnerOptions();
    }

    [Fact]
    public async Task Add_AddsToDatabase()
    {
        await this.abilityCrestRepository.Add(AbilityCrests.ADogsDay);
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerAbilityCrests.Single(x =>
            x.AbilityCrestId == AbilityCrests.ADogsDay && x.ViewerId == IdentityTestUtils.ViewerId
        )
            .Should()
            .BeEquivalentTo(
                new DbAbilityCrest()
                {
                    ViewerId = IdentityTestUtils.ViewerId,
                    AbilityCrestId = AbilityCrests.ADogsDay
                }
            );
    }

    [Fact]
    public async Task Add_AbilityCrestAlreadyExistsWarnsLogger()
    {
        await this.abilityCrestRepository.Add(AbilityCrests.ADragonyuleforIlia);
        await this.fixture.ApiContext.SaveChangesAsync();

        await this.abilityCrestRepository.Add(AbilityCrests.ADragonyuleforIlia);
        this.logger.Verify(
            x =>
                x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(
                        (o, t) =>
                            string.Equals(
                                "Ability crest was already owned.",
                                o.ToString(),
                                StringComparison.InvariantCultureIgnoreCase
                            )
                    ),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
            Times.Once
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
                    ViewerId = IdentityTestUtils.ViewerId,
                    AbilityCrestId = AbilityCrests.FlashofGenius
                }
            );

        (await this.abilityCrestRepository.FindAsync(AbilityCrests.TheBridalDragon))
            .Should()
            .BeNull();
    }

    [Fact]
    public async Task AddOrUpdateSet_AddsWhenNonexistentAndUpdatesWhenExists()
    {
        await this.abilityCrestRepository.AddOrUpdateSet(
            new DbAbilityCrestSet(IdentityTestUtils.ViewerId, 54)
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerAbilityCrestSets.Single(x =>
            x.ViewerId == IdentityTestUtils.ViewerId && x.AbilityCrestSetNo == 54
        )
            .Should()
            .BeEquivalentTo(new DbAbilityCrestSet(IdentityTestUtils.ViewerId, 54));

        await this.abilityCrestRepository.AddOrUpdateSet(
            new DbAbilityCrestSet()
            {
                ViewerId = IdentityTestUtils.ViewerId,
                AbilityCrestSetNo = 54,
                CrestSlotType1CrestId1 = AbilityCrests.WorthyRivals
            }
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerAbilityCrestSets.Single(x =>
            x.ViewerId == IdentityTestUtils.ViewerId && x.AbilityCrestSetNo == 54
        )
            .Should()
            .BeEquivalentTo(
                new DbAbilityCrestSet()
                {
                    ViewerId = IdentityTestUtils.ViewerId,
                    AbilityCrestSetNo = 54,
                    CrestSlotType1CrestId1 = AbilityCrests.WorthyRivals
                }
            );
    }

    [Fact]
    public async Task FindSetAsync_FindsAbilityCrestSetAsExpected()
    {
        await this.abilityCrestRepository.AddOrUpdateSet(
            new DbAbilityCrestSet(IdentityTestUtils.ViewerId, 1)
        );
        await this.fixture.ApiContext.SaveChangesAsync();

        (await this.abilityCrestRepository.FindSetAsync(1))
            .Should()
            .BeEquivalentTo(new DbAbilityCrestSet(IdentityTestUtils.ViewerId, 1));

        (await this.abilityCrestRepository.FindSetAsync(2)).Should().BeNull();
    }
}
