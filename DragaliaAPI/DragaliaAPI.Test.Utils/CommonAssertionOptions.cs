using DragaliaAPI.Database.Entities.Abstract;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace DragaliaAPI.Test.Utils;

public static class CommonAssertionOptions
{
    /// <summary>
    /// Applies a tolerance when comparing <see cref="DateTimeOffset"/> properties in FluentAssertions equivalency.
    /// </summary>
    /// <remarks>
    /// The default applied tolerance is +- three seconds.
    /// </remarks>
    /// <param name="options">Instance of <see cref="EquivalencyAssertionOptions{TExpectation}"/>.</param>
    /// <typeparam name="T">The root type of object being compared.</typeparam>
    /// <returns>The same instance given by <paramref name="options"/>, for chaining calls.</returns>
    public static EquivalencyAssertionOptions<T> WithDateTimeTolerance<T>(
        this EquivalencyAssertionOptions<T> options
    ) => WithDateTimeTolerance(options, TimeSpan.FromSeconds(3));

    /// <summary>
    /// Applies a tolerance when comparing <see cref="DateTimeOffset"/> properties in FluentAssertions equivalency.
    /// </summary>
    /// <param name="options">Instance of <see cref="EquivalencyAssertionOptions{TExpectation}"/>.</param>
    /// <param name="tolerance">The tolerance to apply.</param>
    /// <typeparam name="T">The root type of object being compared.</typeparam>
    /// <returns>The same instance given by <paramref name="options"/>, for chaining calls.</returns>
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
    }

    /// <summary>
    /// Applies a tolerance when comparing <see cref="TimeSpan"/> properties in FluentAssertions equivalency.
    /// </summary>
    /// <param name="options">Instance of <see cref="EquivalencyAssertionOptions{TExpectation}"/>.</param>
    /// <param name="tolerance">The tolerance to apply.</param>
    /// <typeparam name="T">The root type of object being compared.</typeparam>
    /// <returns>The same instance given by <paramref name="options"/>, for chaining calls.</returns>
    public static EquivalencyAssertionOptions<T> WithTimeSpanTolerance<T>(
        this EquivalencyAssertionOptions<T> options,
        TimeSpan tolerance
    )
    {
        options
            .Using<TimeSpan>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, tolerance))
            .WhenTypeIs<TimeSpan>();

        return options;
    }

    /// <summary>
    /// Excludes the <see cref="DbPlayerData.Owner"/> navigation property from equivalency assertions.
    /// </summary>
    /// <param name="options">Instance of <see cref="EquivalencyAssertionOptions{TExpectation}"/>.</param>
    /// <typeparam name="T">The root type of object being compared.</typeparam>
    /// <returns>The same instance given by <paramref name="options"/>, for chaining calls.</returns>
    public static EquivalencyAssertionOptions<T> ExcludingOwner<T>(
        this EquivalencyAssertionOptions<T> options
    )
        where T : DbPlayerData
    {
        options.Excluding(x => x.Owner);
        return options;
    }
}
