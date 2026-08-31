using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models;

[PrimaryKey("_id")]
public class AircraftType
{
    [MaxLength(36)]
    public string _id { get; set; }
    
    [MaxLength(30)]
    public string Model { get; set; }
    [MaxLength(4)]
    public string Iata { get; set; }
    [MaxLength(4)]
    public string Icao { get; set; }
    [MaxLength(100)]
    public string Manufacturer { get; set; }
    
    [MaxLength(200)]
    public string Description { get; set; }
    
    public LiveryInfo LiveryInfo { get; set; }

    public AircraftType()
    {
        this._id = Guid.CreateVersion7().ToString();
        this.Model = "";
        this.Iata = "XXX";
        this.Icao = "XXXX";
        this.Manufacturer = "Default";
        this.Description = "...";
        this.LiveryInfo = new LiveryInfo("null","null");
    }
    public AircraftType(string model, string iata, string icao,string manufacturer,LiveryInfo liveryInfo,string description)
    {
        this._id = Guid.CreateVersion7().ToString();
        this.Model = model;
        this.Iata = iata;
        this.Icao = icao;
        this.Manufacturer = manufacturer;
        this.Description = description;
        this.LiveryInfo = liveryInfo;
    }
}

public record LiveryInfo(string WorkPath,string Preview){}