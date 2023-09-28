using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Test;
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
        DbTimeAttackClear clear =
            new()
            {
                RoomName = "name",
                QuestId = 1,
                Players = new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        RoomName = "name",
                        DeviceAccountId = "id",
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
        await this.timeAttackRepository.CreateOrUpdateClear(
            new()
            {
                RoomName = "name",
                QuestId = 1,
                Players = new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        RoomName = "name",
                        DeviceAccountId = "id",
                        PartyInfo = "{}"
                    }
                }
            }
        );

        await this.ApiContext.SaveChangesAsync();

        await this.timeAttackRepository.CreateOrUpdateClear(
            new()
            {
                RoomName = "name",
                QuestId = 1,
                Players = new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        RoomName = "name",
                        DeviceAccountId = "id 2",
                        PartyInfo = "{}"
                    }
                }
            }
        );

        await this.ApiContext.SaveChangesAsync();

        this.ApiContext.TimeAttackClears.Should().Contain(x => x.RoomName == "name");
        this.ApiContext.TimeAttackClears
            .First(x => x.RoomName == "name")
            .Players.Should()
            .BeEquivalentTo(
                new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        RoomName = "name",
                        DeviceAccountId = "id",
                        PartyInfo = "{}"
                    },
                    new()
                    {
                        RoomName = "name",
                        DeviceAccountId = "id 2",
                        PartyInfo = "{}"
                    }
                },
                opts => opts.Excluding(x => x.Clear).Excluding(x => x.Player)
            );
    }
}
