using airlineRider.Models;
using airlineRider.Models.Auth;
using airlineRider.Models.Game;
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
    public DbSet<Runway> Runways { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    
    public DbSet<SystemUser> Users { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!; 
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    
    public DbSet<Lobby> Lobbies { get; set; } = null!;
    public DbSet<Airline> Airlines { get; set; } = null!;

}