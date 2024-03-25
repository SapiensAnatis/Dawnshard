using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Shared.MasterAsset.Models.Trade;

using MemoryPack;

[MemoryPackable]
public record UseItemData(UseItem Id, UseItemEffect ItemEffect, int ItemEffectValue);
