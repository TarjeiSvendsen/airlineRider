using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models;


// "id", 0
// "airport_ref", 1
// "airport_ident", 2
// "length_ft", 3 
// "width_ft", 4
// "surface", 5
// "lighted", 6
// "closed", 7
// "le_ident", 8
// "le_latitude_deg", 9
// "le_longitude_deg", 10
// "le_elevation_ft", 11
// "le_heading_degT", 12
// "le_displaced_threshold_ft", 13
// "he_ident", 14
// "he_latitude_deg", 15 
// "he_longitude_deg", 16
// "he_elevation_ft", 17
// "he_heading_degT", 18
// "he_displaced_threshold_ft" 19
[PrimaryKey("Id")]
public class Runway
{
    public int Id { get; set; }
    public string AirportId { get; set; }
    [JsonIgnore]
    public Airport Airport { get; set; } = null!;
    
    [MaxLength(6)]
    public string LeIdent { get; set; }
    public int LeHeading { get; set; }

    [MaxLength(6)]
    public string HeIdent { get; set; } 
    public int HeHeading { get; set; }

    public double Length { get; set; }
    public double Width { get; set; }
    
    public bool Closed { get; set; }
    public bool Lighted { get; set; }
}