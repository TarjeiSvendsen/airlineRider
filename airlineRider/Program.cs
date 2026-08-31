var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    
    var aircraftTypeContext = services.GetRequiredService<AircraftTypeContext>();
    
    var aircraftTypesImport = new AircraftTypeImporter(aircraftTypeContext);
    // Imports all aircraft types (or skips it, depending on if it exists in the db already)
    aircraftTypesImport.ImportAll();

}


app.Run();