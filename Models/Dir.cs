using System.IO;

namespace DynamicFileExplorer.Models;

public class DirItem : FileSystemItem
{
    public DirItem(Path path) : base(path)
    {
    }

    public DirItem(string path) : base(path)
    {
    }

    public DirItem(DirectoryInfo info) : base (info.FullName)
    {
        
    }

    public override string GetPath() => path.path;

}