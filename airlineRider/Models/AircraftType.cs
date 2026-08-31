using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models;

[PrimaryKey("Id")]
public class AircraftType
{
    
    public Guid Id { get; }
    
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
        this.Id = Guid.CreateVersion7();
        this.Model = "";
        this.Iata = "XXX";
        this.Icao = "XXXX";
        this.Manufacturer = "Default";
        this.Description = "...";
        this.LiveryInfo = new LiveryInfo("null","null");
    }
    public AircraftType(string model, string iata, string icao,string manufacturer,LiveryInfo liveryInfo,string description)
    {
        this.Id = Guid.CreateVersion7();
        this.Model = model;
        this.Iata = iata;
        this.Icao = icao;
        this.Manufacturer = manufacturer;
        this.Description = description;
        this.LiveryInfo = liveryInfo;
    }
}

public record LiveryInfo(string WorkPath,string Preview){}