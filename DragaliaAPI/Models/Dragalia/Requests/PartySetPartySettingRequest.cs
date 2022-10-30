using DragaliaAPI.Models.Dragalia.Responses.UpdateData; // should probably move this to a common location so requests don't reference responses
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests;

[MessagePackObject(true)]
public record PartySetPartySettingRequest(
    int party_no,
    string party_name,
    // Could've used the UpdateData Party if not for this slightly different name :(
    List<PartyUnit> request_party_setting_list
);
