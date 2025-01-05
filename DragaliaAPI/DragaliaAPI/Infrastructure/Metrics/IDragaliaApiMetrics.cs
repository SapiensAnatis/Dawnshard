using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Infrastructure.Metrics;

internal interface IDragaliaApiMetrics
{
    void OnSaveImport(LoadIndexResponse savefile);

    void OnSaveExport();

    void OnSaveEdit();

    void OnAccountCreated();

    void OnQuestCleared(DungeonSession session);
}
