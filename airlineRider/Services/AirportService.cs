using System.Text.Json;
using airlineRider.DAL;
using airlineRider.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace airlineRider.Services;

public class AirportService(TypeContext typeContext,IConnectionMultiplexer muxer,LoggerFactory loggerFactory)
{
    private readonly IDatabase _redis = muxer.GetDatabase();
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Airport, AirportRedisDto>(),loggerFactory).CreateMapper();


    /*
     * Just used by the AirportImporter to save time in case of a system restart, where there is no need to repopulate the db.
     */
    public bool AirportsExistInDb()
    {
        return typeContext.Airports.Any();
    }
    public Airport GetAirportByIcao(string icao)
    {
        return typeContext.Airports.Include(airport => airport.Runways).First(airport => airport.Icao == icao);
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

    public class AirportRedisDto
    {
        public AirportRedisDto(string name, string aType, string icao, string iata, double latitude, double longitude)
        {
            Name = name;
            AType = aType;
            Icao = icao;
            Iata = iata;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Name { get; set; }
        public string AType { get; set; }
        public string Icao { get; set; } 
        public string Iata { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}