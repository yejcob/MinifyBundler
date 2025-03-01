using System.Text.RegularExpressions;

namespace WF.MinifyBundler;

public sealed record ConfigParser
{
    public static List<CompilerOptions> ParseJson(string json)
    {
        var options = new List<CompilerOptions>();
        json = json.Trim();
        json = Regex.Replace(json, @"\s", "");
        json = json.Trim('[', ']');
        string[] objects = Regex.Split(json, "},{");
        
        foreach (var obj in objects)
        {
            var formattedObj = obj.Trim('{', '}');
            var option = new CompilerOptions();
            var properties = formattedObj.Split(',');
            
            foreach (var property in properties)
            {
                var keyValue = property.Split([":"], StringSplitOptions.None);
                if (keyValue.Length < 2) continue;
                
                var key = keyValue[0].Trim(' ', '"');
                var value = keyValue[1].Trim(' ', '"');
                
                switch (key)
                {
                    case "outputPath":
                        option.OutputPath = value;
                        break;
                    case "sourceFolders":
                        option.SourceFolders = value.Trim('[', ']').Replace("\"", "").Split([","], StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case "fileType":
                        option.FileType = value;
                        break;
                }
            }
            options.Add(option);
        }
        return options;
    }
}