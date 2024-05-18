using DragaliaAPI.Extensions;
using Microsoft.Extensions.Time.Testing;

namespace DragaliaAPI.Test.Extensions;

public class ResetTimeProviderExtensionsTest
{
    private readonly FakeTimeProvider fakeTimeProvider;

    public ResetTimeProviderExtensionsTest()
    {
        this.fakeTimeProvider = new();
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
            },
            {
                new DateTimeOffset(2023, 06, 25, 05, 01, 13, TimeSpan.Zero),
                new DateTimeOffset(2023, 06, 24, 06, 00, 00, TimeSpan.Zero)
            }
        };

    /// <summary>
    /// Fields:
    ///     p1: Current time
    ///     p2: Expected last reset time
    /// </summary>
    public static ResetDayOfWeekTheoryData DailyResetDayOfWeekData { get; } =
        new()
        {
            {
                new DateTimeOffset(2023, 06, 25, 13, 15, 20, TimeSpan.Zero),
                DayOfWeek.Sunday // 2023-06-25
            },
            {
                new DateTimeOffset(2023, 06, 25, 04, 20, 13, TimeSpan.Zero),
                DayOfWeek.Saturday // 2023-06-24
            },
            {
                new DateTimeOffset(2023, 06, 25, 06, 20, 13, TimeSpan.Zero),
                DayOfWeek.Sunday // 2023-06-25
            },
            {
                new DateTimeOffset(2023, 06, 25, 05, 01, 13, TimeSpan.Zero),
                DayOfWeek.Saturday // 2023-06-24
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
    public void GetLastDailyReset_ReturnsCorrectReset(
        DateTimeOffset now,
        DateTimeOffset expectedReset
    )
    {
        this.fakeTimeProvider.SetUtcNow(now);
        this.fakeTimeProvider.SetLocalTimeZone(
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin")
        );

        this.fakeTimeProvider.GetLastDailyReset().Should().Be(expectedReset);
        this.fakeTimeProvider.GetLastDailyReset().Offset.Should().Be(TimeSpan.Zero);
    }

    [Theory]
    [MemberData(nameof(DailyResetDayOfWeekData))]
    public void GetLastDailyResetDayOfWeek_ReturnsCorrectResult(
        DateTimeOffset now,
        DayOfWeek expectedDayOfWeek
    )
    {
        this.fakeTimeProvider.SetUtcNow(now);
        this.fakeTimeProvider.SetLocalTimeZone(
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin")
        );

        this.fakeTimeProvider.GetLastDailyReset().DayOfWeek.Should().Be(expectedDayOfWeek);
    }

    [Theory]
    [MemberData(nameof(WeeklyResetData))]
    public void GetLastWeeklyReset_ReturnsCorrectReset(
        DateTimeOffset now,
        DateTimeOffset expectedReset
    )
    {
        this.fakeTimeProvider.SetUtcNow(now);

        this.fakeTimeProvider.GetLastWeeklyReset().Should().Be(expectedReset);
    }

    [Theory]
    [MemberData(nameof(MonthlyResetData))]
    public void GetLastMonthlyReset_ReturnsCorrectReset(
        DateTimeOffset now,
        DateTimeOffset expectedReset
    )
    {
        this.fakeTimeProvider.SetUtcNow(now);

        this.fakeTimeProvider.GetLastMonthlyReset().Should().Be(expectedReset);
    }

    public class ResetTheoryData : TheoryData<DateTimeOffset, DateTimeOffset> { }

    public class ResetDayOfWeekTheoryData : TheoryData<DateTimeOffset, DayOfWeek> { }
}
