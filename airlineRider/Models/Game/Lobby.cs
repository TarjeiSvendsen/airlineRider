using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models.Game;


[PrimaryKey("Id")]
public class Lobby
{
    public int Id { get; set; }
    [MaxLength(40)] public string Name { get; set; } = string.Empty;
    [MaxLength(128)] public string? Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    
    [MaxLength(8)]
    public string? AccessCode { get; set; }
    
    public LobbyDetails Details { get; set; }
    public LobbySettings Settings { get; set; }
    public LobbyStarterAircraft StarterAircraftDetails { get; set; }
    public List<Airline> LobbyMembers { get; set; }
    
}

[ComplexType]
public record LobbyDetails(DateTime CurrentTime);
[ComplexType]
public record LobbySettings(DateOnly StartDate,DateOnly EndDate,int StartingMoney);
[ComplexType]
public record LobbyStarterAircraft(string StarterAircraftTypeIcao,int StarterAircraftAmount);