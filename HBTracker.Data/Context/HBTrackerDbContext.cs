using HBTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HBTracker.Data.Context;

public class HBTrackerDbContext : DbContext
{
    public HBTrackerDbContext(DbContextOptions<HBTrackerDbContext> options) : base(options) {}

    public DbSet<PriceHistory> PriceHistories { get; set; }
    public DbSet<TrackedProduct> TrackedProducts { get; set; }
}
