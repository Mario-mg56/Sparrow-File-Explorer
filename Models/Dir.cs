namespace DynamicFileExplorer.Models;

class DirItem : FileSystemItem
{
    public DirItem(Path path) : base(path)
    {
    }

    public DirItem(string path) : base(path)
    {
    }

    public override string GetPath() => path.path;

}