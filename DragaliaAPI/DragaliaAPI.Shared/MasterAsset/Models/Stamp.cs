using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models;

/// <summary>
/// A sticker.
/// </summary>
/// <param name="Id">The ID of the sticker.</param>


[MemoryPackable]
public partial record Stamp(int Id);
