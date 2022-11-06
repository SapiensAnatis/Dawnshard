using DragaliaAPI.Models.Components;
using MessagePack;

namespace DragaliaAPI.Models.Requests;

[MessagePackObject(true)]
public record PartySetPartySettingRequest(
    int party_no,
    string party_name,
    // Could've used the UpdateData Party if not for this slightly different name :(
    List<PartyUnit> request_party_setting_list
);
