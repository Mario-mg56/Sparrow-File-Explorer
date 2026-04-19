namespace DynamicFileExplorer.Infrastructures;

public class CacheData
{
    public string LastDir { get; set; } = FileManager.GetRoot()?.GetPath() ?? "";
    public string BgImage { get; set; } = "";
}