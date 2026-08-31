using Microsoft.EntityFrameworkCore;
using airlineRider.Models;
using airlineRider.Tasks;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// DBContext for Aircraft types.
builder.Services.AddDbContextPool<AircraftTypeContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6380"));

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    
    var aircraftTypeContext = services.GetRequiredService<AircraftTypeContext>();
    
    aircraftTypeContext.Database.EnsureDeleted(); // Temporarily here as I constantly change the schema.
    aircraftTypeContext.Database.EnsureCreated();
    
    var aircraftTypesImport = new AircraftTypeImporter(aircraftTypeContext);
    // Imports all aircraft types (or skips it, depending on if it exists in the db already)
    aircraftTypesImport.ImportAll();

}

app.Run();