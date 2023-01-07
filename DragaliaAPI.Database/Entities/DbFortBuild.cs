﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

public class DbFortBuild : IDbHasAccountId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long BuildId { get; set; }

    public required string DeviceAccountId { get; set; }

    public FortPlants PlantId { get; set; }

    public int Level { get; set; } = 1;

    [NotMapped]
    public int FortPlantDetailId => int.Parse($"{(int)this.PlantId}{this.Level:00}");

    public int PositionX { get; set; }

    public int PositionZ { get; set; }

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
    public TimeSpan RemainTime
    {
        get
        {
            TimeSpan result = this.BuildEndDate - DateTime.UtcNow;

            return result > TimeSpan.Zero ? result : TimeSpan.Zero;
        }
    }

    /// <remarks>Does not appear in the client model, but needs to be tracked for deriving LastIncomeTime.</remarks>
    public DateTimeOffset LastIncomeDate { get; set; } = DateTimeOffset.UnixEpoch;

    [NotMapped]
    public TimeSpan LastIncomeTime => DateTime.UtcNow - this.LastIncomeDate;
}
