using DragaliaAPI.Database;
using DragaliaAPI.Database.Entities;
using DragaliaAPI.Database.Repositories;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Dto.Game;
using DragaliaAPI.Services.Api;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DragaliaAPI.Services.Photon;

public class MatchingService : IMatchingService
{
    private readonly IPhotonStateApi photonStateApi;
    private readonly IUnitRepository unitRepository;
    private readonly IPartyRepository partyRepository;
    private readonly IUserDataRepository userDataRepository;

    public MatchingService(
        IPhotonStateApi photonStateApi,
        IUnitRepository unitRepository,
        IPartyRepository partyRepository,
        IUserDataRepository userDataRepository
    )
    {
        this.photonStateApi = photonStateApi;
        this.unitRepository = unitRepository;
        this.partyRepository = partyRepository;
        this.userDataRepository = userDataRepository;
    }

    public async Task<IEnumerable<RoomList>> GetRoomList()
    {
        IEnumerable<ApiGame> games = await photonStateApi.GetAllGames();
        List<RoomList> mapped = new();

        foreach (ApiGame game in games)
        {
            mapped.Add(await MapRoomList(game));
        }

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

        return mapped;
    }

    public async Task<MatchingGetRoomNameData?> GetRoomById(int id)
    {
        ApiGame? game = await this.photonStateApi.GetGameById(id);

        if (game is null)
            return null;

        RoomList roomList = await this.MapRoomList(game);
        return new()
        {
            room_name = game.Name,
            quest_id = game.QuestId,
            cluster_name = game.ClusterName,
            room_data = roomList,
        };
    }

    private async Task<RoomList> MapRoomList(ApiGame game)
    {
        DbPlayerUserData hostUserData = await userDataRepository
            .GetUserData(game.HostViewerId)
            .FirstAsync();

        DbPlayerCharaData hostCharaData = await partyRepository
            .GetPartyUnits(hostUserData.DeviceAccountId, game.HostPartyNo)
            .Where(x => x.UnitNo == 1)
            .Join(
                unitRepository.GetAllCharaData(hostUserData.DeviceAccountId),
                partyUnit => partyUnit.CharaId,
                charaData => charaData.CharaId,
                (partyUnit, charaData) => charaData
            )
            .FirstAsync();

        return new RoomList()
        {
            room_name = game.Name,
            cluster_name = game.ClusterName,
            region = game.Region,
            language = game.Language,
            host_name = hostUserData.Name,
            leader_chara_id = hostCharaData.CharaId,
            leader_chara_level = hostCharaData.Level,
            leader_chara_rarity = hostCharaData.Rarity,
            host_level = hostUserData.Level,
            host_viewer_id = (ulong)game.HostViewerId,
            member_num = game.MemberNum,
            quest_id = game.QuestId,
            quest_type = QuestTypes.Dungeon,
            room_id = game.RoomId,
            status = game.MemberNum >= 4 ? RoomStatuses.Full : RoomStatuses.Available,
            room_member_list = game.Players.Select(
                x => new AtgenRoomMemberList() { viewer_id = (ulong)x.ViewerId }
            ),
            entry_type = 1,
            start_entry_time = game.StartEntryTime,
            entry_guild_id = default,
            compatible_id = game.MatchingCompatibleId,
            entry_conditions = new()
            {
                objective_text_id = game.EntryConditions.ObjectiveTextId,
                required_party_power = game.EntryConditions.RequiredPartyPower,
                unaccepted_element_type_list = game.EntryConditions.UnacceptedElementTypeList,
                unaccepted_weapon_type_list = game.EntryConditions.UnacceptedWeaponTypeList,
            },
        };
    }
}
