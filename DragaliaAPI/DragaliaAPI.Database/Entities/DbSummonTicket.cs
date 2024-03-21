using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;

namespace DragaliaAPI.Database.Entities;

public class DbSummonTicket : DbPlayerData
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [Column("TicketKeyId")]
    public long KeyId { get; set; }

    [Column("Type")]
    public SummonTickets SummonTicketId { get; set; }

    [Column("Quantity")]
    public int Quantity { get; set; }

    [Column("ExpirationTime")]
    public DateTimeOffset UseLimitTime { get; set; } = DateTimeOffset.UnixEpoch;

    [NotMapped]
    public bool IsExpired =>
        this.UseLimitTime != DateTimeOffset.UnixEpoch && this.UseLimitTime > DateTimeOffset.UtcNow;
}
