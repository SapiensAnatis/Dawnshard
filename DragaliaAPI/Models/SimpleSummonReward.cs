using MessagePack;

namespace DragaliaAPI.Models;

[MessagePackObject(true)]
public record SimpleSummonReward(int entity_type, int id, int rarity);
