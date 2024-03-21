using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Features.TimeAttack;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Test.Features.TimeAttack;

public class TimeAttackRepositoryTest : RepositoryTestFixture
{
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly ITimeAttackRepository timeAttackRepository;

    public TimeAttackRepositoryTest()
    {
        this.mockPlayerIdentityService = new Mock<IPlayerIdentityService>(MockBehavior.Strict);
        this.timeAttackRepository = new TimeAttackRepository(
            this.ApiContext,
            this.mockPlayerIdentityService.Object
        );
    }

    [Fact]
    public async Task CreateOrUpdateClear_CreatesNew()
    {
        string gameId = Guid.NewGuid().ToString();

        DbTimeAttackClear clear =
            new()
            {
                GameId = gameId,
                QuestId = 1,
                Players = new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        GameId = gameId,
                        ViewerId = 1,
                        PartyInfo = "{}"
                    }
                }
            };

        await this.timeAttackRepository.CreateOrUpdateClear(clear);
        await this.ApiContext.SaveChangesAsync();

        this.ApiContext.TimeAttackClears.Should().ContainEquivalentOf(clear);
    }

    [Fact]
    public async Task CreateOrUpdateClear_UpdatesExisting()
    {
        string gameId = Guid.NewGuid().ToString();

        await this.timeAttackRepository.CreateOrUpdateClear(
            new()
            {
                GameId = gameId,
                QuestId = 1,
                Players = new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        GameId = gameId,
                        ViewerId = 2,
                        PartyInfo = "{}"
                    }
                }
            }
        );

        await this.ApiContext.SaveChangesAsync();

        await this.timeAttackRepository.CreateOrUpdateClear(
            new()
            {
                GameId = gameId,
                QuestId = 1,
                Players = new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        GameId = gameId,
                        ViewerId = 3,
                        PartyInfo = "{}"
                    }
                }
            }
        );

        await this.ApiContext.SaveChangesAsync();

        this.ApiContext.TimeAttackClears.Should().Contain(x => x.GameId == gameId);
        this.ApiContext.TimeAttackClears.First(x => x.GameId == gameId)
            .Players.Should()
            .BeEquivalentTo(
                new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        GameId = gameId,
                        ViewerId = 2,
                        PartyInfo = "{}"
                    },
                    new()
                    {
                        GameId = gameId,
                        ViewerId = 3,
                        PartyInfo = "{}"
                    }
                },
                opts => opts.Excluding(x => x.Clear).Excluding(x => x.Player)
            );
    }
}
