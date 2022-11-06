using DragaliaAPI.Models.Base;
using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record QuestReadStoryResponse(QuestReadStoryData data) : BaseResponse<QuestReadStoryData>;

[MessagePackObject(true)]
public record QuestReadStoryData(
    List<QuestReward> quest_story_reward_list,
    List<object> converted_entity_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);
