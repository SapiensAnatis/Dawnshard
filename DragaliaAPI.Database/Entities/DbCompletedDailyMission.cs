using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId), nameof(Id), nameof(Date))]
public class DbCompletedDailyMission : DbPlayerData
{
    public int Id { get; set; }

    public int Progress { get; set; }

    public DateOnly Date { get; set; }

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }
}
