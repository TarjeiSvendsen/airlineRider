using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using airlineRider.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models.Game;

[PrimaryKey("Id")]
public class Airline
{
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; }
    public string Name { get; set; }
    
    public int LobbyId { get; set; }
    public Lobby Lobby { get; set; }
    
    [Column(TypeName = "uuid")]
    public Guid SystemUserId { get; set; }
    [JsonIgnore]
    public SystemUser SystemUser { get; set; }
}