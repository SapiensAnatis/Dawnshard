using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class PartyRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly PartyRepository partyRepository;

    public PartyRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.partyRepository = new PartyRepository(
            this.fixture.ApiContext,
            IdentityTestUtils.MockPlayerDetailsService.Object
        );

        AssertionOptions.AssertEquivalencyUsing(options =>
            options.Excluding(x => x.Name == "Owner")
        );
    }

    [Fact]
    public async Task GetParties_Returns54Entries_AndPartyUnitsAreOrdered()
    {
        IEnumerable<DbParty> result = await this.partyRepository.Parties.ToListAsync();

        result.Should().HaveCount(54);

        IEnumerable<IEnumerable<DbPartyUnit>> allUnits = result.Select(x => x.Units);
        allUnits.Should().AllSatisfy(x => x.Select(y => y.UnitNo).Should().BeInAscendingOrder());
    }

    [Fact]
    public async Task SetParty_UpdatesDatabase()
    {
        DbParty toAdd =
            new()
            {
                ViewerId = ViewerId,
                PartyName = "New Name",
                PartyNo = 3,
                Units = new List<DbPartyUnit>()
                {
                    new() { UnitNo = 1, CharaId = Charas.Ieyasu }
                }
            };

        await this.partyRepository.SetParty(toAdd);
        await this.partyRepository.SaveChangesAsync();

        DbParty dbEntry = await this
            .fixture.ApiContext.PlayerParties.Where(x => x.ViewerId == ViewerId && x.PartyNo == 3)
            .Include(x => x.Units)
            .SingleAsync();

        // Units list will be filled with default values
        dbEntry.Should().BeEquivalentTo(toAdd, options => options.Excluding(x => x.Units));
        dbEntry.Units.Where(x => x.UnitNo == 1).Single().CharaId.Should().Be(Charas.Ieyasu);
    }

    [Fact]
    public async Task SetParty_HandlesOverfilledUnitList()
    {
        DbParty toAdd =
            new()
            {
                ViewerId = ViewerId,
                PartyName = "New Name",
                PartyNo = 5,
                Units = new List<DbPartyUnit>()
                {
                    new() { UnitNo = 1, CharaId = Charas.Ieyasu },
                    new() { UnitNo = 1, CharaId = Charas.Addis },
                    new() { UnitNo = 2, CharaId = Charas.Botan },
                    new() { UnitNo = 2, CharaId = Charas.Sazanka },
                    new() { UnitNo = 3, CharaId = Charas.Mitsuhide },
                    new() { UnitNo = 3, CharaId = Charas.Nobunaga },
                    new() { UnitNo = 4, CharaId = Charas.Chitose },
                    new() { UnitNo = 4, CharaId = Charas.Hanabusa },
                }
            };

        await this.partyRepository.SetParty(toAdd);
        await this.partyRepository.SaveChangesAsync();

        DbParty dbEntry = await this
            .fixture.ApiContext.PlayerParties.Where(x => x.ViewerId == ViewerId && x.PartyNo == 5)
            .Include(x => x.Units)
            .SingleAsync();

        dbEntry
            .Units.Select(x => (x.UnitNo, x.CharaId))
            .Should()
            .BeEquivalentTo(
                new List<(int, Charas)>()
                {
                    new(1, Charas.Ieyasu),
                    new(2, Charas.Botan),
                    new(3, Charas.Mitsuhide),
                    new(4, Charas.Chitose)
                }
            );
    }
}
