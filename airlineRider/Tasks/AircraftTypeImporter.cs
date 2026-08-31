using System.Text.Json;
using airlineRider.Models;
using airlineRider.Utils;

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
            var aircraftType = new AircraftType();
            aircraftType.Model = element.GetProperty("model").ToString();
            aircraftType.Iata = element.GetProperty("iata").ToString();
            aircraftType.Icao = element.GetProperty("icao").ToString();
            aircraftType.Manufacturer = element.GetProperty("manufacturer").ToString();
            aircraftType.LiveryInfo = AircraftTypeImporterUtils.ParseLiveryInfo(element);
            TypeContext.AircraftTypes.Add(aircraftType);
        }

        TypeContext.SaveChanges();
    }
}