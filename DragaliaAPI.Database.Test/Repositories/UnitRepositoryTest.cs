using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Entities.Scaffold;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class UnitRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IUnitRepository unitRepository;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;

    public UnitRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService.Setup(x => x.AccountId).Returns(DeviceAccountId);

        this.unitRepository = new UnitRepository(
            fixture.ApiContext,
            this.mockPlayerIdentityService.Object,
            LoggerTestUtils.Create<UnitRepository>()
        );
    }

    [Fact]
    public async Task GetAllCharaData_ValidId_ReturnsData()
    {
        (await this.unitRepository.Charas.ToListAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_InvalidId_ReturnsEmpty()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("wrong id");

        (await this.unitRepository.Charas.ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_ReturnsOnlyDataForGivenId()
    {
        await this.fixture.AddToDatabase(new DbPlayerCharaData("other id", Charas.Ilia));

        (await this.unitRepository.Charas.ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be(DeviceAccountId));
    }

    [Fact]
    public async Task GetAllDragonata_ValidId_ReturnsData()
    {
        await this.fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Agni)
        );
        (await this.unitRepository.Dragons.ToListAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_InvalidId_ReturnsEmpty()
    {
        this.mockPlayerIdentityService.SetupGet(x => x.AccountId).Returns("wrong id");

        (await this.unitRepository.Dragons.ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_ReturnsOnlyDataForGivenId()
    {
        (await this.unitRepository.Charas.ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be(DeviceAccountId));
    }

    [Fact]
    public async Task CheckHasCharas_OwnedList_ReturnsTrue()
    {
        IEnumerable<Charas> idList = await fixture.ApiContext.PlayerCharaData
            .Where(x => x.DeviceAccountId == DeviceAccountId)
            .Select(x => x.CharaId)
            .ToListAsync();

        (await this.unitRepository.CheckHasCharas(idList)).Should().BeTrue();
    }

    [Fact]
    public async Task CheckHasCharas_NotAllOwnedList_ReturnsFalse()
    {
        IEnumerable<Charas> idList = (
            await fixture.ApiContext.PlayerCharaData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .Select(x => x.CharaId)
                .ToListAsync()
        ).Append(Charas.BondforgedZethia);

        (await this.unitRepository.CheckHasCharas(idList)).Should().BeFalse();
    }

    [Fact]
    public async Task CheckHasDragons_OwnedList_ReturnsTrue()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.AC011Garland)
        );
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Ariel)
        );

        List<Dragons> idList = new() { Dragons.AC011Garland, Dragons.Ariel };

        (await this.unitRepository.CheckHasDragons(idList)).Should().BeTrue();
    }

    [Fact]
    public async Task CheckHasDragons_NotAllOwnedList_ReturnsFalse()
    {
        (await this.unitRepository.CheckHasDragons(new List<Dragons>() { Dragons.BronzeFafnir }))
            .Should()
            .BeFalse();
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
            await this.fixture.ApiContext.PlayerCharaData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
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
    public async Task AddDragons_CorrectlyMarksDuplicates()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Barbatos)
        );

        List<Dragons> idList = new() { Dragons.Marishiten, Dragons.Barbatos, Dragons.Marishiten };

        IEnumerable<(Dragons id, bool isNew)> result = await this.unitRepository.AddDragons(idList);

        result
            .Where(x => x.isNew)
            .Select(x => x.id)
            .Should()
            .BeEquivalentTo(new List<Dragons>() { Dragons.Marishiten });
    }

    [Fact]
    public async Task AddDragons_UpdatesDatabase()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.KonohanaSakuya)
        );
        await fixture.AddToDatabase(
            DbPlayerDragonReliabilityFactory.Create(DeviceAccountId, Dragons.KonohanaSakuya)
        );

        List<Dragons> idList = new() { Dragons.KonohanaSakuya, Dragons.Michael, Dragons.Michael };

        await this.unitRepository.AddDragons(idList);
        await this.fixture.ApiContext.SaveChangesAsync();

        (
            await this.fixture.ApiContext.PlayerDragonData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
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
            await this.fixture.ApiContext.PlayerDragonReliability
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .Select(x => x.DragonId)
                .ToListAsync()
        )
            .Should()
            .Contain(new List<Dragons>() { Dragons.KonohanaSakuya, Dragons.Michael, });
    }
}
