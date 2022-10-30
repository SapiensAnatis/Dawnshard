using DragaliaAPI.Models.Dragalia.Responses.Common;
using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Responses;

[MessagePackObject(true)]
public record PartySetMainPartyNoResponse(PartySetMainPartyNoData data)
    : BaseResponse<PartySetMainPartyNoData>;

[MessagePackObject(true)]
public record PartySetMainPartyNoData(int main_party_no);
