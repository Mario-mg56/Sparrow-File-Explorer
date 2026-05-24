using System;
using System.IO;
using DynamicFileExplorer.Models;
namespace DynamicFileExplorer.Infrastructures;
public class CacheService : JsonStorageService
{
    private string CachePath
    {
        get
        {
            var dir = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SparrowFileExplorer"
            );

            Directory.CreateDirectory(dir);

            return System.IO.Path.Combine(dir, "cache.json");
        }
    }

    public CacheData Cache { get; private set; } = new();


    public CacheService()
    {
        FileManager? lastTab = null;
        App.Current.tabManager.TabFocused += tab => {
            lastTab?.WorkingDirChanged -= UpdateLastDir;
            tab?.fileManager?.WorkingDirChanged += UpdateLastDir;
        };
    }
    public CacheService Load()
    {
        Cache = Load<CacheData>(CachePath);
        return this;
    }


    public void UpdateLastDir(DirItem LastDir)
    {
        Cache.LastDir = LastDir.GetPath();
        Save();

    }

    public void UpdateBgImage(string bgImage)
    {
        Cache.BgImage = bgImage;
        Save();
    }

    public DirItem ImportLastDir()
    {
        return new DirItem(new Models.Path(Cache.LastDir));
    }

    public void Save()
    {
        Save(CachePath, Cache);
    }
}
