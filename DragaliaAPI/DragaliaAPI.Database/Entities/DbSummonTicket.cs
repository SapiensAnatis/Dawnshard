using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

public class DbSummonTicket : DbPlayerData
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public long TicketKeyId { get; set; }

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
