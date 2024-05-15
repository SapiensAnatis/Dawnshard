using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Database.Test;
using DragaliaAPI.Features.Present;
using DragaliaAPI.Features.Reward;
using DragaliaAPI.Features.Reward.Handlers;
using DragaliaAPI.Features.Summoning;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Test.Features.Summon;

[Collection("RepositoryTest")]
public class UnitServiceTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly UnitService unitService;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;
    private readonly Mock<IPresentService> mockPresentService;

    public UnitServiceTest(DbTestFixture fixture)
    {
        this.fixture = fixture;

        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService.Setup(x => x.ViewerId).Returns(ViewerId);
        this.mockPresentService = new(MockBehavior.Loose);

        CharaHandler charaHandler =
            new(
                this.fixture.ApiContext,
                this.mockPlayerIdentityService.Object,
                NullLogger<CharaHandler>.Instance
            );
        DragonHandler dragonHandler =
            new(
                this.fixture.ApiContext,
                this.mockPlayerIdentityService.Object,
                NullLogger<DragonHandler>.Instance
            );
        RewardService rewardService =
            new(
                NullLogger<RewardService>.Instance,
                new Mock<IUnitRepository>().Object,
                [],
                [charaHandler, dragonHandler]
            );

        this.unitService = new UnitService(
            this.mockPresentService.Object,
            rewardService,
            fixture.ApiContext
        );

        this.fixture.ApiContext.PlayerCharaData.RemoveRange(
            this.fixture.ApiContext.PlayerCharaData.IgnoreQueryFilters()
        );
        this.fixture.ApiContext.PlayerDragonData.RemoveRange(
            this.fixture.ApiContext.PlayerDragonData.IgnoreQueryFilters()
        );
        this.fixture.ApiContext.PlayerDragonReliability.RemoveRange(
            this.fixture.ApiContext.PlayerDragonReliability.IgnoreQueryFilters()
        );
        this.fixture.ApiContext.PlayerStoryState.RemoveRange(
            this.fixture.ApiContext.PlayerStoryState.IgnoreQueryFilters()
        );
        this.fixture.ApiContext.SaveChanges();

        this.fixture.ApiContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task AddCharas_CorrectlyMarksDuplicates()
    {
        List<Charas> idList =
            new() { Charas.Chrom, Charas.Chrom, Charas.Panther, Charas.Izumo, Charas.Izumo };

        (await this.unitService.AddCharas(idList))
            .Where(x => x.IsNew)
            .Select(x => x.Id)
            .Should()
            .BeEquivalentTo(new List<Charas>() { Charas.Chrom, Charas.Panther, Charas.Izumo });
    }

    [Fact]
    public async Task AddCharas_UpdatesDatabase()
    {
        List<Charas> idList = new() { Charas.Addis, Charas.Aeleen };

        await this.unitService.AddCharas(idList);
        await this.fixture.ApiContext.SaveChangesAsync();

        (
            await this
                .fixture.ApiContext.PlayerCharaData.Where(x => x.ViewerId == ViewerId)
                .Select(x => x.CharaId)
                .ToListAsync()
        )
            .Should()
            .Contain(new List<Charas>() { Charas.Addis, Charas.Aeleen });
        (await this.fixture.ApiContext.PlayerStoryState.Select(x => x.StoryId).ToListAsync())
            .Should()
            .Contain(
                new List<int>()
                {
                    MasterAsset.CharaStories[(int)Charas.Addis].StoryIds[0],
                    MasterAsset.CharaStories[(int)Charas.Aeleen].StoryIds[0]
                }
            );
    }

    [Fact]
    public async Task AddCharas_HandlesExistingStory()
    {
        int natalieStoryId = MasterAsset.CharaStories[(int)Charas.Natalie].StoryIds[0];
        int catherineStoryId = MasterAsset.CharaStories[(int)Charas.Catherine].StoryIds[0];
        await this.fixture.AddRangeToDatabase(
            [
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryType = StoryTypes.Chara,
                    StoryId = natalieStoryId,
                    State = 0,
                },
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryType = StoryTypes.Dragon,
                    StoryId = catherineStoryId,
                    State = 0,
                },
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId + 1,
                    StoryType = StoryTypes.Chara,
                    StoryId = catherineStoryId,
                    State = 0,
                }
            ]
        );

        List<Charas> idList = [Charas.Natalie, Charas.Catherine];

        await this.unitService.AddCharas(idList);

        await this.fixture.ApiContext.SaveChangesAsync();

        this.fixture.ApiContext.PlayerStoryState.Should()
            .ContainEquivalentOf(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryType = StoryTypes.Chara,
                    StoryId = natalieStoryId,
                    State = 0,
                },
                opts => opts.Excluding(x => x.Owner)
            )
            .And.ContainEquivalentOf(
                new DbPlayerStoryState()
                {
                    ViewerId = ViewerId,
                    StoryType = StoryTypes.Chara,
                    StoryId = catherineStoryId,
                    State = 0,
                },
                opts => opts.Excluding(x => x.Owner)
            );
    }

    [Fact]
    public async Task AddDragons_CorrectlyMarksDuplicates()
    {
        await this.fixture.AddToDatabase(new DbPlayerDragonData(ViewerId, Dragons.Barbatos));

        List<Dragons> idList = new() { Dragons.Marishiten, Dragons.Barbatos, Dragons.Marishiten };

        IEnumerable<(Dragons Id, bool IsNew)> result = await this.unitService.AddDragons(idList);

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

        await this.unitService.AddDragons(idList);
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
        await this.fixture.AddToDatabase(new DbPlayerDragonData(ViewerId, Dragons.KonohanaSakuya));
        await this.fixture.AddToDatabase(
            new DbPlayerDragonReliability(ViewerId, Dragons.KonohanaSakuya)
        );

        List<Dragons> idList = new() { Dragons.KonohanaSakuya, Dragons.Michael, Dragons.Michael };

        await this.unitService.AddDragons(idList);
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
