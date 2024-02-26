using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Features.Missions;

public interface INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> NormalMissionList { get; set; }
    public IEnumerable<DailyMissionList> DailyMissionList { get; set; }
    public IEnumerable<PeriodMissionList> PeriodMissionList { get; set; }
    public IEnumerable<BeginnerMissionList> BeginnerMissionList { get; set; }
    public IEnumerable<SpecialMissionList> SpecialMissionList { get; set; }
    public IEnumerable<MainStoryMissionList> MainStoryMissionList { get; set; }
    public IEnumerable<MemoryEventMissionList> MemoryEventMissionList { get; set; }
    public IEnumerable<AlbumMissionList> AlbumMissionList { get; set; }
}
