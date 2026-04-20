using System;

namespace DynamicFileExplorer.Models;

public class File : FileSystemItem
{
    public readonly string extension;
    public File(Path path) : base(path)
    {
        extension = System.IO.Path.GetExtension(path.path);
    }
    public File(string localPath, string extension) : base(localPath)
    {
        this.extension = extension;
    }   

    public override string GetPath() => path.path;


    public string NameWithoutExtension()
    {
        if (string.IsNullOrEmpty(extension))
        return path.name;

        return path.name.Replace(extension, "");
    }
    
}