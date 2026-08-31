using System.Text.Json;
using airlineRider.Models;

namespace airlineRider.Utils;

public class AircraftTypeImporterUtils
{

    public static LiveryInfo ParseLiveryInfo(JsonElement element)
    {
        if (element.TryGetProperty("imageAssets", out var infoElement))
        {
            return new LiveryInfo(infoElement.GetProperty("workPath").ToString(),
                infoElement.GetProperty("preview").ToString());
        }
        return null;
    }
}