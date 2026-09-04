using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models;


// "id", 0
// "ident", 1
// "type", 2
// "name", 3 
// "latitude_deg", 4
// "longitude_deg", 5
// "elevation_ft", 6
// "continent", 7
// "iso_country", 8
// "iso_region", 9
// "municipality", 10
// "scheduled_service", 11
// "icao_code", 12
// "iata_code", 13
// "gps_code", 14
// "local_code", 15
// "home_link", 16 
// "wikipedia_link", 17
// "keywords"
// 
[PrimaryKey("Icao")]
public class Airport
{
    public string Name { get; set; }
    public string AType { get; set; }
    [MaxLength(4)]
    public string Icao { get; set; }
    [MaxLength(3)]
    public string Iata { get; set; }
    [MaxLength(2)]
    public string Alpha2CountryCode { get; set; }
    public double Elevation { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    
}