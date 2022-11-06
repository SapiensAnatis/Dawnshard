using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Test.Repositories;

public class DeviceAccountRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;

    private readonly IDeviceAccountRepository deviceAccountRepository;
    private readonly ICharaDataService charaDataService;

    public DeviceAccountRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;

        this.charaDataService = new CharaDataService();

        deviceAccountRepository = new DeviceAccountRepository(
            fixture.ApiContext,
            this.charaDataService
        );
    }

    [Fact]
    public async Task AddNewDeviceAccount_CanGetAfterwards()
    {
        await deviceAccountRepository.AddNewDeviceAccount("id 2", "hashed password 2");
        DbDeviceAccount? result = await deviceAccountRepository.GetDeviceAccountById("id 2");

        result.Should().NotBeNull();
        result!.Id.Should().Be("id 2");
        result!.HashedPassword.Should().Be("hashed password 2");
    }

    [Fact]
    public async Task GetDeviceAccountById_InvalidId_ReturnsNull()
    {
        DbDeviceAccount? result = await deviceAccountRepository.GetDeviceAccountById("wrong id");

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateNewSavefile_HasExpectedProperties()
    {
        string newId = "id 2";
        await this.deviceAccountRepository.CreateNewSavefile(newId);

        (
            await this.fixture.ApiContext.PlayerUserData.SingleOrDefaultAsync(
                x => x.DeviceAccountId == newId
            )
        )
            .Should()
            .NotBeNull();

        (
            await this.fixture.ApiContext.PlayerCharaData.SingleOrDefaultAsync(
                x => x.DeviceAccountId == newId && x.CharaId == Charas.ThePrince
            )
        )
            .Should()
            .NotBeNull();

        (
            await this.fixture.ApiContext.PlayerParties
                .Where(x => x.DeviceAccountId == newId)
                .ToListAsync()
        )
            .Should()
            .HaveCount(54);
    }
}
