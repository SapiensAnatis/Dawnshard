using AutoMapper;
using DragaliaAPI.Models.Generated;
using DragaliaAPI.Photon.Shared.Models;

namespace DragaliaAPI.AutoMapper.Profiles;

public class PhotonMapProfile : Profile
{
    public PhotonMapProfile()
    {
        this.CreateMap<ApiGame, RoomList>();
        this.CreateMap<EntryConditions, AtgenEntryConditions>();
        this.CreateMap<Player, AtgenRoomMemberList>();
    }
}
