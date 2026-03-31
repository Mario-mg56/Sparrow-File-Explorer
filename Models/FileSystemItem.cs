using System;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using DynamicFileExplorer.Infrastructures;

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

    /**
    Este constructor es para archivos locales
    */
    
    protected FileSystemItem(String localPath)
    {
        Path = FileManager.GetInstance().WorkingFolder.GetFullPath();
        Name = localPath;
    }
    private string IOPathFileToName(PathFile name)
    {
        return System.IO.Path.GetFileNameWithoutExtension(name.Path);
    }

    private string IOPathFileToPath(PathFile name)
    {
        string ?tusmuertos = "chupalo";
        try
        {
            tusmuertos = System.IO.Path.GetDirectoryName(name.Path);

        }
        catch (Exception)
        {
            Console.WriteLine("chupaloputa");
        }
        return tusmuertos;
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
