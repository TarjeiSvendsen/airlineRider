using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models;

[PrimaryKey("Id")]
public class AircraftType
{
    [MaxLength(36)]
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; }
    
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
    
    
    public virtual LiveryInfo LiveryInfo { get; set; }

    public AircraftType()
    {
        this.Model = "";
        this.Iata = "XXX";
        this.Icao = "XXXX";
        this.Manufacturer = "Default";
        this.Description = "...";
        
    }
    public AircraftType(string model, string iata, string icao,string manufacturer,LiveryInfo liveryInfo,string description)
    {
        this.Model = model;
        this.Iata = iata;
        this.Icao = icao;
        this.Manufacturer = manufacturer;
        this.Description = description;
        this.LiveryInfo = liveryInfo;
    }
}

[PrimaryKey("LiveryInfoId")]
public class LiveryInfo
{
    [ForeignKey("AircraftType")] 
    [Column(TypeName = "uuid")]
    public Guid LiveryInfoId;
    public string WorkDir { get; set; }
    
    [Column(TypeName = "uuid")]
    public Guid AircraftTypeId { get; set; }
    [JsonIgnore]
    public virtual AircraftType AircraftType { get; set; }
}