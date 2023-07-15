using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(DeviceAccountId), nameof(QuestId), nameof(IsMulti), nameof(UnitNo))]
public class DbQuestClearPartyUnit : DbPartyUnitBase, IDbHasAccountId
{
    public virtual DbPlayer? Owner { get; set; }

    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    public int QuestId { get; set; }

    public bool IsMulti { get; set; }

    public int UnitNo { get; set; }
}
