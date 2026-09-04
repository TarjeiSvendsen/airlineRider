using airlineRider.DAL;
using Microsoft.EntityFrameworkCore;
using airlineRider.Models;
using airlineRider.Services;
using airlineRider.Tasks;
using AutoMapper;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var logfactory = new LoggerFactory();

// Add services to the container.
builder.Services.AddControllers();

// DBContext.
builder.Services.AddDbContextPool<TypeContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6380"));

// Automapper config
builder.Services.AddSingleton(logfactory);
builder.Services.AddSingleton(new MapperConfiguration(cfg => cfg.CreateMap<AircraftType, AircraftTypePublicDto>(),logfactory));


builder.Services.AddScoped<AircraftTypeService>();
builder.Services.AddScoped<AirportService>();


var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    
    var typeContext = services.GetRequiredService<TypeContext>();
    
    typeContext.Database.EnsureDeleted(); // Temporarily here as I constantly change the schema.
    typeContext.Database.EnsureCreated();
    
    var aircraftTypesImport = new AircraftTypeImporter(typeContext);
    // Imports all aircraft types (or skips it, depending on if it exists in the db already)
    aircraftTypesImport.ImportAll();

}

app.Run();