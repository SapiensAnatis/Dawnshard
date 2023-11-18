using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId))]
public class DbPlayerShopInfo : DbPlayerData
{
    public DateTimeOffset LastSummonTime { get; set; } = DateTimeOffset.UnixEpoch;
    public int DailySummonCount { get; set; }
}
