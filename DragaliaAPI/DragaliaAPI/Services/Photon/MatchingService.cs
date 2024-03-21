using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Models;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.PlayerDetails;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Services.Photon;

public class MatchingService : IMatchingService
{
    private readonly IPhotonStateApi photonStateApi;
    private readonly IUnitRepository unitRepository;
    private readonly IPartyRepository partyRepository;
    private readonly IUserDataRepository userDataRepository;
    private readonly ILogger<MatchingService> logger;
    private readonly IPlayerIdentityService playerIdentityService;

    public MatchingService(
        IPhotonStateApi photonStateApi,
        IUnitRepository unitRepository,
        IPartyRepository partyRepository,
        IUserDataRepository userDataRepository,
        ILogger<MatchingService> logger,
        IPlayerIdentityService playerIdentityService
    )
    {
        this.photonStateApi = photonStateApi;
        this.unitRepository = unitRepository;
        this.partyRepository = partyRepository;
        this.userDataRepository = userDataRepository;
        this.logger = logger;
        this.playerIdentityService = playerIdentityService;
    }

    public async Task<IEnumerable<RoomList>> GetRoomList()
    {
        IEnumerable<ApiGame> games = await photonStateApi.GetAllGames();
        List<RoomList> mapped = new();

        foreach (ApiGame game in games)
        {
            mapped.Add(await MapRoomList(game));
        }

        this.logger.LogDebug("Got room list: {@list}", mapped);

        return mapped;
    }

    public async Task<IEnumerable<RoomList>> GetRoomList(int questId)
    {
        IEnumerable<ApiGame> games = await photonStateApi.GetByQuestId(questId);
        List<RoomList> mapped = new();

        foreach (ApiGame game in games)
        {
            mapped.Add(await MapRoomList(game));
        }

        this.logger.LogDebug("Got room list for quest {quest}: {@list}", questId, mapped);

        return mapped;
    }

    public async Task<MatchingGetRoomNameResponse?> GetRoomById(int id)
    {
        this.logger.LogDebug("Getting room for ID {id}", id);

        ApiGame? game = await this.photonStateApi.GetGameById(id);

        if (game is null)
        {
            this.logger.LogDebug("Game was null");
            return null;
        }

        RoomList roomList = await this.MapRoomList(game);
        this.logger.LogDebug("Got room: {@room}", roomList);

        return new()
        {
            RoomName = game.Name,
            QuestId = game.QuestId,
            ClusterName = game.ClusterName,
            RoomData = roomList,
        };
    }

    public async Task<IEnumerable<Player>> GetTeammates()
    {
        long viewerId = this.playerIdentityService.ViewerId;

        ApiGame? game = await this.photonStateApi.GetGameByViewerId(viewerId);

        if (game is null)
        {
            this.logger.LogWarning("Failed to retrieve game for ID {viewerId}", viewerId);
            return Enumerable.Empty<Player>();
        }

        return game.Players.Where(x => x.ViewerId != viewerId);
    }

    public async Task<string?> GetRoomName()
    {
        long viewerId = this.playerIdentityService.ViewerId;

        ApiGame? game = await this.photonStateApi.GetGameByViewerId(viewerId);

        if (game is null)
        {
            this.logger.LogWarning("Failed to retrieve game for ID {viewerId}", viewerId);
            return null;
        }

        return game.Name;
    }

    public async Task<bool> GetIsHost()
    {
        long viewerId = this.playerIdentityService.ViewerId;

        bool isHost = await this.photonStateApi.GetIsHost(viewerId);

        this.logger.LogDebug("Viewer ID {viewerId} isHost result: {isHost}", viewerId, isHost);

        return isHost;
    }

    private async Task<RoomList> MapRoomList(ApiGame game)
    {
        DbPlayerUserData hostUserData;
        DbPlayerCharaData hostCharaData;

        try
        {
            hostUserData = await userDataRepository.GetViewerData(game.HostViewerId).FirstAsync();

            using IDisposable ctx = this.playerIdentityService.StartUserImpersonation(
                (int)game.HostViewerId
            );

            hostCharaData = await partyRepository
                .GetPartyUnits(game.HostPartyNo)
                .Where(x => x.UnitNo == 1)
                .Join(
                    unitRepository.Charas,
                    partyUnit => partyUnit.CharaId,
                    charaData => charaData.CharaId,
                    (partyUnit, charaData) => charaData
                )
                .FirstAsync();
        }
        catch (Exception ex)
        {
            this.logger.LogWarning(
                ex,
                "Failed to lookup host data for host ID {hostId} party #{partyNo}. Using fallback.",
                game.HostViewerId,
                game.HostPartyNo
            );

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
                ViewerId = (ulong)x.ViewerId
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
}
