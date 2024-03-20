using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[Index(nameof(ViewerId), nameof(Type))]
[PrimaryKey(nameof(ViewerId), nameof(Id))]
public class DbPlayerTrade : DbPlayerData
{
    [Column("TradeType")]
    public required TradeType Type { get; set; }

    [Column("TradeId")]
    public required int Id { get; set; }

    [Column("TradeCount")]
    public int Count { get; set; }

    [Column("LastTrade")]
    public DateTimeOffset LastTradeTime { get; set; } = DateTimeOffset.UnixEpoch;
}
