using DragaliaAPI.Database.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

/// <summary>
/// Entity for tracking Mercurial Gauntlet Victor's Trove claim information.
/// </summary>
public class DbWallRewardDate : DbPlayerData
{
    /// <summary>
    /// Gets or sets the last time at which a player claimed their Mercurial Gauntlet trove.
    /// </summary>
    public DateTimeOffset LastClaimDate { get; set; }

    public class Configuration : IEntityTypeConfiguration<DbWallRewardDate>
    {
        public void Configure(EntityTypeBuilder<DbWallRewardDate> builder) =>
            builder.HasKey(e => e.ViewerId);
    }
}
