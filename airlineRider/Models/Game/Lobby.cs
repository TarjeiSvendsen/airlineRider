using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models.Game;


[PrimaryKey("Id")]
[Index(nameof(AccessCode))]
public class Lobby
{
    public int Id { get; set; }
    [MaxLength(40)] public string Name { get; set; } = string.Empty;
    [MaxLength(128)] public string? Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    
    [MaxLength(8)]
    public string? AccessCode { get; set; }
    
    public LobbyDetails Details { get; set; }
    public List<Airline> LobbyMembers { get; set; }
    
}
public record LobbyDetails(DateTime currentTime);