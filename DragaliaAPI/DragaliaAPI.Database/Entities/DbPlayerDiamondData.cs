using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

public class DbPlayerDiamondData : DbPlayerData
{
    public int FreeDiamond { get; set; }

    public int PaidDiamond { get; set; }
}

public class DbPlayerDiamondDataConfiguration : IEntityTypeConfiguration<DbPlayerDiamondData>
{
    public void Configure(EntityTypeBuilder<DbPlayerDiamondData> builder)
    {
        builder.HasKey(e => e.ViewerId);
    }
}
