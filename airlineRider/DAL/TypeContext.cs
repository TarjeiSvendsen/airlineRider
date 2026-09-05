using airlineRider.Models;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.DAL;

public class TypeContext : DbContext
{
    public TypeContext(DbContextOptions<TypeContext> options)
        : base(options)
    { }
    public DbSet<AircraftType> AircraftTypes { get; set; } = null!;
    public DbSet<LiveryInfo> LiveryInfos { get; set; } = null!;
    public DbSet<Airport> Airports { get; set; } = null!;
    public DbSet<Airport> Runways { get; set; } = null!;


}