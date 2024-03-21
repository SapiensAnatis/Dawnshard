using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class UnitRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly UnitRepository unitRepository;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;

    public UnitRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.fixture.ApiContext.Database.EnsureCreated();
        this.fixture.ApiContext.Database.EnsureDeleted();

        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService.Setup(x => x.ViewerId).Returns(ViewerId);

        this.unitRepository = new UnitRepository(
            fixture.ApiContext,
            this.mockPlayerIdentityService.Object,
            LoggerTestUtils.Create<UnitRepository>()
        );

        this.fixture.ApiContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task GetAllCharaData_ValidId_ReturnsData()
    {
        await this.fixture.AddToDatabase(new DbPlayerCharaData(1, Charas.Akasha));

        (await this.unitRepository.Charas.ToListAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_InvalidId_ReturnsEmpty()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(400);

        await this.fixture.AddToDatabase(DbPlayerDragonDataFactory.Create(1, Dragons.Nyarlathotep));

        (await this.unitRepository.Charas.ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_ReturnsOnlyDataForGivenId()
    {
        await this.fixture.AddToDatabase(new DbPlayerCharaData(1, Charas.Ilia));

        (await this.unitRepository.Charas.ToListAsync())
            .Should()
            .AllSatisfy(x => x.ViewerId.Should().Be(ViewerId));
    }

    [Fact]
    public async Task GetAllDragonData_ValidId_ReturnsData()
    {
        await this.fixture.AddToDatabase(DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Agni));
        (await this.unitRepository.Dragons.ToListAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_InvalidId_ReturnsEmpty()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId).Returns(400);

        (await this.unitRepository.Dragons.ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_ReturnsOnlyDataForGivenId()
    {
        await this.fixture.AddToDatabase(new DbPlayerCharaData(ViewerId, Charas.Ilia));
        await this.fixture.AddToDatabase(new DbPlayerCharaData(244, Charas.Ilia));

        (await this.unitRepository.Charas.ToListAsync())
            .Should()
            .AllSatisfy(x => x.ViewerId.Should().Be(ViewerId));
    }

    [Fact]
    public async Task AddCharas_CorrectlyMarksDuplicates()
    {
        List<Charas> idList =
            new() { Charas.Chrom, Charas.Chrom, Charas.Panther, Charas.Izumo, Charas.Izumo };

        (await this.unitRepository.AddCharas(idList))
            .Where(x => x.isNew)
            .Select(x => x.id)
            .Should()
            .BeEquivalentTo(new List<Charas>() { Charas.Chrom, Charas.Panther, Charas.Izumo });
    }

    [Fact]
    public async Task AddCharas_UpdatesDatabase()
    {
        List<Charas> idList = new() { Charas.Addis, Charas.Aeleen };

        await this.unitRepository.AddCharas(idList);
        await this.fixture.ApiContext.SaveChangesAsync();

        (
            await this
                .fixture.ApiContext.PlayerCharaData.Where(x => x.ViewerId == ViewerId)
                .Select(x => x.CharaId)
                .ToListAsync()
        )
            .Should()
            .Contain(new List<Charas>() { Charas.Addis, Charas.Aeleen });
        (await fixture.ApiContext.PlayerStoryState.Select(x => x.StoryId).ToListAsync())
            .Should()
            .Contain(
                new List<int>()
                {
                    MasterAsset.CharaStories[(int)Charas.Addis].storyIds[0],
                    MasterAsset.CharaStories[(int)Charas.Aeleen].storyIds[0]
                }
            );
    }

    [Fact]
    public async Task AddCharas_HandlesExistingStory()
    {
        int izumoStoryId = MasterAsset.CharaStories[(int)Charas.Izumo].storyIds[0];
        int mitsuhideStoryId = MasterAsset.CharaStories[(int)Charas.Mitsuhide].storyIds[0];
        await this.fixture.AddRangeToDatabase(
            [
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryType = StoryTypes.Chara,
                    StoryId = izumoStoryId,
                    State = 0,
                },
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryType = StoryTypes.Dragon,
                    StoryId = mitsuhideStoryId,
                    State = 0,
                },
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId + 1,
                    StoryType = StoryTypes.Chara,
                    StoryId = mitsuhideStoryId,
                    State = 0,
                }
            ]
        );

        List<Charas> idList = [Charas.Izumo, Charas.Mitsuhide];

        await this.unitRepository.AddCharas(idList);

        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerStoryState.Should()
            .ContainEquivalentOf(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryType = StoryTypes.Chara,
                    StoryId = izumoStoryId,
                    State = 0,
                },
                opts => opts.Excluding(x => x.Owner)
            )
            .And.ContainEquivalentOf(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryType = StoryTypes.Chara,
                    StoryId = mitsuhideStoryId,
                    State = 0,
                },
                opts => opts.Excluding(x => x.Owner)
            );
    }

    [Fact]
    public async Task AddDragons_CorrectlyMarksDuplicates()
    {
        await fixture.AddToDatabase(DbPlayerDragonDataFactory.Create(ViewerId, Dragons.Barbatos));

        List<Dragons> idList = new() { Dragons.Marishiten, Dragons.Barbatos, Dragons.Marishiten };

        IEnumerable<(Dragons Id, bool IsNew)> result = await this.unitRepository.AddDragons(idList);

        result
            .Where(x => x.IsNew)
            .Select(x => x.Id)
            .Should()
            .BeEquivalentTo(new List<Dragons>() { Dragons.Marishiten });
    }

    [Fact]
    public async Task AddDragons_HandlesExistingReliability()
    {
        await this.fixture.AddRangeToDatabase(
            [
                new DbPlayerDragonReliability()
                {
                    ViewerId = ViewerId,
                    DragonId = Dragons.AC011Garland,
                },
                new DbPlayerDragonReliability()
                {
                    ViewerId = ViewerId + 1,
                    DragonId = Dragons.Agni,
                },
            ]
        );

        List<Dragons> idList = [Dragons.AC011Garland, Dragons.Agni];

        await this.unitRepository.AddDragons(idList);
        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerDragonReliability.Should()
            .ContainEquivalentOf(
                new DbPlayerDragonReliability() { ViewerId = ViewerId, DragonId = Dragons.Agni, },
                opts => opts.Including(x => x.ViewerId).Including(x => x.DragonId)
            );
    }

    [Fact]
    public async Task AddDragons_UpdatesDatabase()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(ViewerId, Dragons.KonohanaSakuya)
        );
        await fixture.AddToDatabase(
            DbPlayerDragonReliabilityFactory.Create(ViewerId, Dragons.KonohanaSakuya)
        );

        List<Dragons> idList = new() { Dragons.KonohanaSakuya, Dragons.Michael, Dragons.Michael };

        await this.unitRepository.AddDragons(idList);
        await this.fixture.ApiContext.SaveChangesAsync();

        (
            await this
                .fixture.ApiContext.PlayerDragonData.Where(x => x.ViewerId == ViewerId)
                .Select(x => x.DragonId)
                .ToListAsync()
        )
            .Should()
            .Contain(
                new List<Dragons>()
                {
                    Dragons.KonohanaSakuya,
                    Dragons.KonohanaSakuya,
                    Dragons.Michael,
                    Dragons.Michael
                }
            );

        (
            await this
                .fixture.ApiContext.PlayerDragonReliability.Where(x => x.ViewerId == ViewerId)
                .Select(x => x.DragonId)
                .ToListAsync()
        )
            .Should()
            .Contain(new List<Dragons>() { Dragons.KonohanaSakuya, Dragons.Michael, });
    }
}
