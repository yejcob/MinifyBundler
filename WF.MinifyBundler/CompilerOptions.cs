using System.Text.Json.Serialization;

namespace WF.MinifyBundler;

public class CompilerOptions
{
    public string OutputPath { get; set; } = string.Empty;
    public string[] SourceFolders { get; set; } = [];
    public string FileType { get; set; } = string.Empty;
}

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<CompilerOptions>))]
internal partial class CompilerOptionsContext : JsonSerializerContext
{
}