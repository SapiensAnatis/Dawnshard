using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Factories;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;
using Microsoft.EntityFrameworkCore;
using static DragaliaAPI.Database.Test.DbTestFixture;

namespace DragaliaAPI.Database.Test.Repositories;

public class UnitRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly ICharaDataService charaDataService;
    private readonly IUnitRepository unitRepository;

    public UnitRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.charaDataService = new CharaDataService();
        this.unitRepository = new UnitRepository(fixture.ApiContext, this.charaDataService);
    }

    [Fact]
    public async Task GetAllCharaData_ValidId_ReturnsData()
    {
        (await this.unitRepository.GetAllCharaData(DeviceAccountId).ToListAsync())
            .Should()
            .NotBeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_InvalidId_ReturnsEmpty()
    {
        (await this.unitRepository.GetAllCharaData("wrong id").ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllCharaData_ReturnsOnlyDataForGivenId()
    {
        await this.fixture.AddToDatabase(
            Factories.DbPlayerCharaDataFactory.Create(
                "other id",
                this.charaDataService.GetData(Charas.Ilia)
            )
        );

        (await this.unitRepository.GetAllCharaData(DeviceAccountId).ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be(DeviceAccountId));
    }

    [Fact]
    public async Task GetAllDragonata_ValidId_ReturnsData()
    {
        await this.fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Agni)
        );
        (await this.unitRepository.GetAllDragonData(DeviceAccountId).ToListAsync())
            .Should()
            .NotBeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_InvalidId_ReturnsEmpty()
    {
        (await this.unitRepository.GetAllDragonData("wrong id").ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDragonData_ReturnsOnlyDataForGivenId()
    {
        await this.fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Andromeda)
        );

        await this.fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create("other id", Dragons.Apollo)
        );

        (await this.unitRepository.GetAllCharaData(DeviceAccountId).ToListAsync())
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

        (await this.unitRepository.CheckHasCharas(DeviceAccountId, idList)).Should().BeTrue();
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

        (await this.unitRepository.CheckHasCharas(DeviceAccountId, idList)).Should().BeFalse();
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

        (await this.unitRepository.CheckHasDragons(DeviceAccountId, idList)).Should().BeTrue();
    }

    [Fact]
    public async Task CheckHasDragons_NotAllOwnedList_ReturnsFalse()
    {
        (
            await this.unitRepository.CheckHasDragons(
                DeviceAccountId,
                new List<Dragons>() { Dragons.BronzeFafnir }
            )
        )
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task AddCharas_CorrectlyMarksDuplicates()
    {
        List<Charas> idList = new() { Charas.ThePrince, Charas.Chrom, Charas.Chrom };

        (await this.unitRepository.AddCharas(DeviceAccountId, idList))
            .Select(x => x.CharaId)
            .Should()
            .BeEquivalentTo(new List<Charas>() { Charas.Chrom });
    }

    [Fact]
    public async Task AddCharas_UpdatesDatabase()
    {
        List<Charas> idList = new() { Charas.Addis, Charas.Aeleen };

        await this.unitRepository.AddCharas(DeviceAccountId, idList);

        (
            await this.fixture.ApiContext.PlayerCharaData
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .Select(x => x.CharaId)
                .ToListAsync()
        )
            .Should()
            .Contain(new List<Charas>() { Charas.Addis, Charas.Aeleen });
    }

    [Fact]
    public async Task AddDragons_CorrectlyMarksDuplicates()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.Barbatos)
        );

        List<Dragons> idList = new() { Dragons.Marishiten, Dragons.Barbatos, Dragons.Marishiten };

        (
            IEnumerable<DbPlayerDragonData> newDragons,
            IEnumerable<DbPlayerDragonReliability> newReliabilities
        ) = await this.unitRepository.AddDragons(DeviceAccountId, idList);

        newDragons
            .Select(x => x.DragonId)
            .Should()
            .BeEquivalentTo(
                new List<Dragons>() { Dragons.Marishiten, Dragons.Barbatos, Dragons.Marishiten, }
            );

        newReliabilities
            .Select(x => x.DragonId)
            .Should()
            .BeEquivalentTo(new List<Dragons>() { Dragons.Marishiten });
    }

    [Fact]
    public async Task AddDragons_UpdatesDatabase()
    {
        await fixture.AddToDatabase(
            DbPlayerDragonDataFactory.Create(DeviceAccountId, Dragons.GalaRebornAgni)
        );
        await fixture.AddToDatabase(
            DbPlayerDragonReliabilityFactory.Create(DeviceAccountId, Dragons.GalaRebornAgni)
        );

        List<Dragons> idList =
            new() { Dragons.GalaRebornAgni, Dragons.GalaRebornZephyr, Dragons.GalaRebornZephyr };

        await this.unitRepository.AddDragons(DeviceAccountId, idList);

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
                    Dragons.GalaRebornAgni,
                    Dragons.GalaRebornAgni,
                    Dragons.GalaRebornZephyr,
                    Dragons.GalaRebornZephyr
                }
            );

        (
            await this.fixture.ApiContext.PlayerDragonReliability
                .Where(x => x.DeviceAccountId == DeviceAccountId)
                .Select(x => x.DragonId)
                .ToListAsync()
        )
            .Should()
            .Contain(new List<Dragons>() { Dragons.GalaRebornAgni, Dragons.GalaRebornZephyr, });
    }
}
