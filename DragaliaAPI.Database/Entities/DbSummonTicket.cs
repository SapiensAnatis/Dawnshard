using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(DeviceAccountId))]
public class DbSummonTicket : IDbHasAccountId
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public long TicketKeyId { get; set; }

    /// <inheritdoc />
    public virtual DbPlayer? Owner { get; set; }

    /// <inheritdoc />
    [ForeignKey(nameof(Owner))]
    public required string DeviceAccountId { get; set; }

    [Column("Type")]
    public SummonTickets Type { get; set; }

    [Column("Quantity")]
    public int Quantity { get; set; }

    [Column("ExpirationTime")]
    public DateTimeOffset ExpirationTime { get; set; }

    [NotMapped]
    public bool IsExpired =>
        ExpirationTime != DateTimeOffset.UnixEpoch && ExpirationTime > DateTimeOffset.UtcNow;
}
