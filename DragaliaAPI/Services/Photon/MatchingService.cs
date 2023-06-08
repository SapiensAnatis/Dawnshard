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
    private const string Japan = "jp";

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
        IEnumerable<Game> storedGames = await photonStateApi.GetAllGames();
        List<RoomList> mapped = new();

        foreach (Game storedGame in storedGames)
        {
            DbPlayerUserData hostUserData = await userDataRepository
                .GetUserData(storedGame.HostViewerId)
                .FirstAsync();

            DbPlayerCharaData leadCharaData = await partyRepository
                .GetPartyUnits(hostUserData.DeviceAccountId, hostUserData.MainPartyNo)
                .Where(x => x.UnitNo == 1)
                .Join(
                    unitRepository.GetAllCharaData(hostUserData.DeviceAccountId),
                    partyUnit => partyUnit.CharaId,
                    charaData => charaData.CharaId,
                    (partyUnit, charaData) => charaData
                )
                .FirstAsync();

            mapped.Add(
                new RoomList()
                {
                    room_name = storedGame.Name,
                    cluster_name = Japan,
                    region = Japan,
                    language = "en_us",
                    host_name = hostUserData.Name,
                    leader_chara_id = leadCharaData.CharaId,
                    leader_chara_level = leadCharaData.Level,
                    leader_chara_rarity = leadCharaData.Rarity,
                    host_level = hostUserData.Level,
                    host_viewer_id = (ulong)storedGame.HostViewerId,
                    member_num = storedGame.Players.Count(),
                    quest_id = storedGame.QuestId,
                    quest_type = QuestTypes.Dungeon,
                    room_id = storedGame.RoomId,
                    status = RoomStatuses.Available,
                    room_member_list = storedGame.Players.Select(
                        x => new AtgenRoomMemberList() { viewer_id = 1 }
                    ),
                    entry_type = 1,
                    start_entry_time = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(1),
                    entry_guild_id = default,
                    compatible_id = storedGame.MatchingCompatibleId,
                    entry_conditions = new()
                    {
                        objective_text_id = 0,
                        required_party_power = 0,
                        unaccepted_element_type_list = new List<int>(),
                        unaccepted_weapon_type_list = new List<int>(),
                    },
                }
            );
        }
        return mapped;
    }
}
