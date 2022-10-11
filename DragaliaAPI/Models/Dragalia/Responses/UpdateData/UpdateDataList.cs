using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.UpdateData;

[MessagePackObject(keyAsPropertyName: true)]
public class UpdateDataList
{
    public UserData? user_data { get; set; }

    public List<Chara>? chara_list { get; set; }

    public List<Dragon>? dragon_list { get; set; }
}
