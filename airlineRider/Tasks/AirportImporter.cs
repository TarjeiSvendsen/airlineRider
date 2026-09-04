using airlineRider.Models;
using airlineRider.Services;
using airlineRider.Utils;
using NetTopologySuite.Geometries;

namespace airlineRider.Tasks;

public class AirportImporter(AirportService service)
{
    public static HashSet<string> DisallowedTypes = 
        new HashSet<string>(){ "closed","heliport","balloonport","seaplane_base"};
    
    /**
     * Returns the number of items saved.
     */
    public async Task<int> ImportAll()
    {
        var airportMap = new Dictionary<String,Airport>();
        var stream = new FileStream("Resources/Airports/airports.csv",FileMode.Open);
        var streamReader = new StreamReader(stream);
        string line;
        while ((line = streamReader.ReadLine()) != null)
        {
            var values = CsvUtils.ParseLine(line,',');
            var airport = new Airport();
            
            
            if (DisallowedTypes.Contains(values[2])) // Filters away the unwanted airport types
                continue;
            
            airport.AType = values[2];

            if (values[3].Contains("Base")) // Filters out air force bases, which wouldn't be available anyway.
            {
                continue;
            }
            airport.Name = values[3];
            
            // If this is null, it probably isn't relevant to add.
            if (values[12].IsWhiteSpace())
                continue;
            airport.Icao = values[12];
            
            // Not necessarily something to remove, but eases the burden on the database me thinks.
            if (values[13].IsWhiteSpace())
                continue;
            airport.Iata = values[13];
            
            airport.Alpha2CountryCode = values[8];
            
            // Does not necessarily mark grounds for removal, therefore it is just set to 0 if missing.
            double elevation;
            if (double.TryParse(values[6],out elevation)) airport.Elevation = elevation / 3.281; // Converts from feet to meters.
            else airport.Elevation = 0;

            // These, however, do constitute removal if they are missing.
            double latitude;
            if (!double.TryParse(values[4],out latitude))
                continue;
            
            double longitude;
            if (!double.TryParse(values[5],out longitude))
                continue;

            airport.Location = new Point(longitude, latitude);
            
            airport.Runways = new List<Runway>();
            
            airportMap.Add(airport.Icao,airport);
        }

        var runwayStream = new FileStream("Resources/Airports/runways.csv",FileMode.Open);
        var runwayStreamReader = new StreamReader(runwayStream);
        string rLine;
        while ((rLine = runwayStreamReader.ReadLine()) != null)
        {
            var values = CsvUtils.ParseLine(rLine,',');
            var runway = new Runway();

            int runwayID;
            if (!int.TryParse(values[0],out runwayID)) continue;
            runway.Id = runwayID;
            
            if(values[2].Length > 4) continue;
            runway.AirportId = values[2];

            int runwayLength;
            if(!int.TryParse(values[3],out runwayLength))
                continue;
            runway.Length = runwayLength / 3.281;

            int runwayWidth;
            if(!int.TryParse(values[3],out runwayWidth))
                continue;
            runway.Width = runwayWidth / 3.281;

            
            runway.LeIdent = values[8];
            int runwayLeHeading;
            if (!int.TryParse(values[12],out runwayLeHeading))
                continue;
            runway.LeHeading = runwayLeHeading;

            
            runway.HeIdent = values[14];
            int runwayHeHeading;
            if (!int.TryParse(values[18],out runwayHeHeading))
                continue;
            runway.HeHeading = runwayHeHeading;

            bool runwayClosed = false;
            if (values[7] == "1") runwayClosed = true;
            runway.Closed = runwayClosed;
            
            bool runwayLighted = false;
            if (values[6] == "1") runwayLighted = true;
            runway.Lighted = runwayLighted;

            
            if (airportMap.ContainsKey(runway.AirportId)) 
                airportMap.GetValueOrDefault(runway.AirportId).Runways.Add(runway);
            
        }

        foreach (Airport airport in airportMap.Values)
        {
            if (airport.Runways.Count == 0) airportMap.Remove(airport.Icao);
        }
        return await service.SaveCollection(airportMap.Values.ToList());
    }
}