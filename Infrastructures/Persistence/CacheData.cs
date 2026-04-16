namespace DynamicFileExplorer.Infrastructures;

public class CacheData
{
    public string LastDir =
    FileManager.GetRoot()?.GetPath() ?? "";
}