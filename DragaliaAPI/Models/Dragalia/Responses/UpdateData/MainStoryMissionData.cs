using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(true)]
public record MainStoryMissionData(
    int main_story_mission_group_id,
    List<MainStoryMission> main_story_mission_state_list
);

[MessagePackObject(true)]
public record MainStoryMission(int main_story_mission_id, int state);
