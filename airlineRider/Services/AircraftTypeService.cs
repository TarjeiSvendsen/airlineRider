using System.Text.Json;
using System.Text.Json.Serialization;
using airlineRider.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace airlineRider.Services;

public class AircraftTypeService(AircraftTypeContext typeContext,IConnectionMultiplexer muxer,MapperConfiguration mapperConfiguration)
{
    private readonly IDatabase _redis = muxer.GetDatabase();
    private IMapper _mapper = mapperConfiguration.CreateMapper();
    
    public List<AircraftTypePublicDto> GetAllAircraftTypes()
    {
        return typeContext.AircraftTypes.ToList().ConvertAll<AircraftTypePublicDto>(a => _mapper.Map<AircraftTypePublicDto>(a));
    }

    public async Task<AircraftTypePublicDto> GetAircraftTypeDtoByIcao(string icao)
    {
        var aType = await GetAircraftTypeByIcao(icao);
        var dtoMapped = _mapper.Map<AircraftTypePublicDto>(aType);
        return dtoMapped;
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
public record AircraftTypePublicDto(string Iata,string Icao,string Model,string Manufacturer,string Description,string BodyType);
