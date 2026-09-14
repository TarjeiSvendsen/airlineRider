using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models.Auth;

[PrimaryKey("Id")]
public class SystemUser
{
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    
    public List<UserRole> Roles { get; set; }

    
}


[PrimaryKey("Id")]
public class UserRole
{
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; }   
    public string Role { get; set; }
    
    [Column(TypeName = "uuid")]
    public Guid SystemUserId { get; set; }
    public SystemUser SystemUser { get; set; } = null!;

    public UserRole(string role)
    {
        Role = role;
    }

}