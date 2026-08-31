using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models;

[PrimaryKey("Id")]
public class AircraftType
{
    
    public Guid Id { get; set; }
    
    [MaxLength(30)]
    public string Model { get; set; }
    [MaxLength(4)]
    public string? Iata { get; set; }
    [MaxLength(4)]
    public string? Icao { get; set; }
    [MaxLength(100)]
    public string Manufacturer { get; set; }
    
    public LiveryInfo? LiveryInfo { get; set; }


    public AircraftType()
    {
        
    }
    public AircraftType(String model, String iata, String icao,String manufacturer,LiveryInfo liveryInfo)
    {
        this.Id = Guid.CreateVersion7();
        this.Model = model;
        this.Iata = iata;
        this.Icao = icao;
        this.Manufacturer = manufacturer;
        this.LiveryInfo = liveryInfo;
    }
}

public record LiveryInfo(string WorkPath,string Preview){}