using DragaliaAPI.Database.Entities;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace DragaliaAPI.Test.Utils;

public static class CommonAssertionOptions
{
    public static EquivalencyAssertionOptions<T> WithDateTimeTolerance<T>(
        this EquivalencyAssertionOptions<T> options
    ) => WithDateTimeTolerance(options, TimeSpan.FromSeconds(1));

    public static EquivalencyAssertionOptions<T> WithDateTimeTolerance<T>(
        this EquivalencyAssertionOptions<T> options,
        TimeSpan tolerance
    )
    {
        options
            .Using<DateTimeOffset>(ctx =>
                ctx.Subject.Should().BeCloseTo(ctx.Expectation, tolerance)
            )
            .WhenTypeIs<DateTimeOffset>();

        return options;

        // AssertionOptions.AssertEquivalencyUsing(options =>
        //     options
        //         .Using<TimeSpan>(ctx =>
        //             ctx.Subject.Should()
        //                 .BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(toleranceSec))
        //         )
        //         .WhenTypeIs<TimeSpan>()
        // );
    }
}
