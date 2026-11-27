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
    public DbSet<TripFood> TripFood => Set<TripFood>();
    public DbSet<SleepingBag> SleepingBags => Set<SleepingBag>();

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

        var tripFood = builder.Entity<TripFood>();
        tripFood.ToTable("trip_food", "public");
        tripFood.Property(item => item.Id).ValueGeneratedNever();
        tripFood.HasOne(item => item.Trip).WithMany().HasForeignKey(item => item.TripId);

        var sleepingBag = builder.Entity<SleepingBag>();
        sleepingBag.ToTable("sleeping_bag", "public", table =>
            table.HasCheckConstraint("CK_sleeping_bag_temperature_order", "lower_limit_temp_f <= comfort_temp_f"));
        sleepingBag.HasKey(item => item.GearId);
        sleepingBag.Property(item => item.GearId).ValueGeneratedNever();
        sleepingBag.Property(item => item.LowerLimitTempF).HasColumnName("lower_limit_temp_f");
        sleepingBag.Property(item => item.ComfortTempF).HasColumnName("comfort_temp_f");
        sleepingBag.HasOne(item => item.Gear)
            .WithOne(item => item.SleepingBag)
            .HasForeignKey<SleepingBag>(item => item.GearId);
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
