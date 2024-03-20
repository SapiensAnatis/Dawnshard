using System.ComponentModel.DataAnnotations.Schema;
using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DragaliaAPI.Database.Entities;

[PrimaryKey(nameof(ViewerId))]
public class DbPartyPower : DbPlayerData
{
    [Column("MaxPartyPower")]
    public int MaxPartyPower { get; set; }
}
