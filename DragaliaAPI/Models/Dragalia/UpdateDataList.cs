using DragaliaAPI.Models.Dragalia.Savefile;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia;

[MessagePackObject(keyAsPropertyName: true)]
public record UpdateDataList(SavefileUserData? user_data);
