using DragaliaAPI.Database.Entities;
using FluentAssertions;

namespace DragaliaAPI.Test.Utils;

public static class CommonAssertionOptions
{
    public static void ApplyTimeOptions(int toleranceSec = 1)
    {
        AssertionOptions.AssertEquivalencyUsing(
            options =>
                options
                    .Using<DateTimeOffset>(
                        ctx =>
                            ctx.Subject.Should()
                                .BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(toleranceSec))
                    )
                    .WhenTypeIs<DateTimeOffset>()
        );

        AssertionOptions.AssertEquivalencyUsing(
            options =>
                options
                    .Using<TimeSpan>(
                        ctx =>
                            ctx.Subject.Should()
                                .BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(toleranceSec))
                    )
                    .WhenTypeIs<TimeSpan>()
        );
    }

    public static void ApplyIgnoreOwnerOptions()
    {
        AssertionOptions.AssertEquivalencyUsing(
            options => options.Excluding(x => x.Type == typeof(DbPlayer))
        );
    }
}
