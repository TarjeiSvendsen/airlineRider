using System.Text.Json;
using airlineRider.Services;
using Microsoft.AspNetCore.Mvc;

namespace airlineRider.Controllers;

[ApiController]
[Route("/api/aircraft/type")]
public class AircraftTypeController(AircraftTypeService aircraftTypeService) : ControllerBase
{
    
    [HttpGet("all")]
    public List<AircraftTypePublicDto> GetAll()
    {
        return aircraftTypeService.GetAllAircraftTypes();
    }

    [HttpGet("{aircraftType}/details")]
    [Produces("application/json")]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 36000)]
    public async Task<IActionResult> GetDetails([FromRoute] string aircraftType)
    {
        try
        {
            var resultDto = await aircraftTypeService.GetAircraftTypeDtoByIcao(aircraftType);
            var result = new ContentResult();
            result.Content = JsonSerializer.Serialize(resultDto);
            result.ContentType = "application/json";
            return result;
        }
        catch (InvalidOperationException)
        {
            return new NotFoundResult();
        }
    }
    
    [HttpGet("{aircraftType}/preview")]
    [Produces("image/png")]
    [ResponseCache(VaryByHeader = "User-Agent", Duration = 36000)]
    public async Task<IActionResult> GetImage([FromRoute] string aircraftType)
    {
        try
        {
            var aType = await aircraftTypeService.GetAircraftTypeByIcao(aircraftType);
            var b = await System.IO.File.ReadAllBytesAsync(
                aType.LiveryInfo.WorkDir + "/preview.png");
            return File(b,"image/png");
        }
        catch (InvalidOperationException)
        {
            return new NotFoundResult();
        }
    }
    
}
