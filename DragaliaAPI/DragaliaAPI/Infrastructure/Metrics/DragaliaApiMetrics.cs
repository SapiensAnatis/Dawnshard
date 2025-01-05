using System.Diagnostics;
using System.Diagnostics.Metrics;
using DragaliaAPI.Features.Dungeon;
using DragaliaAPI.Models.Generated;

namespace DragaliaAPI.Infrastructure.Metrics;

internal sealed class DragaliaApiMetrics : IDragaliaApiMetrics
{
    private readonly Counter<int> saveImportCounter;
    private readonly Counter<int> saveExportCounter;
    private readonly Counter<int> saveEditCounter;

    private readonly Counter<int> accountCreatedCounter;

    private readonly Counter<int> questClearedCounter;

    public static string MeterName => "DragaliaAPI";

    public DragaliaApiMetrics(IMeterFactory meterFactory)
    {
        Meter meter = meterFactory.Create(MeterName);

        this.saveImportCounter = meter.CreateCounter<int>("dragalia.savefile.imported");
        this.saveExportCounter = meter.CreateCounter<int>("dragalia.savefile.exported");
        this.saveEditCounter = meter.CreateCounter<int>("dragalia.savefile.edited");

        this.accountCreatedCounter = meter.CreateCounter<int>("dragalia.player.created");

        this.questClearedCounter = meter.CreateCounter<int>("dragalia.quest.clears");
    }

    public void OnSaveImport(LoadIndexResponse savefile)
    {
        TagList tags = [new("origin", savefile.Origin)];
        this.saveImportCounter.Add(1, tags);
    }

    public void OnSaveExport()
    {
        this.saveExportCounter.Add(1);
    }

    public void OnSaveEdit()
    {
        this.saveEditCounter.Add(1);
    }

    public void OnAccountCreated()
    {
        this.accountCreatedCounter.Add(1);
    }

    public void OnQuestCleared(DungeonSession session)
    {
        TagList tags = [new("co_op", session.IsMulti)];
        this.questClearedCounter.Add(1, tags);
    }
}
