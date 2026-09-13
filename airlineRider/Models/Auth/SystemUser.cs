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


    public SystemUser(string username,string email,string hashedPassword)
    {
        this.Username = username;
        this.Email = email;
        this.HashedPassword = hashedPassword;
    }
}