using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

[Table("FortBuildList")]
public class DbFortBuildList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long BuildId { get; set; }

    public required string AccountId { get; set; }

    public FortPlants PlantId { get; set; }

    public int Level { get; set; } = 1;

    [NotMapped]
    public int FortPlantDetailId => int.Parse($"{this.PlantId}{this.Level:00}");

    public int PositionX { get; set; }

    public int PositionY { get; set; }

    [NotMapped]
    public FortBuildStatus BuildStatus
    {
        get
        {
            if (
                this.BuildStartDate == DateTimeOffset.UnixEpoch
                && this.BuildEndDate == DateTimeOffset.UnixEpoch
            )
                return FortBuildStatus.None;

            if (DateTimeOffset.UtcNow < this.BuildEndDate)
                return FortBuildStatus.Construction;

            return FortBuildStatus.ConstructionComplete;
        }
    }

    public DateTimeOffset BuildStartDate { get; set; } = DateTimeOffset.UnixEpoch;

    public DateTimeOffset BuildEndDate { get; set; } = DateTimeOffset.UnixEpoch;

    public bool IsNew { get; set; }

    /// <remarks>Unknown what this does.</remarks>
    [NotMapped]
    public TimeSpan RemainTime { get; set; }

    /// <remarks>Does not appear in the client model, but needs to be tracked for deriving LastIncomeTime.</remarks>
    public DateTimeOffset LastIncomeDate { get; set; } = DateTimeOffset.UnixEpoch;

    [NotMapped]
    public TimeSpan LastIncomeTime => DateTime.UtcNow - this.LastIncomeDate;
}
