using System.Text.Json;
using airlineRider.Models;

namespace airlineRider.Tasks;

public class AircraftTypeImporter
{
    private AircraftTypeContext TypeContext { get; set; }

    public AircraftTypeImporter(AircraftTypeContext typeContext)
    {
        TypeContext = typeContext;
    }

    public void ImportAll()
    {
        if (TypeContext.AircraftTypes.Count() >= 1)
        {
            Console.WriteLine("Skipping Import");
            return;
        }

        var files = Directory.GetFiles("Resources/Aircraft/","*.json",SearchOption.AllDirectories);
        foreach (var fileName in files)
        {
            var file = File.ReadAllTextAsync(fileName);
            var element = JsonElement.Parse(file.Result);
            TypeContext.AircraftTypes.Add(new AircraftType(element.GetProperty("model").ToString(),element.GetProperty("iata").ToString(),element.GetProperty("icao").ToString(),element.GetProperty("manufacturer").ToString()));
        }

        TypeContext.SaveChanges();
    }
}