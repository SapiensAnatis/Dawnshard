using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DragaliaAPI.Features.CoOp;

public partial class MatchingService : IMatchingService
{
    private readonly IPhotonStateApi photonStateApi;
    private readonly IPartyRepository partyRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly ILogger<MatchingService> logger;
    private readonly IPlayerIdentityService playerIdentityService;
    private readonly ApiContext apiContext;

    public MatchingService(
        IPhotonStateApi photonStateApi,
        IPartyRepository partyRepository,
        IUserDataRepository userDataRepository,
        ILogger<MatchingService> logger,
        IPlayerIdentityService playerIdentityService,
        ApiContext apiContext
    )
    {
        this.photonStateApi = photonStateApi;
        this.partyRepository = partyRepository;
        this.userDataRepository = userDataRepository;
        this.logger = logger;
        this.playerIdentityService = playerIdentityService;
        this.apiContext = apiContext;
    }

    public async Task<IEnumerable<RoomList>> GetRoomList()
    {
        IEnumerable<ApiGame> games = await this.photonStateApi.GetAllGames();
        List<RoomList> mapped = new();

        foreach (ApiGame game in games)
        {
            mapped.Add(await this.MapRoomList(game));
        }

        Log.GotRoomList(this.logger, mapped);

        return mapped;
    }

    public async Task<IEnumerable<RoomList>> GetRoomList(int questId)
    {
        IEnumerable<ApiGame> games = await this.photonStateApi.GetByQuestId(questId);
        List<RoomList> mapped = new();

        foreach (ApiGame game in games)
        {
            mapped.Add(await this.MapRoomList(game));
        }

        Log.GotRoomListForQuest(this.logger, questId, mapped);

        return mapped;
    }

    public async Task<MatchingGetRoomNameResponse?> GetRoomById(int id)
    {
        Log.GettingRoomForID(this.logger, id);

        ApiGame? game = await this.photonStateApi.GetGameById(id);

        if (game is null)
        {
            Log.GameWasNull(this.logger);
            return null;
        }

        RoomList roomList = await this.MapRoomList(game);
        Log.GotRoom(this.logger, roomList);

        return new()
        {
            RoomName = game.Name,
            QuestId = game.QuestId,
            ClusterName = game.ClusterName,
            RoomData = roomList,
        };
    }

    public async Task<IEnumerable<Photon.Shared.Models.Player>> GetTeammates()
    {
        long viewerId = this.playerIdentityService.ViewerId;

        ApiGame? game = await this.photonStateApi.GetGameByViewerId(viewerId);

        if (game is null)
        {
            Log.FailedToRetrieveGameForID(this.logger, viewerId);
            return Enumerable.Empty<Photon.Shared.Models.Player>();
        }

        return game.Players.Where(x => x.ViewerId != viewerId);
    }

    public async Task<bool> GetIsHost()
    {
        long viewerId = this.playerIdentityService.ViewerId;

        bool isHost = await this.photonStateApi.GetIsHost(viewerId);

        Log.ViewerIDIsHostResult(this.logger, viewerId, isHost);

        return isHost;
    }

    private async Task<RoomList> MapRoomList(ApiGame game)
    {
        DbPlayerUserData hostUserData;
        DbPlayerCharaData hostCharaData;

        try
        {
            hostUserData = await this
                .userDataRepository.GetViewerData(game.HostViewerId)
                .FirstAsync();

            using IDisposable ctx = this.playerIdentityService.StartUserImpersonation(
                (int)game.HostViewerId
            );

            hostCharaData = await this
                .partyRepository.GetPartyUnits(game.HostPartyNo)
                .Where(x => x.UnitNo == 1)
                .Join(
                    this.apiContext.PlayerCharaData,
                    partyUnit => partyUnit.CharaId,
                    charaData => charaData.CharaId,
                    (partyUnit, charaData) => charaData
                )
                .FirstAsync();
        }
        catch (Exception ex)
        {
            Log.FailedToLookupHostDataForHostIDPartyUsingFallback(this.logger, ex, game.HostViewerId, game.HostPartyNo);

            hostUserData = new()
            {
                ViewerId = 1,
                Name = "Euden",
                Level = 1,
            };

            hostCharaData = new(1, Charas.ThePrince);
        }

        return new RoomList()
        {
            RoomName = game.Name,
            ClusterName = game.ClusterName,
            Region = game.Region,
            Language = game.Language,
            HostName = hostUserData.Name,
            LeaderCharaId = hostCharaData.CharaId,
            LeaderCharaLevel = hostCharaData.Level,
            LeaderCharaRarity = hostCharaData.Rarity,
            HostLevel = hostUserData.Level,
            HostViewerId = (ulong)game.HostViewerId,
            MemberNum = game.MemberNum,
            QuestId = game.QuestId,
            QuestType = QuestTypes.Dungeon,
            RoomId = game.RoomId,
            Status = game.MemberNum >= 4 ? RoomStatuses.Full : RoomStatuses.Available,
            RoomMemberList = game.Players.Select(x => new AtgenRoomMemberList()
            {
                ViewerId = (ulong)x.ViewerId,
            }),
            EntryType = 1,
            StartEntryTime = game.StartEntryTime,
            EntryGuildId = default,
            CompatibleId = game.MatchingCompatibleId,
            EntryConditions = new()
            {
                ObjectiveTextId = game.EntryConditions.ObjectiveTextId,
                RequiredPartyPower = game.EntryConditions.RequiredPartyPower,
                UnacceptedElementTypeList = game.EntryConditions.UnacceptedElementTypeList,
                UnacceptedWeaponTypeList = game.EntryConditions.UnacceptedWeaponTypeList,
            },
        };
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Debug, "Got room list: {@list}")]
        public static partial void GotRoomList(ILogger logger, List<RoomList> list);
        [LoggerMessage(LogLevel.Debug, "Got room list for quest {quest}: {@list}")]
        public static partial void GotRoomListForQuest(ILogger logger, int quest, List<RoomList> list);
        [LoggerMessage(LogLevel.Debug, "Getting room for ID {id}")]
        public static partial void GettingRoomForID(ILogger logger, int id);
        [LoggerMessage(LogLevel.Debug, "Game was null")]
        public static partial void GameWasNull(ILogger logger);
        [LoggerMessage(LogLevel.Debug, "Got room: {@room}")]
        public static partial void GotRoom(ILogger logger, RoomList room);
        [LoggerMessage(LogLevel.Warning, "Failed to retrieve game for ID {viewerId}")]
        public static partial void FailedToRetrieveGameForID(ILogger logger, long viewerId);
        [LoggerMessage(LogLevel.Debug, "Viewer ID {viewerId} isHost result: {isHost}")]
        public static partial void ViewerIDIsHostResult(ILogger logger, long viewerId, bool isHost);
        [LoggerMessage(LogLevel.Warning, "Failed to lookup host data for host ID {hostId} party #{partyNo}. Using fallback.")]
        public static partial void FailedToLookupHostDataForHostIDPartyUsingFallback(ILogger logger, Exception exception, long hostId, int partyNo);
    }
}
