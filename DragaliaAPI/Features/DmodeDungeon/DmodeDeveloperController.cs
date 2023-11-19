using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Middleware;
using DragaliaAPI.Services.Exceptions;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Features.DmodeDungeon;

[Authorize(AuthenticationSchemes = SchemeName.Developer)]
[Route("dmode_dev")]
public class DmodeDeveloperController(
    IPlayerIdentityService playerIdentityService,
    IDmodeCacheService dmodeCacheService,
    IUserDataRepository userDataRepository
) : ControllerBase
{
    [HttpGet("floor_info/{viewerId}")]
    public async Task<IActionResult> GetFloorInfo(long viewerId)
    {
        string? accountId = await this.GetAccountId(viewerId);
        if (accountId is null)
            return this.NotFound("Player not found");

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(viewerId, accountId);

        try
        {
            string floorKey = (await dmodeCacheService.LoadPlayRecord()).floor_key;
            return this.Ok(await dmodeCacheService.LoadFloorInfo(floorKey));
        }
        catch (DragaliaException e) when (e.Code == ResultCode.CommonDbError)
        {
            return this.NotFound("No cache entry found");
        }
    }

    [HttpGet("play_record/{viewerId}")]
    public async Task<IActionResult> GetPlayRecord(long viewerId)
    {
        string? accountId = await this.GetAccountId(viewerId);
        if (accountId is null)
            return this.NotFound("Player not found");

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(viewerId, accountId);

        try
        {
            return this.Ok(await dmodeCacheService.LoadPlayRecord());
        }
        catch (DragaliaException e) when (e.Code == ResultCode.CommonDbError)
        {
            return this.NotFound("No cache entry found");
        }
    }

    [HttpGet("ingame_info/{viewerId}")]
    public async Task<IActionResult> GetIngameInfo(long viewerId)
    {
        string? accountId = await this.GetAccountId(viewerId);
        if (accountId is null)
            return this.NotFound("Player not found");

        using IDisposable ctx = playerIdentityService.StartUserImpersonation(viewerId, accountId);
        try
        {
            return this.Ok(await dmodeCacheService.LoadIngameInfo());
        }
        catch (DragaliaException e) when (e.Code == ResultCode.CommonDbError)
        {
            return this.NotFound("No cache entry found");
        }
    }

    private Task<string?> GetAccountId(long viewerId)
    {
        return userDataRepository
            .GetViewerData(viewerId)
            .Select(x => x.Owner!.AccountId)
            .SingleOrDefaultAsync();
    }
}
