﻿using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Shared.PlayerDetails;
using DragaliaAPI.Test.Utils;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Test.Repositories;

[Collection("RepositoryTest")]
public class UserDataRepositoryTest : IClassFixture<DbTestFixture>
{
    private readonly DbTestFixture fixture;
    private readonly UserDataRepository userDataRepository;
    private readonly Mock<IPlayerIdentityService> mockPlayerIdentityService;

    public UserDataRepositoryTest(DbTestFixture fixture)
    {
        this.fixture = fixture;
        this.mockPlayerIdentityService = new(MockBehavior.Strict);
        this.mockPlayerIdentityService.SetupGet(x => x.ViewerId)
            .Returns(IdentityTestUtils.ViewerId);

        this.userDataRepository = new UserDataRepository(
            this.fixture.ApiContext,
            this.mockPlayerIdentityService.Object,
            LoggerTestUtils.Create<UserDataRepository>()
        );
    }

    [Fact]
    public async Task UpdateName_UpdatesName()
    {
        await this.userDataRepository.UpdateName("Euden 2");
        await this.userDataRepository.SaveChangesAsync();

        this.fixture.ApiContext.PlayerUserData.Single(x => x.ViewerId == IdentityTestUtils.ViewerId)
            .Name.Should()
            .Be("Euden 2");
    }

    [Theory]
    [InlineData(100, true)]
    [InlineData(long.MaxValue, false)]
    public async Task CheckCoin_ReturnsExpectedResult(long checkValue, bool expectedResult)
    {
        DbPlayerUserData userData = (
            await this.fixture.ApiContext.PlayerUserData.FindAsync(
                [IdentityTestUtils.ViewerId],
                TestContext.Current.CancellationToken
            )
        )!;
        userData.Coin = 200;
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        (await this.userDataRepository.CheckCoin(checkValue)).Should().Be(expectedResult);
    }

    [Fact]
    public async Task UpdateCoin_UpdatesCoin()
    {
        long oldCoin = await this
            .fixture.ApiContext.PlayerUserData.Where(x => x.ViewerId == IdentityTestUtils.ViewerId)
            .Select(x => x.Coin)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        await this.userDataRepository.UpdateCoin(2000);
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        long newCoin = await this
            .fixture.ApiContext.PlayerUserData.Where(x => x.ViewerId == IdentityTestUtils.ViewerId)
            .Select(x => x.Coin)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        newCoin.Should().Be(oldCoin + 2000);
    }

    [Fact]
    public async Task UpdateDewpoint_UpdatesDewpoint()
    {
        int oldDewpoint = await this
            .fixture.ApiContext.PlayerUserData.Where(x => x.ViewerId == IdentityTestUtils.ViewerId)
            .Select(x => x.DewPoint)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        await this.userDataRepository.UpdateDewpoint(4000);
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        int newDewpoint = await this
            .fixture.ApiContext.PlayerUserData.Where(x => x.ViewerId == IdentityTestUtils.ViewerId)
            .Select(x => x.DewPoint)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        newDewpoint.Should().Be(oldDewpoint + 4000);
    }

    [Fact]
    public async Task UpdateDewpoint_ThrowsExceptionWhenNegativeDewpoint()
    {
        await this.userDataRepository.SetDewpoint(1000);
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            this.userDataRepository.UpdateDewpoint(-1500)
        );
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        exception.Message.Should().Be("Player cannot have negative eldwater");

        int dewpoint = await this
            .fixture.ApiContext.PlayerUserData.Where(x => x.ViewerId == IdentityTestUtils.ViewerId)
            .Select(x => x.DewPoint)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        dewpoint.Should().Be(1000);
    }

    [Theory]
    [InlineData(500, true)]
    [InlineData(1000, true)]
    [InlineData(1500, false)]
    public async Task CheckDewpoint_ReturnsExpectedBool(int quantity, bool expectedValue)
    {
        await this.userDataRepository.SetDewpoint(1000);
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        bool output = await this.userDataRepository.CheckDewpoint(quantity);
        output.Should().Be(expectedValue);
    }

    [Fact]
    public async Task SetDewpoint_SetsDewpointValue()
    {
        await this.userDataRepository.SetDewpoint(10001);
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        int dewpoint = await this
            .fixture.ApiContext.PlayerUserData.Where(x => x.ViewerId == IdentityTestUtils.ViewerId)
            .Select(x => x.DewPoint)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        dewpoint.Should().Be(10001);
    }

    [Fact]
    public async Task SetDewpoint_ThrowsExceptionWhenNegative()
    {
        int oldDewpoint = await this
            .fixture.ApiContext.PlayerUserData.Where(x => x.ViewerId == IdentityTestUtils.ViewerId)
            .Select(x => x.DewPoint)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            this.userDataRepository.SetDewpoint(-1)
        );
        await this.fixture.ApiContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        exception.Message.Should().Be("Player cannot have negative eldwater");

        int newDewpoint = await this
            .fixture.ApiContext.PlayerUserData.Where(x => x.ViewerId == IdentityTestUtils.ViewerId)
            .Select(x => x.DewPoint)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        newDewpoint.Should().Be(oldDewpoint);
    }
}
