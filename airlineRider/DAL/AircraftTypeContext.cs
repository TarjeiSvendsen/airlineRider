using airlineRider.Models;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.DAL;

public class AircraftTypeContext : DbContext
{
    public AircraftTypeContext(DbContextOptions<AircraftTypeContext> options)
        : base(options)
    { }
    public DbSet<AircraftType> AircraftTypes { get; set; } = null!;
    public DbSet<LiveryInfo> LiveryInfos { get; set; } = null!;

}