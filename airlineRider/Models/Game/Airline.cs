using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using airlineRider.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models.Game;

[PrimaryKey("Id")]
[Microsoft.EntityFrameworkCore.Index(nameof(Name))]
public class Airline
{
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; }

    [MaxLength(40)]
    public string Name { get; set; } = string.Empty;
    
    public int LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    
    [Column(TypeName = "uuid")]
    public Guid SystemUserId { get; set; }
    [JsonIgnore]
    public SystemUser SystemUser { get; set; }
}

[ComplexType]
public record AirlineFinances(Int64 CurrentBalance);
[ComplexType]
public record AircraftLease(int MonthlyCost,DateTime ContractStart,DateTime ContractEnd);