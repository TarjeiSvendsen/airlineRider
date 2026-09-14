using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models.Game;


[PrimaryKey("Id")]
[Index(nameof(AccessCode))]
public class Lobby
{
    public int Id { get; set; }
    [MaxLength(40)]
    public string Name { get; set; }
    public bool IsPublic { get; set; }
    public string AccessCode { get; set; }
    
    public List<Airline> LobbyMembers { get; set; }
    
}