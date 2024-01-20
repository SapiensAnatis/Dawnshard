using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Authentication;

public interface IBlazorIdentityService : IPlayerIdentityService
{
    public string UserDataName { get; }
    bool IsAuthenticated { get; }

    Task InitializeAsync();
}
