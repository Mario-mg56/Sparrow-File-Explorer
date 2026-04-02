using System;

namespace DynamicFileExplorer.Models;

abstract class FileSystemItem
{
    public string Name { get; set; }
    public string Path { get; set; }

    protected FileSystemItem(PathFile name)
    {
        Name = IOPathFileToName(name);
        Path = IOPathFileToPath(name);
    }

    protected FileSystemItem(string path, string name)
    {
        Name = name;
        Path = path;
    }

    /**
    Este constructor es para archivos locales
    */
    
    protected FileSystemItem(string localPath)
    {
        Path = App.Current.fileManager.WorkingFolder.GetFullPath();
        Name = localPath;
    }
    private string IOPathFileToName(PathFile name)
    {
        return System.IO.Path.GetFileNameWithoutExtension(name.Path);
    }

    private string IOPathFileToPath(PathFile name)
    {
        return System.IO.Path.GetDirectoryName(name.Path) ?? throw new ArgumentException($"Invalid path: {name.Path}");
    }

    public virtual string GetFullPath()
    {
        return Path+"/"+Name;
    }

    public override string ToString()
    {
        return GetFullPath();
    }
}
