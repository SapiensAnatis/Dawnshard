using DragaliaAPI.Models.Responses.Base;
using MessagePack;

namespace DragaliaAPI.Models.Responses;

[MessagePackObject(true)]
public record PartySetMainPartyNoResponse(PartySetMainPartyNoData data)
    : BaseResponse<PartySetMainPartyNoData>;

[MessagePackObject(true)]
public record PartySetMainPartyNoData(int main_party_no);
