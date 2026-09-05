using System.Text;

namespace airlineRider.Utils;

public class CsvUtils
{
    public static List<String> ParseLine(string line, char delimiter)
    {
        var values = new List<string>();
        if (line.IsWhiteSpace())
        {
            return values;
        }
        var inQuotes = false;
        var stringBuilder = new StringBuilder(); 
        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == delimiter && !inQuotes)
            {
                values.Add(stringBuilder.ToString());
                stringBuilder.Clear();
            }
            else
            {
                stringBuilder.Append(c);
            }
        }
        values.Add(stringBuilder.ToString());
        return values;
    }
}