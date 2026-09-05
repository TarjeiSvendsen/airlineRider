using airlineRider.DAL;
using airlineRider.Models;
using AutoMapper;
using StackExchange.Redis;

namespace airlineRider.Services;

public class CountryService(TypeContext typeContext,IConnectionMultiplexer muxer,LoggerFactory loggerFactory)
{
    private readonly IDatabase _redis = muxer.GetDatabase();
    private readonly IMapper _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Airport, AirportService.AirportRedisDto>(),loggerFactory).CreateMapper();


    public bool CountriesPresentInDb()
    {
        return typeContext.Countries.Any();
    }
    
    public Country GetCountryByAlpha2(string alpha2)
    {
        return typeContext.Countries.First(country => country.Alpha2 == alpha2);
    }
    
    public Country GetCountryByAlpha3(string alpha3)
    {
        return typeContext.Countries.First(country => country.Alpha3 == alpha3);
    }

    
    
    public async Task<int> SaveCollection(List<Country> countries)
    {
        await typeContext.Countries.AddRangeAsync(countries);
        return await typeContext.SaveChangesAsync();
    }

    
}