using MessagePack;

namespace DragaliaAPI.Models.Dragalia.Requests;

[MessagePackObject(true)]
public record PartySetMainPartyNoRequest(int main_party_no);
