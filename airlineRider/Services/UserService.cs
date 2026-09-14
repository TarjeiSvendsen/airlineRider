using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using airlineRider.DAL;
using airlineRider.Models.Auth;
using Microsoft.IdentityModel.Tokens;

namespace airlineRider.Services;

public class UserService(TypeContext typeContext,IConfiguration config)
{
    private readonly IConfigurationSection _jwtKeySection = 
        config.GetSection("JWTKey");
    
    public async Task<TokenResponse?> LoginAsync(LoginRequest request)
    {
        var user = typeContext.Users
            .FirstOrDefault(u => u.Username == request.Username);

        if (user == null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.HashedPassword))
        {
            return null; // Invalid credentials
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }


    public async Task<UserSignupSuccessDto?> SaveUser(UserSignupDto user)
    {
        if (typeContext.Users.Any(u => u.Username == user.username))
        {
            return null; // User already exists
        }

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(user.password);
        typeContext.Users.Add(new SystemUser(user.username,user.email,hashPassword,new []{"ROLE_USER"}));
        await typeContext.SaveChangesAsync();
        return new UserSignupSuccessDto(user.email,user.username);
    }
    
    
    private string GenerateAccessToken(SystemUser user)
    {
        var key = Encoding.UTF8.GetBytes(_jwtKeySection.GetValue<string>("Secret")!);
        var claimList = new List<Claim>();
        
        claimList.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        claimList.Add(new Claim(JwtRegisteredClaimNames.Name, user.Username));
        claimList.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claimList.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        
        foreach (string role in user.Roles)
        {
            claimList.Add(new Claim(ClaimTypes.Role,role));
        }
        
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtKeySection.GetValue<string>("ValidIssuer"),
            audience: _jwtKeySection.GetValue<string>("ValidAudience"),
            claims: claimList,
            expires: DateTime.UtcNow.AddHours(double.Parse(_jwtKeySection.GetValue<string>("TokenExpiryTimeInHour")!)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(SystemUser user)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddHours(
                double.Parse(_jwtKeySection.GetValue<string>("RefreshTokenExpiryTimeInHour")!))
        };

        typeContext.RefreshTokens.Add(refreshToken);
        await typeContext.SaveChangesAsync();
        return refreshToken;
    }
    
    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        var token = await typeContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (token == null || token.IsRevoked || token.Expires < DateTime.UtcNow)
        {
            return null; // Invalid or expired token
        }

        var user = await typeContext.Users.FindAsync(token.UserId);
        if (user == null)
        {
            return null; // User not found
        }

        // Generate new access token
        var newAccessToken = GenerateAccessToken(user);

        // Generate new refresh token and revoke the old one
        token.IsRevoked = true;
        var newRefreshToken = await GenerateRefreshTokenAsync(user);

        await typeContext.SaveChangesAsync();

        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        var token = await typeContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (token == null || token.IsRevoked)
        {
            return false;
        }

        token.IsRevoked = true;
        await typeContext.SaveChangesAsync();
        return true;
    }

}



public record UserSignupDto(string email, string username, string password);
public record UserSignupSuccessDto(string Email,string Username);