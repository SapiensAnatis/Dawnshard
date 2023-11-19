namespace DragaliaAPI.Shared.PlayerDetails;

public interface IPlayerIdentityService
{
    string AccountId { get; }
    long ViewerId { get; }

    IDisposable StartUserImpersonation(long? viewer = null, string? account = null);
}
