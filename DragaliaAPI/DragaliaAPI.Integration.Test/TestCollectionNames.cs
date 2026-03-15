namespace DragaliaAPI.Integration.Test;

public static class TestCollectionNames
{
    /// <summary>
    /// Indicates the test is part of a collection that needs to set the current system time to assert against it, and
    /// should not be run in parallel with other tests that adjust the current system time.
    /// </summary>
    public const string MockTimeProvider = "MockTimeProvider";
}
