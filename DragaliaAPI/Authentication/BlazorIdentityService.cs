using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Components.Authorization;

namespace DragaliaAPI.Blazor.Authentication;

public class BlazorIdentityService : IBlazorIdentityService
{
    private readonly AuthenticationStateProvider authenticationStateProvider;

    private string? accountId;
    private string? userIdName;
    private int? viewerId;

    public BlazorIdentityService(AuthenticationStateProvider authenticationStateProvider)
    {
        this.authenticationStateProvider = authenticationStateProvider;
        this.authenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    public async Task InitializeAsync()
    {
        AuthenticationState state =
            await this.authenticationStateProvider.GetAuthenticationStateAsync();

        this.SetIdentity(state);
    }

    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        AuthenticationState state = await task;
        this.SetIdentity(state);
    }

    private void SetIdentity(AuthenticationState state)
    {
        this.IsAuthenticated = state.User.Identity?.IsAuthenticated ?? false;

        if (!this.IsAuthenticated)
            return;

        this.accountId = state
            .User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.AccountId)
            ?.Value;

        this.userIdName = state
            .User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.PlayerName)
            ?.Value;

        string? viewerIdString = state
            .User.Claims.FirstOrDefault(x => x.Type == CustomClaimType.ViewerId)
            ?.Value;

        if (int.TryParse(viewerIdString, out int viewerId))
            this.viewerId = viewerId;
    }

    public bool IsAuthenticated { get; private set; }

    public string AccountId =>
        this.accountId ?? throw new InvalidOperationException("User was not authenticated!");

    public long ViewerId =>
        this.viewerId ?? throw new InvalidOperationException("User was not authenticated!");

    public string UserDataName =>
        this.userIdName ?? throw new InvalidOperationException("User was not authenticated!");

    public IDisposable StartUserImpersonation(long? viewer = null, string? account = null)
    {
        throw new NotImplementedException();
    }
}
