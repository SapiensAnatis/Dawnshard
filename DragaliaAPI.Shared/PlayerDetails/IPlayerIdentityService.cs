namespace DragaliaAPI.Shared.PlayerDetails;

public interface IPlayerIdentityService
{
    string AccountId { get; }
    long? ViewerId { get; }

    IDisposable StartUserImpersonation(string account, long? viewer = null);
}
