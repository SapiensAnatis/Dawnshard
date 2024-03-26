using DragaliaAPI.Shared.Definitions.Enums;
using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Trade;

[MemoryPackable]
public partial record UseItemData(UseItem Id, UseItemEffect ItemEffect, int ItemEffectValue);
