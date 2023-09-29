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
        string roomName = Guid.NewGuid().ToString();

        DbTimeAttackClear clear =
            new()
            {
                RoomName = roomName,
                QuestId = 1,
                Players = new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        RoomName = roomName,
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
        string roomName = Guid.NewGuid().ToString();

        await this.timeAttackRepository.CreateOrUpdateClear(
            new()
            {
                RoomName = roomName,
                QuestId = 1,
                Players = new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        RoomName = roomName,
                        DeviceAccountId = "id 2",
                        PartyInfo = "{}"
                    }
                }
            }
        );

        await this.ApiContext.SaveChangesAsync();

        await this.timeAttackRepository.CreateOrUpdateClear(
            new()
            {
                RoomName = roomName,
                QuestId = 1,
                Players = new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        RoomName = roomName,
                        DeviceAccountId = "id 3",
                        PartyInfo = "{}"
                    }
                }
            }
        );

        await this.ApiContext.SaveChangesAsync();

        this.ApiContext.TimeAttackClears.Should().Contain(x => x.RoomName == roomName);
        this.ApiContext.TimeAttackClears
            .First(x => x.RoomName == roomName)
            .Players.Should()
            .BeEquivalentTo(
                new List<DbTimeAttackPlayer>()
                {
                    new()
                    {
                        RoomName = roomName,
                        DeviceAccountId = "id 2",
                        PartyInfo = "{}"
                    },
                    new()
                    {
                        RoomName = roomName,
                        DeviceAccountId = "id 3",
                        PartyInfo = "{}"
                    }
                },
                opts => opts.Excluding(x => x.Clear).Excluding(x => x.Player)
            );
    }
}
