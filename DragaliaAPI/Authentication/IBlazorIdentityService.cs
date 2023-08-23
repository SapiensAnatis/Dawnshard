using DragaliaAPI.Shared.PlayerDetails;

namespace DragaliaAPI.Blazor.Authentication;

public interface IBlazorIdentityService : IPlayerIdentityService
{
    public string UserDataName { get; }
}
