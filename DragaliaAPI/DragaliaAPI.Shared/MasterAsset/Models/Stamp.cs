namespace DragaliaAPI.Shared.MasterAsset.Models;

/// <summary>
/// A sticker.
/// </summary>
/// <param name="Id">The ID of the sticker.</param>
using MemoryPack;

[MemoryPackable]
public record Stamp(int Id);
