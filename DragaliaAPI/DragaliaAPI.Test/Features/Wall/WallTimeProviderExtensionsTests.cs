using DragaliaAPI.Features.Wall;
using Microsoft.Extensions.Time.Testing;

namespace DragaliaAPI.Test.Features.Wall;

public class WallTimeProviderExtensionsTests
{
    private readonly FakeTimeProvider fakeTimeProvider = new();

    /// <summary>
    /// Fields:
    ///     p1: Current time
    ///     p2: Expected last reward time
    /// </summary>
    public static TheoryData<DateTimeOffset, DateTimeOffset> WallRewardTheoryData { get; } =
        new()
        {
            {
                DateTimeOffset.Parse("2024-05-14T21:27:31Z"),
                DateTimeOffset.Parse("2024-04-15T06:00:00Z")
            },
            {
                DateTimeOffset.Parse("2024-05-15T06:27:31Z"),
                DateTimeOffset.Parse("2024-05-15T06:00:00Z")
            },
            {
                DateTimeOffset.Parse("2024-05-26T06:27:31Z"),
                DateTimeOffset.Parse("2024-05-15T06:00:00Z")
            },
            {
                DateTimeOffset.Parse("2024-05-15T06:00:00Z"),
                DateTimeOffset.Parse("2024-05-15T06:00:00Z")
            }
        };

    [Theory]
    [MemberData(nameof(WallRewardTheoryData))]
    public void GetLastMonthlyRewardDate_ReturnsExpectedResult(
        DateTimeOffset currentTime,
        DateTimeOffset expectedRewardDate
    )
    {
        this.fakeTimeProvider.SetUtcNow(currentTime);

        this.fakeTimeProvider.GetLastWallRewardDate().Should().Be(expectedRewardDate);
    }
}
