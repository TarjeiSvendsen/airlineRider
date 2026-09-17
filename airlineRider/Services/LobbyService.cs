using System.Text.Json;
using airlineRider.DAL;
using airlineRider.Models.Game;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace airlineRider.Services;

public class LobbyService(TypeContext typeContext,IConnectionMultiplexer muxer,LoggerFactory loggerFactory)
{
    private readonly IDatabase _redis = muxer.GetDatabase();
    private readonly IMapper _publicDtoMapper = new MapperConfiguration(cfg => cfg.CreateMap<Lobby, LobbyPublicDto>(),loggerFactory).CreateMapper();
    private readonly IMapper _publicDetailDtoMapper = new MapperConfiguration(cfg => cfg.CreateMap<Lobby, LobbyPublicDetailDto>(),loggerFactory).CreateMapper();


    /**
     * May return null if lobby by the specified id is not found.
     */
    
    public LobbyPublicDetailDto? GetPublicLobbyDetailById(int lobbyId)
    {
        var redisResult = _redis.StringGet(new RedisKey("lobby:" + lobbyId));
        if (redisResult.IsNull)
        {
            var lobby = typeContext.Lobbies.Include(l => l.LobbyMembers).FirstOrDefault(lo => lo.Id == lobbyId);
            return lobby is null ? null : _publicDetailDtoMapper.Map<LobbyPublicDetailDto>(lobby);
        }
        else
        {
            var lobby = JsonSerializer.Deserialize<Lobby>(redisResult.ToString());
            return _publicDetailDtoMapper.Map<LobbyPublicDetailDto>(lobby);
        }
    }

    public void AddAirlineToLobby(int lobbyId,Airline airline)
    {
        var lobby = typeContext.Lobbies.Include(lob => lob.LobbyMembers).First(l => l.Id == lobbyId);
        lobby.LobbyMembers.Add(airline);
        typeContext.Lobbies.Update(lobby);
        typeContext.SaveChanges();
    }

    /**
     * Gets public lobbies
     */
    public List<LobbyPublicDto> GetPublicLobbies()
    {
        return typeContext.Lobbies.Where(lobby => lobby.IsPublic).ToList().ConvertAll(input => _publicDtoMapper.Map<LobbyPublicDto>(input));
    }
    
    /**
     * Gets all lobbies, public and private, for admin use.
     */
    public List<Lobby> GetAllLobbies()
    {
        return typeContext.Lobbies.ToList();
    }


    public Lobby CreateNewLobby(LobbyCreationDto dto)
    {
        if (typeContext.Lobbies.Any(l => l.Slug == dto.Slug))
        {
            throw new ArgumentException("A lobby with this slug already exists");
        }
        Lobby lobby = new Lobby
        {
            Slug = dto.Slug,
            Name = dto.Name,
            Description = dto.Description,
            IsPublic = dto.IsPublic,
            AccessCode = dto.Password,
            Details = new LobbyDetails(dto.Settings.StartDate.ToDateTime(new TimeOnly(1,0))),
            Settings = new LobbySettings(dto.Settings.StartDate,dto.Settings.EndDate,dto.Settings.StartingMoney),
            StarterAircraftDetails = new LobbyStarterAircraft(dto.Settings.StarterAircraftType,dto.Settings.StarterAircraftAmount)
            
        };
        
        typeContext.Lobbies.Add(lobby);
        typeContext.SaveChanges();
        _redis.StringSet("lobby:"+lobby.Id,JsonSerializer.Serialize(_publicDetailDtoMapper.Map<LobbyPublicDetailDto>(lobby)));
        
        return lobby;
    }

    public async Task<int> DeleteLobbyAsync(int lobbyId)
    {
        var lobby = new Lobby() { Id = lobbyId };
        typeContext.Entry(lobby).State = EntityState.Deleted;
        return await typeContext.SaveChangesAsync();
        
    }
}

public record LobbyCreationDto(string Name,string Slug,string Description,string Password,bool IsPublic,LobbyCreationSettingsDto Settings);

public record LobbyCreationSettingsDto(string StarterAircraftType,int StarterAircraftAmount,int StartingMoney,DateOnly StartDate,DateOnly EndDate);
public record LobbyPublicDto(string Name,string Description,bool IsPublic);
public record LobbyPublicDetailDto(int Id,string Name,string Slug,bool IsPublic,List<Airline> LobbyMembers,LobbySettings Settings,LobbyStarterAircraft StarterAircraftDetails);