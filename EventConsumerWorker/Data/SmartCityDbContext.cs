using Microsoft.EntityFrameworkCore;
using EventConsumerWorker.Models;
namespace EventConsumerWorker.Data;

public class SmartCityDbContext : DbContext
{
    public SmartCityDbContext(DbContextOptions<SmartCityDbContext> options)
    : base(options)
    {
    }
    public DbSet<TrafficEvent> TrafficEvents { get; set; }
    public DbSet<WeatherEvent> WeatherEvents { get; set; }
    public DbSet<ParkingEvent> ParkingEvents { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrafficEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Location);
        });
        modelBuilder.Entity<WeatherEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TemperatureCelsius).HasPrecision(5, 2);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Location);
        });
        modelBuilder.Entity<ParkingEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Location);
        });
    }
}