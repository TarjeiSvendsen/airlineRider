using System.Collections.ObjectModel;
using airlineRider.DAL;
using airlineRider.Models;
using AutoMapper;
using StackExchange.Redis;

namespace airlineRider.Services;

public class AirportService(TypeContext typeContext,IConnectionMultiplexer muxer,MapperConfiguration mapperConfiguration)
{
    private readonly IDatabase _redis = muxer.GetDatabase();
    private readonly IMapper _mapper = mapperConfiguration.CreateMapper();


    public void Save(Airport airport)
    {
        typeContext.Airports.Add(airport);
        typeContext.SaveChanges();
    }

    public GeoRadiusResult[] GetAirportsWithinBoundingBox(GeoPosition center)
    {
        var results = _redis.GeoSearch(new RedisKey("airports:geo"), center.Longitude, center.Latitude,
            new GeoSearchBox(1_000_000, 1_000_000));
        
        return results;
    }
    public async Task<int> SaveCollection(List<Airport> airports)
    {
        foreach (Airport airport in airports)
        {
            var airportDto = new AirportRedisDto(airport.Name,airport.AType,airport.Icao,airport.Iata,airport.Location.Y,airport.Location.X);
            _redis.GeoAdd(new RedisKey("airports:geo"), new GeoEntry(airportDto.Longitude,airportDto.Latitude,new RedisValue(JsonSerializer.Serialize(airportDto))));
        }
        await typeContext.Airports.AddRangeAsync(airports);
        return await typeContext.SaveChangesAsync();
    }
    
}