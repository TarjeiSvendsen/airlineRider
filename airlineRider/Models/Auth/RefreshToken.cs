using System.ComponentModel.DataAnnotations.Schema;

namespace airlineRider.Models.Auth;

public class RefreshToken
{
    [Column(TypeName = "uuid")]
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    
    [Column(TypeName = "uuid")]
    public Guid UserId { get; set; }
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
}
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
