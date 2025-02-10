using System.Runtime.Serialization;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using Task = Microsoft.Build.Utilities.Task;

namespace WF.MinifyBundler;

public class Bundler : Task
{
    public DateTime TimeProvided { get; set; } = DateTime.UtcNow;
    public IFileWrapper FileWrapper { get; set; } = new FileWrapper();

    public string CompilerSettingsFile { get; set; } = null!;

    [Output] public ITaskItem[] GeneratedFiles { get; private set; } = [];

    private IEnumerable<CompilerOptions> _compilerOptions = [];
    private readonly List<ITaskItem> _generatedFiles = [];

    public override bool Execute()
    {
        ReadConfigFile();

        foreach (var compilerOption in _compilerOptions)
        {
            var sourceFiles = new List<FileInfo>();

            foreach (var sourceFolder in compilerOption.SourceFolders)
            {
                sourceFiles.AddRange(FileWrapper.GetFiles(sourceFolder, compilerOption.FileType));
            }

            var maxSourceWriteTime = TimeProvided;

            maxSourceWriteTime = sourceFiles.Aggregate(maxSourceWriteTime, (current, sourceFile) => sourceFile.LastWriteTime > current ? sourceFile.LastWriteTime : current);

            using var mutex = new Mutex(initiallyOwned: false, "WFMinifyBundler");

            mutex.WaitOne();

            if (FileWrapper.Exists(compilerOption.OutputPath))
            {
                if (FileWrapper.GetLastWriteTime(compilerOption.OutputPath) < maxSourceWriteTime)
                {
                    WriteFile(compilerOption.OutputPath, sourceFiles.ToArray());
                    Log.LogMessage(MessageImportance.High, $"{compilerOption.OutputPath} Updated");
                }
                else
                {
                    Log.LogMessage(MessageImportance.High, $"{compilerOption.OutputPath} UpToDate");
                }
            }
            else
            {
                WriteFile(compilerOption.OutputPath, sourceFiles.ToArray());
                Log.LogMessage(MessageImportance.High, $"{compilerOption.OutputPath} Created");
            }

            mutex.ReleaseMutex();

            _generatedFiles.Add(new TaskItem(compilerOption.OutputPath));
        }

        GeneratedFiles = _generatedFiles.ToArray();
        return true;
    }

    private void ReadConfigFile()
    {
        if (!FileWrapper.Exists(CompilerSettingsFile)) return;

        try
        {
            var text = File.ReadAllText(CompilerSettingsFile);
            if (!string.IsNullOrWhiteSpace(text))
            {
                _compilerOptions = JsonConvert.DeserializeObject<IEnumerable<CompilerOptions>>(text)!;
            }
            else
            {
                Log.LogWarning("compilerSettings.json exists but is empty, ignoring it.");
            }
        }
        catch (SerializationException ex)
        {
            Log.LogError("compilerSettings.json is invalid: {0}", ex.Message);
        }
        catch (Exception ex)
        {
            Log.LogError("Unable to read compilerSettings.json: {0}", ex.ToString());
        }
    }

    private void WriteFile(string destinationFile, FileInfo[] sourceFiles)
    {
        var bundled = new StringBuilder();

        foreach (var sourceFile in sourceFiles)
        {
            var fileText = FileWrapper.ReadAllText(sourceFile.FullName);
            bundled.Append(fileText);
        }
        
        var compressed = Minifier.Minify(bundled.ToString());
        FileWrapper.WriteAllText(destinationFile, compressed);
    }
}