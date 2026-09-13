using airlineRider.Models;
using airlineRider.Models.Auth;
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
    public DbSet<Country> Countries { get; set; } = null!;
    
    public DbSet<SystemUser> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

}