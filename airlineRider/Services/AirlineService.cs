using System.Text.Json;
using airlineRider.DAL;
using airlineRider.Models.Game;
using AutoMapper;
using StackExchange.Redis;

namespace airlineRider.Services;

public class AirlineService(TypeContext typeContext,IConnectionMultiplexer muxer,LoggerFactory loggerFactory)
{
    private readonly IDatabase _redis = muxer.GetDatabase();

    private readonly IMapper _publicDtoMapper = new MapperConfiguration(cfg => cfg.CreateMap<Airline, AirlinePublicDto>(),loggerFactory).CreateMapper();


    public Airline? GetAirlineDetailsBySlug(string airlineSlug, int lobbyId)
    {
        var redisResult = _redis.StringGet(new RedisKey($"airline:{lobbyId}:{airlineSlug}"));
        return redisResult.IsNull ? typeContext.Airlines.First(a => a.LobbyId == lobbyId && a.Slug == airlineSlug) : null;
    }
    
    public async Task<Airline> CreateNewAirlineAsync(AirlineCreationDto dto,int lobbyId,Guid systemUserId) //LobbySettings lobbySettings
    {
        if (typeContext.Airlines.Any(a => a.Slug == dto.Slug || a.Name == dto.Name))
        {
            throw new ArgumentException("An airline with this slug or name already exists!");
        }
        Airline airline = new()
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Slogan = dto.Slogan,
            LobbyId = lobbyId,
            SystemUserId = systemUserId
        };
        await typeContext.Airlines.AddAsync(airline);
        var serializedAirline = JsonSerializer.Serialize(airline);
        _redis.SetAdd(new RedisKey($"lobbyMembers{lobbyId}"), new RedisValue(dto.Slug));
        _redis.StringSet(new RedisKey($"airline:{lobbyId}:{dto.Slug}"), new RedisValue(serializedAirline), TimeSpan.FromHours(12));
        return airline;
    }
    
}
public record AirlineCreationDto(string Name,string Slug,string Slogan);
public record AirlinePublicDto(string Name,string Slug,string Slogan);