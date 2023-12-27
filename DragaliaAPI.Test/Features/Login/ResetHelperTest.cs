using DragaliaAPI.Helpers;

namespace DragaliaAPI.Test.Features.Login;

public class ResetHelperTest
{
    private readonly Mock<TimeProvider> mockTimeProvider;

    private readonly IResetHelper resetHelper;

    public ResetHelperTest()
    {
        this.mockTimeProvider = new(MockBehavior.Strict);

        this.resetHelper = new ResetHelper(this.mockTimeProvider.Object);
    }

    /// <summary>
    /// Fields:
    ///     p1: Current time
    ///     p2: Expected last reset time
    /// </summary>
    public static ResetTheoryData DailyResetData { get; } =
        new()
        {
            {
                new DateTimeOffset(2023, 06, 25, 13, 15, 20, TimeSpan.Zero),
                new DateTimeOffset(2023, 06, 25, 06, 00, 00, TimeSpan.Zero)
            },
            {
                new DateTimeOffset(2023, 06, 25, 04, 20, 13, TimeSpan.Zero),
                new DateTimeOffset(2023, 06, 24, 06, 00, 00, TimeSpan.Zero)
            },
            {
                new DateTimeOffset(2023, 06, 25, 06, 20, 13, TimeSpan.Zero),
                new DateTimeOffset(2023, 06, 25, 06, 00, 00, TimeSpan.Zero)
            }
        };

    public static ResetTheoryData WeeklyResetData { get; } =
        new()
        {
            {
                new DateTimeOffset(2023, 06, 25, 13, 15, 20, TimeSpan.Zero),
                new DateTimeOffset(2023, 06, 19, 06, 00, 00, TimeSpan.Zero)
            },
            {
                new DateTimeOffset(2023, 06, 23, 13, 15, 20, TimeSpan.Zero),
                new DateTimeOffset(2023, 06, 19, 06, 00, 00, TimeSpan.Zero)
            },
            {
                new DateTimeOffset(2023, 06, 19, 04, 20, 13, TimeSpan.Zero),
                new DateTimeOffset(2023, 06, 12, 06, 00, 00, TimeSpan.Zero)
            },
            {
                new DateTimeOffset(2023, 06, 19, 06, 20, 13, TimeSpan.Zero),
                new DateTimeOffset(2023, 06, 19, 06, 00, 00, TimeSpan.Zero)
            },
        };

    public static ResetTheoryData MonthlyResetData { get; } =
        new()
        {
            {
                new DateTimeOffset(2023, 06, 25, 13, 15, 20, TimeSpan.Zero),
                new DateTimeOffset(2023, 06, 01, 06, 00, 00, TimeSpan.Zero)
            },
            {
                new DateTimeOffset(2023, 06, 01, 04, 20, 13, TimeSpan.Zero),
                new DateTimeOffset(2023, 05, 01, 06, 00, 00, TimeSpan.Zero)
            },
            {
                new DateTimeOffset(2023, 07, 31, 13, 15, 20, TimeSpan.Zero),
                new DateTimeOffset(2023, 07, 01, 06, 00, 00, TimeSpan.Zero)
            },
            {
                new DateTimeOffset(2023, 07, 01, 13, 15, 20, TimeSpan.Zero),
                new DateTimeOffset(2023, 07, 01, 06, 00, 00, TimeSpan.Zero)
            },
        };

    [Theory]
    [MemberData(nameof(DailyResetData))]
    public void LastDailyReset_ReturnsCorrectReset(DateTimeOffset now, DateTimeOffset expectedReset)
    {
        this.mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(now);

        this.resetHelper.LastDailyReset.Should().Be(expectedReset);
    }

    [Theory]
    [MemberData(nameof(WeeklyResetData))]
    public void LastWeeklyReset_ReturnsCorrectReset(
        DateTimeOffset now,
        DateTimeOffset expectedReset
    )
    {
        this.mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(now);

        this.resetHelper.LastWeeklyReset.Should().Be(expectedReset);
    }

    [Theory]
    [MemberData(nameof(MonthlyResetData))]
    public void LastMonthlyReset_ReturnsCorrectReset(
        DateTimeOffset now,
        DateTimeOffset expectedReset
    )
    {
        this.mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(now);

        this.resetHelper.LastMonthlyReset.Should().Be(expectedReset);
    }

    public class ResetTheoryData : TheoryData<DateTimeOffset, DateTimeOffset> { }
}
