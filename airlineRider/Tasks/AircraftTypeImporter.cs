using System.Text.Json;
using airlineRider.Models;
using airlineRider.Utils;

namespace airlineRider.Tasks;

public class AircraftTypeImporter(AircraftTypeContext typeContext)
{

    public void ImportAll()
    {
        if (typeContext.AircraftTypes.Any())
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
            aircraftType.Description = element.GetProperty("description").ToString();
            aircraftType.LiveryInfo = AircraftTypeImporterUtils.ParseLiveryInfo(element);
            typeContext.AircraftTypes.Add(aircraftType);
        }

        typeContext.SaveChanges();
    }
}