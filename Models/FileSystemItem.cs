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

    protected FileSystemItem(String path, String name)
    {
        Name = name;
        Path = path;
    }

    private string IOPathFileToName(PathFile name)
    {
        return System.IO.Path.GetFileNameWithoutExtension(name.Path);
    }

    private string IOPathFileToPath(PathFile name)
    {
        return System.IO.Path.GetFullPath(name.Path);
    }

    public virtual string GetFullPath()
    {
        return Path+Name;
    }
}
