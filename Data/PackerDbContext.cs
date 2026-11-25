using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packer.Models;

namespace Packer.Data;

public class PackerDbContext(DbContextOptions<PackerDbContext> options)
    : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<Gear> Gear => Set<Gear>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<TripGear> TripGear => Set<TripGear>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>().HasIndex(user => user.NormalizedEmail).IsUnique();
        ConfigureGear(builder.Entity<Gear>());

        var trip = builder.Entity<Trip>();
        ConfigureRecord(trip);
        trip.ToTable("trip", "public");
        trip.HasOne(item => item.User)
            .WithMany()
            .HasForeignKey(item => item.UserId);

        var tripGear = builder.Entity<TripGear>();
        tripGear.ToTable("trip_gear", "public", table =>
            table.HasCheckConstraint("CK_trip_gear_quantity", "\"Quantity\" > 0"));
        ConfigureRecord(tripGear);
        tripGear.HasIndex(item => new { item.TripId, item.GearId }).IsUnique();
        tripGear.HasOne(item => item.Trip).WithMany().HasForeignKey(item => item.TripId);
        tripGear.HasOne(item => item.Gear).WithMany().HasForeignKey(item => item.GearId);
        tripGear.Property(item => item.IsPacked).HasDefaultValue(false);
    }

    private static void ConfigureRecord<TEntity>(EntityTypeBuilder<TEntity> entity) where TEntity : class
    {
        entity.Property<Guid>("Id").ValueGeneratedNever();
        entity.Property<long>("Version").HasColumnName("version")
            .HasDefaultValue(1L).ValueGeneratedOnAddOrUpdate();
        entity.Property<DateTimeOffset>("CreatedAt").HasColumnName("created_at")
            .HasDefaultValueSql("statement_timestamp()");
        entity.Property<DateTimeOffset>("UpdatedAt").HasColumnName("updated_at")
            .HasDefaultValueSql("statement_timestamp()").ValueGeneratedOnAddOrUpdate();
    }

    private static void ConfigureGear(EntityTypeBuilder<Gear> gear)
    {
        ConfigureRecord(gear);
        var categories = string.Join(", ", Enum.GetNames<GearCategory>().Select(name => $"'{name}'"));

        gear.ToTable("gear", "public", table =>
        {
            table.HasCheckConstraint("CK_gear_weight_ounces", "\"WeightOunces\" >= 0");
            table.HasCheckConstraint("CK_gear_category", $"\"Category\" IN ({categories})");
        });

        gear.Property(item => item.Category).HasConversion<string>();
        gear.Property(item => item.IsConsumable).HasDefaultValue(false);
        gear.HasOne(item => item.User)
            .WithMany()
            .HasForeignKey(item => item.UserId);
    }
}
