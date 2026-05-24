namespace DynamicFileExplorer.Models;

using System;
using System.Linq;
using static System.IO.Path;

public readonly struct Path
{
    public readonly string path, name;
    public static readonly Path BasePath = new(AppContext.BaseDirectory.Last() == DirectorySeparatorChar ? AppContext.BaseDirectory[..^1] : AppContext.BaseDirectory);

    public Path(string path)
    {
        if (!(System.IO.File.Exists(path) || System.IO.Directory.Exists(path)))
            throw new ArgumentException($"Invalid path: {path}");
        this.path = path;
        name = GetFileName(path);
        // Console.WriteLine("p: " + path + " : " + name);
    }
    public string PathWithoutName()
    {
        return path.Replace(name,"");
    }

    public override string ToString() => path;

    public static bool IsRootPath(string path)
    {
        string normalizedPath = GetFullPath(path);
        return normalizedPath.Equals(GetPathRoot(normalizedPath));
    }
}