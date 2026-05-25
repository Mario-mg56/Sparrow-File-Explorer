using DynamicFileExplorer.Helpers;

namespace DynamicFileExplorer.UI.Persistence;

public class CacheData
{
    public string LastDir { get; set; } = FileManager.GetRoot()?.GetPath() ?? "";
    public string BgImage { get; set; } = "";
    public string CurrentTheme { get; set; } = "default_theme";
}