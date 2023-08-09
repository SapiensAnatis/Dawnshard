using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(DeviceAccountId))]
public class DbPartyPower : IDbHasAccountId
{
    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    [Required]
    public required string DeviceAccountId { get; set; }

    [Column("MaxPartyPower")]
    public int MaxPartyPower { get; set; }
}
