using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Test.Repositories;

public class WeaponBodyRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IWeaponRepository weaponRepository;

    public WeaponBodyRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.weaponRepository = new WeaponRepository(this.fixture.ApiContext);
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

        (await this.weaponRepository.GetWeaponBodies("id").ToListAsync())
            .Should()
            .AllSatisfy(x => x.DeviceAccountId.Should().Be("id"));
    }
}
