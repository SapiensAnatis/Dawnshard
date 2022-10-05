using DragaliaAPI.Models.Dragalia.Savefile;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses.Common;

[MessagePackObject(keyAsPropertyName: true)]
public record UpdateDataList(SavefileUserData? user_data);
