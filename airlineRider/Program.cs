using System.Text;
using airlineRider.DAL;
using Microsoft.EntityFrameworkCore;
using airlineRider.Models;
using airlineRider.Services;
using airlineRider.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var logfactory = new LoggerFactory();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddResponseCaching();

// DBContext.
builder.Services.AddDbContextPool<TypeContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),x=> x.UseNetTopologySuite()));

var currentMode = Environment.GetEnvironmentVariable("DOTNET_CURRENT_MODE");
if (currentMode != "TESTING" ) // This is necessary because I 
    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6380"));


// JWT Authentication 
builder.Services.AddAuthentication(cfg => {
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.SaveToken = false;
    x.TokenValidationParameters = new TokenValidationParameters {
        ValidateAudience = false, // Just for testing
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8
                .GetBytes(builder.Configuration["JWTKey:Secret"]!)
        )
    };
});

// Automapper config
builder.Services.AddSingleton(logfactory);
builder.Services.AddSingleton(new MapperConfiguration(cfg => cfg.CreateMap<AircraftType, AircraftTypePublicDto>(),logfactory));


builder.Services.AddScoped<AircraftTypeService>();
builder.Services.AddScoped<AirportService>();
builder.Services.AddScoped<CountryService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LobbyService>();
builder.Services.AddScoped<AirlineService>();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    
    var typeContext = services.GetRequiredService<TypeContext>();
    
    //typeContext.Database.EnsureDeleted(); // Temporarily here as I constantly change the schema.
    //typeContext.Database.EnsureCreated(); // Will be replaced by Migrations in prod...
    
    var aircraftTypesImport = new AircraftTypeImporter(typeContext);
    // Imports all aircraft types (or skips it, depending on if it exists in the db already)
    aircraftTypesImport.ImportAll();

    var airportService = services.GetRequiredService<AirportService>();
    
    var airportImporter = new AirportImporter(airportService);
    // Imports all airports (or skips it, depending on if it exists in the db already) and returns the amount of airports imported.
    var savedAirports = await airportImporter.ImportAll();

    // 
    var countryService = services.GetRequiredService<CountryService>();

    var countryImporter = new CountryImporter(countryService);

    var savedCountries = await countryImporter.ImportAll();

    var userService = services.GetRequiredService<UserService>();

    if (app.Environment.IsDevelopment()) // Seeds database with test user credentials
    {
        await userService.SaveUser(new UserSignupDto("test@kvok.no", "test", "test"));
    }

}

app.Run();