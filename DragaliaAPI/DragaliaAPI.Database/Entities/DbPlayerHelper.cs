using DragaliaAPI.Shared.Definitions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DragaliaAPI.Database.Entities;

public class DbPlayerHelper
{
    public long ViewerId { get; set; }

    public Charas CharaId { get; set; }

    public long? EquipDragonKeyId { get; set; }

    public WeaponBodies? EquipWeaponBodyId { get; set; }

    public AbilityCrestId? EquipCrestSlotType1CrestId1 { get; set; }

    public AbilityCrestId? EquipCrestSlotType1CrestId2 { get; set; }

    public AbilityCrestId? EquipCrestSlotType1CrestId3 { get; set; }

    public AbilityCrestId? EquipCrestSlotType2CrestId1 { get; set; }

    public AbilityCrestId? EquipCrestSlotType2CrestId2 { get; set; }

    public AbilityCrestId? EquipCrestSlotType3CrestId1 { get; set; }

    public AbilityCrestId? EquipCrestSlotType3CrestId2 { get; set; }

    public long? EquipTalismanKeyId { get; set; }

    public int HelperRewardCount { get; set; }

    public int HelperFriendRewardCount { get; set; }

    public DbPlayer? Owner { get; set; }

    public DbPlayerCharaData? EquippedChara { get; set; }

    public DbPlayerDragonData? EquippedDragon { get; set; }

    public DbWeaponBody? EquippedWeaponBody { get; set; }

    public DbAbilityCrest? EquippedCrestSlotType1Crest1 { get; set; }

    public DbAbilityCrest? EquippedCrestSlotType1Crest2 { get; set; }

    public DbAbilityCrest? EquippedCrestSlotType1Crest3 { get; set; }

    public DbAbilityCrest? EquippedCrestSlotType2Crest1 { get; set; }

    public DbAbilityCrest? EquippedCrestSlotType2Crest2 { get; set; }

    public DbAbilityCrest? EquippedCrestSlotType3Crest1 { get; set; }

    public DbAbilityCrest? EquippedCrestSlotType3Crest2 { get; set; }

    public DbTalisman? EquippedTalisman { get; set; }

    public List<DbPlayerHelperUseDate> UseDates { get; set; } = [];

    private class Configuration : IEntityTypeConfiguration<DbPlayerHelper>
    {
        public void Configure(EntityTypeBuilder<DbPlayerHelper> builder)
        {
            builder.HasKey(e => e.ViewerId);

            builder
                .HasOne(e => e.Owner)
                .WithOne(e => e.Helper)
                .HasForeignKey<DbPlayerHelper>(e => e.ViewerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedChara)
                .WithOne()
                .HasForeignKey<DbPlayerHelper>(e => new { e.ViewerId, e.CharaId })
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedDragon)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => e.EquipDragonKeyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedWeaponBody)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => new { e.ViewerId, e.EquipWeaponBodyId })
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedCrestSlotType1Crest1)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => new
                {
                    e.ViewerId,
                    e.EquipCrestSlotType1CrestId1,
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedCrestSlotType1Crest2)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => new
                {
                    e.ViewerId,
                    e.EquipCrestSlotType1CrestId2,
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedCrestSlotType1Crest3)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => new
                {
                    e.ViewerId,
                    e.EquipCrestSlotType1CrestId3,
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedCrestSlotType2Crest1)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => new
                {
                    e.ViewerId,
                    e.EquipCrestSlotType2CrestId1,
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedCrestSlotType2Crest2)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => new
                {
                    e.ViewerId,
                    e.EquipCrestSlotType2CrestId2,
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedCrestSlotType3Crest1)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => new
                {
                    e.ViewerId,
                    e.EquipCrestSlotType3CrestId1,
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedCrestSlotType3Crest2)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => new
                {
                    e.ViewerId,
                    e.EquipCrestSlotType3CrestId2,
                })
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.EquippedTalisman)
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<DbPlayerHelper>(e => e.EquipTalismanKeyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
