using DragaliaAPI.Shared;

namespace DragaliaAPI.Models.Nintendo;

[Obsolete(ObsoleteReasons.BaaS)]
public record DeviceAccount(string id, string password);
