using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

        AuthenticationState state = this.authenticationStateProvider
            .GetAuthenticationStateAsync()
            .Result;

        this.InitializeIdentity(state);
    }

    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        AuthenticationState state = await task;
        this.InitializeIdentity(state);
    }

    private void InitializeIdentity(AuthenticationState state)
    {
        this.accountId = state.User.Claims
            .FirstOrDefault(x => x.Type == CustomClaimType.AccountId)
            ?.Value;

        this.userIdName = state.User.Claims
            .FirstOrDefault(x => x.Type == CustomClaimType.PlayerName)
            ?.Value;

        string? viewerIdString = state.User.Claims
            .FirstOrDefault(x => x.Type == CustomClaimType.ViewerId)
            ?.Value;

        if (int.TryParse(viewerIdString, out int viewerId))
            this.viewerId = viewerId;
    }

    public string AccountId =>
        this.accountId ?? throw new InvalidOperationException("User was not authenticated!");

    public long? ViewerId =>
        this.viewerId ?? throw new InvalidOperationException("User was not authenticated!");

    public string UserDataName =>
        this.userIdName ?? throw new InvalidOperationException("User was not authenticated!");

    public IDisposable StartUserImpersonation(string account, long? viewer = null)
    {
        throw new NotImplementedException();
    }
}
