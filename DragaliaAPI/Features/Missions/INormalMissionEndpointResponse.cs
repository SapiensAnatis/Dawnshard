using DragaliaAPI.Models.Generated;

// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006

namespace DragaliaAPI.Features.Missions;

public interface INormalMissionEndpointResponse
{
    public IEnumerable<NormalMissionList> normal_mission_list { get; set; }
    public IEnumerable<DailyMissionList> daily_mission_list { get; set; }
    public IEnumerable<PeriodMissionList> period_mission_list { get; set; }
    public IEnumerable<BeginnerMissionList> beginner_mission_list { get; set; }
    public IEnumerable<SpecialMissionList> special_mission_list { get; set; }
    public IEnumerable<MainStoryMissionList> main_story_mission_list { get; set; }
    public IEnumerable<MemoryEventMissionList> memory_event_mission_list { get; set; }
    public IEnumerable<AlbumMissionList> album_mission_list { get; set; }
}
