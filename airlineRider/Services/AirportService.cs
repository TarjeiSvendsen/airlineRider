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

    public async Task<int> SaveCollection(List<Airport> airports)
    {
        await typeContext.Airports.AddRangeAsync(airports);
        return await typeContext.SaveChangesAsync();
    }
    
}