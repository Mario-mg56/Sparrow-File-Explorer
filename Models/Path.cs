namespace DynamicFileExplorer.Models;

using System;
using static System.IO.Path;

public readonly struct Path
{
    public readonly string path, name;

    public Path(string path)
    {
        if (!(System.IO.File.Exists(path) || System.IO.Directory.Exists(path)))
            throw new ArgumentException($"Invalid path: {path}");
        this.path = path;
        name = GetFileName(path);
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