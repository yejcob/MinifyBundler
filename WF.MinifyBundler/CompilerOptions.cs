namespace WF.MinifyBundler;

public class CompilerOptions
{
    public string OutputPath { get; set; } = string.Empty;
    public string[] SourceFolders { get; set; } = [];
    public string FileType { get; set; } = string.Empty;
}