using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DragaliaAPI.Database.Entities;

public class DbUserImpersonation
{
    [Key]
    public required string DeviceAccountId { get; set; }

    public required string ImpersonatedDeviceAccountId { get; set; }

    public required long ImpersonatedViewerId { get; set; }

    [ForeignKey(nameof(DeviceAccountId))]
    public virtual DbPlayer? Owner { get; set; }

    [ForeignKey(nameof(ImpersonatedDeviceAccountId))]
    public virtual DbPlayer? ImpersonatedPlayer { get; set; }
}
