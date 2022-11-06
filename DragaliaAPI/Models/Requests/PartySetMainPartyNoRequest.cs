using MessagePack;

namespace DragaliaAPI.Models.Requests;

[MessagePackObject(true)]
public record PartySetMainPartyNoRequest(int main_party_no);
