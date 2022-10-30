using System.ComponentModel;
using System.Runtime.Serialization;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(keyAsPropertyName: true)]
public class UpdateDataList
{
    public UserData? user_data { get; set; }

    public List<QuestStory> quest_story_list { get; set; } = new();

    public List<Chara>? chara_list { get; set; }

    public List<Dragon>? dragon_list { get; set; }

    public List<DragonReliability>? dragon_reliability_list { get; set; }

    public List<Party>? party_list { get; set; }

    public List<object> functional_maintenance_list { get; set; } = new();
}
