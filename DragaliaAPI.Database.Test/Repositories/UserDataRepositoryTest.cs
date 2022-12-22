using DragaliaAPI.Database.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class UserDataRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly IUserDataRepository userDataRepository;

    public UserDataRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;

        this.userDataRepository = new UserDataRepository(this.fixture.ApiContext);
    }

    [Fact]
    public async Task GetPlayerInfo_ValidId_ReturnsInfo()
    {
        (await this.userDataRepository.GetUserData("id").ToListAsync()).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPlayerInfo_InvalidId_ReturnsEmptyQueryable()
    {
        (await this.userDataRepository.GetUserData("wrong id").ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateTutorialStatus_UpdatesTutorialStatus()
    {
        await this.userDataRepository.UpdateTutorialStatus("id", 200);
        await this.userDataRepository.SaveChangesAsync();

        this.fixture.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == "id")
            .TutorialStatus.Should()
            .Be(200);
    }

    [Fact]
    public async Task UpdateName_UpdatesName()
    {
        await this.userDataRepository.UpdateName("id", "Euden 2");
        await this.userDataRepository.SaveChangesAsync();

        this.fixture.ApiContext.PlayerUserData
            .Single(x => x.DeviceAccountId == "id")
            .Name.Should()
            .Be("Euden 2");
    }
}
