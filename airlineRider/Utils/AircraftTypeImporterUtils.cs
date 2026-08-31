using System.Text.Json;
using airlineRider.Models;

namespace airlineRider.Utils;

public class AircraftTypeImporterUtils
{

    public static LiveryInfo ParseLiveryInfo(JsonElement element,AircraftType type)
    {
        
        LiveryInfo info = new LiveryInfo();
        info.WorkDir = element.GetProperty("imageAssets").GetProperty("workPath").ToString();
        info.AircraftTypeId = type.Id;
        return info;
    }
}