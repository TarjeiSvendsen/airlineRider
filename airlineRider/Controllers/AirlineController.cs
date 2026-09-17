using System.Security.Claims;
using airlineRider.Models.Game;
using airlineRider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace airlineRider.Controllers;

[Authorize]
[ApiController]
[Route("/api/lobby")]
public class AirlineController(AirlineService airlineService,LobbyService lobbyService): ControllerBase
{
    
    [HttpGet]
    [Route("{lobbyId}/airline/{airlineSlug}/details")]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
    public IActionResult GetAirlineDetails([FromRoute] int lobbyId,[FromRoute] string airlineSlug)
    {
        var aResult = airlineService.GetAirlineDetailsBySlug(airlineSlug, lobbyId);
        if (aResult is not null)
        {
            return Ok(aResult);
        }

        return NotFound();
    }
    
    [HttpPost]
    [Route("{lobbyId}/airline/create")]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
    public async Task<IActionResult> CreateNewAirline([FromRoute] int lobbyId,[FromBody] AirlineCreationDto dto)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.Name), out var userGuid)) 
            return BadRequest();

        try
        {
            var airline = await airlineService.CreateNewAirlineAsync(dto, lobbyId, userGuid);
            lobbyService.AddAirlineToLobby(lobbyId, airline);
        }
        catch (ArgumentException ae)
        {
            return BadRequest("{'message':'airline with this name already exists'}");
        }
        
        
        return Ok();

    }
}
