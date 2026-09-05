using airlineRider.Models;
using airlineRider.Services;
using airlineRider.Utils;

namespace airlineRider.Tasks;

public class CountryImporter(CountryService countryService)
{

    public async Task<int> ImportAll()
    {
        if (countryService.CountriesPresentInDb())
        {
            return 0;
        }
        
        var countryMap = new Dictionary<String,Country>();
        var countryStream = new FileStream("Resources/Various/Countries.csv",FileMode.Open);
        var countryStreamReader = new StreamReader(countryStream);
        countryStreamReader.ReadLine(); // Skips header
        string cLine;
        while ((cLine = countryStreamReader.ReadLine()) != null)
        {
            var country = new Country();
            var values = CsvUtils.ParseLine(cLine,',');
            country.Name = values[0];
            country.Alpha2 = values[1];
            country.Alpha3 = values[2];
            country.Region = values[5];
            
            countryMap.Add(country.Name,country);

        }
        
        // Acquired from https://github.com/rikgale/ICAOList/blob/main/RegPrefixList.csv but minor modifications were made.
        
        var icaoPrefixStream = new FileStream("Resources/Various/RegPrefixList.csv",FileMode.Open);
        var icaoStreamReader = new StreamReader(icaoPrefixStream);
        icaoStreamReader.ReadLine(); // Skips header
        string iLine;
        while ((iLine = icaoStreamReader.ReadLine()) != null)
        {
            var values = CsvUtils.ParseLine(iLine, ',');
            if (countryMap.ContainsKey(values[0]))
            {
                countryMap.GetValueOrDefault(values[0]).IcaoPrefix = values[1];
            }
        }
        
        return await countryService.SaveCollection(countryMap.Values.ToList());
    }
}