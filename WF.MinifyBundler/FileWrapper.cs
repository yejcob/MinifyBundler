namespace WF.MinifyBundler;

public interface IFileWrapper
{
    bool Exists(string path);
    DateTime GetLastWriteTime(string path);
    string ReadAllText(string path);
    void WriteAllText(string path, string compressedContent);
    FileInfo[] GetFiles(string basePath, string fileType);
}

public class FileWrapper : IFileWrapper
{
    public bool Exists(string path) => File.Exists(path);
    public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);
    public string ReadAllText(string path) => File.ReadAllText(path);
    public void WriteAllText(string path, string compressedContent) => File.WriteAllText(path, compressedContent);
    public FileInfo[] GetFiles(string basePath, string fileType) => new DirectoryInfo(basePath).GetFiles($"*.{fileType}");
}
