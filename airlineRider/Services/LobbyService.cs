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
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Lobby, LobbyPublicDto>(),loggerFactory).CreateMapper();


    /**
     * May return null if lobby by the specified id is not found.
     */
    public LobbyPublicDetailDto? TryGetPublicLobbyDetail(int lobbyId)
    {
        var lobby = typeContext.Lobbies.Include(l => l.LobbyMembers).FirstOrDefault(lo => lo.Id == lobbyId);
        return lobby is null ? null : new LobbyPublicDetailDto(lobby.Name,lobby.IsPublic,lobby.LobbyMembers);
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
        return typeContext.Lobbies.Where(lobby => lobby.IsPublic).ToList().ConvertAll(input => _mapper.Map<LobbyPublicDto>(input));
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
        Lobby lobby = new Lobby
        {
            Name = dto.name,
            IsPublic = dto.isPublic,
            AccessCode = dto.password
        };
        
        typeContext.Add(lobby);
        _redis.StringSet("lobby:"+dto.name,JsonSerializer.Serialize(new LobbyPublicDto(lobby.Name,"",lobby.IsPublic)));
        
        typeContext.SaveChanges();
        return lobby;
    }
}

public record LobbyCreationDto(string name,string description,string password,bool isPublic,LobbyCreationSettingsDto Settings);

public record LobbyCreationSettingsDto(List<string> starterAircraftTypes,int starterAircraftAmountPerType,int startingMoney,DateTime startDate,DateTime endDate);
public record LobbyPublicDto(string Name,string Description,bool IsPublic);
public record LobbyPublicDetailDto(string Name,bool IsPublic,List<Airline> LobbyMembers);