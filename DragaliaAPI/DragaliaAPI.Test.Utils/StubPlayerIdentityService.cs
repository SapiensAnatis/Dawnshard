using DragaliaAPI.Database;
using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Test.Utils;

/// <summary>
/// Stub of <see cref="IPlayerIdentityService"/> for injecting into <see cref="ApiContext"/> in tests.
/// </summary>
/// <param name="viewerId">The viewer ID to use for this instance.</param>
public class StubPlayerIdentityService(long viewerId) : IPlayerIdentityService
{
    public string AccountId => throw new NotImplementedException();
    public long ViewerId => viewerId;

    public IDisposable StartUserImpersonation(long? viewer = null, string? account = null) =>
        throw new NotImplementedException();
}
