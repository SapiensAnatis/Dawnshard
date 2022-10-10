using System.Text.Json;
using DragaliaAPI.Models.Database.Savefile;
using DragaliaAPI.Models.Dragalia.Responses;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Savefile;
using DragaliaAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace DragaliaAPI.Controllers.Dragalia.RedoableSummon;

[Route("redoable_summon/fix_exec")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class FixExecController : ControllerBase
{
    private readonly IApiRepository _apiRepository;
    private readonly IDistributedCache _cache;
    private readonly ISessionService _sessionService;
    private const string GetSummonCacheKey = ":summonCache:";

    public FixExecController(
        IApiRepository apiRepository,
        IDistributedCache cache,
        ISessionService sessionService
    )
    {
        _apiRepository = apiRepository;
        _cache = cache;
        _sessionService = sessionService;
    }

    [HttpGet]
    public async Task<DragaliaResult> Get()
    {
        string sessionId = Request.Headers["SID"];
        try
        {
            if (!await _sessionService.ValidateSession(sessionId))
            {
                throw new TimeoutException($"No session exists for {sessionId}");
            }

            string deviceAccountId = await _sessionService.GetDeviceAccountId_SessionId(sessionId);

            string summonCacheKey = $":{deviceAccountId}Or{sessionId}" + GetSummonCacheKey;
            List<SummonEntity> cachedSummonResults = JsonSerializer.Deserialize<List<SummonEntity>>(
                await _cache.GetStringAsync(summonCacheKey)
            )!;
            Tuple<
                IEnumerable<Entity>,
                IEnumerable<Entity>,
                IEnumerable<Entity>
            > orderedPostCommitLists = await _apiRepository.commitSummonResults(
                deviceAccountId,
                cachedSummonResults
            );
            List<Entity> convertedSummonResults = orderedPostCommitLists.Item1.ToList();
            List<Entity> newSummonResults = orderedPostCommitLists.Item2.ToList();
            DbSavefileUserData dbUserData = await _apiRepository
                .GetPlayerInfo(deviceAccountId)
                .SingleAsync();
            SavefileUserData userData = SavefileUserDataFactory.Create(dbUserData, new() { });

            RedoableSummonFixExecResponse response =
                new(
                    RedoableSummonFixExecFactory.CreateData(
                        cachedSummonResults,
                        convertedSummonResults,
                        newSummonResults,
                        userData
                    )
                );
            return Ok(response);
        }
        catch (Exception)
        {
            return Ok(new ServerErrorResponse());
        }
    }
}
