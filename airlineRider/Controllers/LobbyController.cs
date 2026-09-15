using System.Security.Claims;
using airlineRider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace airlineRider.Controllers;

[Authorize]
[ApiController]
[Route("/api")]
public class LobbyController(LobbyService lobbyService): ControllerBase
{
    
    [HttpGet]
    [Route("lobby/{lobbyId}/details")]
    public IActionResult GetPublicLobbyDetail([FromRoute] int lobbyId)
    {
        var lobby = lobbyService.GetPublicLobbyDetailById(lobbyId);
        if (lobby is not null)
        {
            return Ok(lobby);
        }
        return NotFound();
    }

    [HttpGet]
    [Route("lobbies/public")]
    public IActionResult GetPublicLobbies()
    {
        
        Console.WriteLine(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(lobbyService.GetPublicLobbies());
    }

    [HttpPost]
    [Route("lobby/new")]
    public IActionResult CreateNewLobby([FromBody] LobbyCreationDto creationDto)
    {
        return Ok(lobbyService.CreateNewLobby(creationDto));
    }
    
    [HttpGet]
    [Authorize(Roles = "ROLE_ADMIN")]
    [Route("lobbies/all_admin")]
    public IActionResult GetAllLobbies()
    {
        
        return Ok(lobbyService.GetAllLobbies());
    }

    
}
