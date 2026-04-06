namespace DynamicFileExplorer.Models;

class File : FileSystemItem
{
    readonly string extension;
    public File(Path path) : base(path)
    {
        extension = System.IO.Path.GetExtension(path.path);
    }
    public File(string localPath, string extension) : base(localPath)
    {
        this.extension = extension;
    }   

    public override string GetPath() => System.IO.Path.Combine(path.path, extension);
    
}