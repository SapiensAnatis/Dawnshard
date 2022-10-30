using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record QuestReadStoryResponse(QuestReadStoryData data) : BaseResponse<QuestReadStoryData>;

[MessagePackObject(true)]
public record QuestReadStoryData(
    List<QuestReward> quest_story_reward_list,
    List<object> converted_entity_list,
    UpdateDataList update_data_list,
    EntityResult entity_result
);
