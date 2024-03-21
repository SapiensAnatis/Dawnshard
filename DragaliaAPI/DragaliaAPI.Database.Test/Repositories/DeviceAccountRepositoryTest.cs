using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
[Obsolete(ObsoleteReasons.BaaS)]
public class DeviceAccountRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DeviceAccountRepository deviceAccountRepository;

    public DeviceAccountRepositoryTest(DbTestFixture fixture)
    {
        deviceAccountRepository = new DeviceAccountRepository(fixture.ApiContext);
    }

    [Fact]
    public async Task AddNewDeviceAccount_CanGetAfterwards()
    {
        await deviceAccountRepository.AddNewDeviceAccount("id 2", "hashed password 2");
        await deviceAccountRepository.SaveChangesAsync();

        DbDeviceAccount? result = await deviceAccountRepository.GetDeviceAccountById("id 2");

        result.Should().NotBeNull();
        result!.Id.Should().Be("id 2");
        result.HashedPassword.Should().Be("hashed password 2");
    }

    [Fact]
    public async Task GetDeviceAccountById_InvalidId_ReturnsNull()
    {
        DbDeviceAccount? result = await deviceAccountRepository.GetDeviceAccountById("wrong id");

        result.Should().BeNull();
    }
}
