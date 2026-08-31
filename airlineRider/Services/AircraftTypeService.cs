using System.Text.Json;
using System.Text.Json.Serialization;
using airlineRider.Controllers;
using airlineRider.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace airlineRider.Services;

public class AircraftTypeService(AircraftTypeContext typeContext,IConnectionMultiplexer muxer)
{
    private readonly IDatabase _redis = muxer.GetDatabase();
    
    public List<AircraftTypePublicDto> GetAllAircraftTypes()
    {
        return typeContext.AircraftTypes.ToList().ConvertAll<AircraftTypePublicDto>(a =>
             new AircraftTypePublicDto(
                a.Iata,
                a.Icao,
                a.Model,
                a.Manufacturer,
                a.Description
                ));
    }

    public async Task<AircraftTypePublicDto> GetAircraftTypeDtoByIcao(string icao)
    {
        var aType = await GetAircraftTypeByIcao(icao);
        var dto = new AircraftTypePublicDto(aType.Iata, aType.Icao, aType.Model, aType.Manufacturer,aType.Description);
        return dto;
    }

    public async Task<AircraftType> GetAircraftTypeByIcao(string icao)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };
        AircraftType aType;
        var cached = await _redis.StringGetAsync(icao);
        if (cached.IsNullOrEmpty)
        { 
            aType = typeContext.AircraftTypes.Include(at=> at.LiveryInfo).First(a=> a.Icao == icao);
            var aTypeJson = JsonSerializer.Serialize(aType,options);
            var setaType = await _redis.StringSetAsync(new RedisKey("details:type:" + icao),new RedisValue(aTypeJson));
            var setaTypeExpire = await _redis.KeyExpireAsync(new RedisKey(icao), TimeSpan.FromHours(24));
        }
        else
        {
            aType = JsonSerializer.Deserialize<AircraftType>(cached.ToString());
        }

        return aType;
    }
}