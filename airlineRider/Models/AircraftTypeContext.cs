using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models;

public class AircraftTypeContext : DbContext
{
    public AircraftTypeContext(DbContextOptions<AircraftTypeContext> options)
        : base(options)
    {
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    public DbSet<AircraftType> AircraftTypes { get; set; } = null!;

}