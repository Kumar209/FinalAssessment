using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FinalAssessment_Backend.Models.Entities;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PrashantDbAddress> PrashantDbAddresses { get; set; }

    public virtual DbSet<PrashantDbMasterAddressType> PrashantDbMasterAddressTypes { get; set; }

    public virtual DbSet<PrashantDbUser> PrashantDbUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrashantDbAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prashant__3214EC07C18403A9");

            entity.ToTable("PrashantDbAddress");

            entity.Property(e => e.City)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.State)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ZipCode)
                .HasMaxLength(6)
                .IsUnicode(false);

            entity.HasOne(d => d.AddressType).WithMany(p => p.PrashantDbAddresses)
                .HasForeignKey(d => d.AddressTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PrashantD__Addre__113584D1");

            entity.HasOne(d => d.User).WithMany(p => p.PrashantDbAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PrashantD__UserI__1229A90A");
        });

        modelBuilder.Entity<PrashantDbMasterAddressType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prashant__3214EC0704EE6951");

            entity.ToTable("PrashantDbMasterAddressType");

            entity.Property(e => e.AddressType)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PrashantDbUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prashant__3214EC07ECC1B546");

            entity.ToTable("PrashantDbUser");

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(90)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FirstName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.MiddleName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(90)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(60)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
