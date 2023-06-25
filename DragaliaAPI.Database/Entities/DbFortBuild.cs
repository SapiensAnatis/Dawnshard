using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using DragaliaAPI.Shared.MasterAsset;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
public class DbFortBuild : IDbHasAccountId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long BuildId { get; set; }

    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    public FortPlants PlantId { get; set; }

    public int Level { get; set; } = 1;

    /// <summary>
    /// Get a fort_plant_detail_id from the stored entity properties.
    /// <remarks>Do not use in a .Select in queries; will cause the entire entity to load.</remarks>
    /// </summary>
    [NotMapped]
    public int FortPlantDetailId =>
        MasterAssetUtils.GetPlantDetailId(
            this.PlantId,
            BuildStatus == FortBuildStatus.Neutral ? Level : Level + 1
        );

    public int PositionX { get; set; }

    public int PositionZ { get; set; }

    [NotMapped]
    public FortBuildStatus BuildStatus => this.GetBuildStatus();

    public FortBuildStatus GetBuildStatus()
    {
        if (BuildStartDate != DateTimeOffset.UnixEpoch || BuildEndDate != DateTimeOffset.UnixEpoch)
        {
            return Level == 0 ? FortBuildStatus.Building : FortBuildStatus.LevelUp;
        }

        return FortBuildStatus.Neutral;
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
            TimeSpan result = this.BuildEndDate - DateTimeOffset.UtcNow;

            return result > TimeSpan.Zero ? result : TimeSpan.Zero;
        }
    }

    /// <remarks>Does not appear in the client model, but needs to be tracked for deriving LastIncomeTime.</remarks>
    public DateTimeOffset LastIncomeDate { get; set; } = DateTimeOffset.UnixEpoch;

    [NotMapped]
    public TimeSpan LastIncomeTime => DateTimeOffset.UtcNow - this.LastIncomeDate;
}
