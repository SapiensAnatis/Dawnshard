using MemoryPack;

namespace DragaliaAPI.Shared.MasterAsset.Models.Enemy;

[MemoryPackable]
public partial record EnemyData(int Id, int BookId);
