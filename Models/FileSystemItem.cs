using System;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace DynamicFileExplorer.Models;
public abstract class FileSystemItem
{
    public IBrush icon;
    public Path path;
    protected FileSystemItem(Path path)
    {
        this.path = path;
        icon = GenerateIconPlaceholder();
    }

    protected FileSystemItem(string path)
    {
        this.path = new Path(path);
        icon = GenerateIconPlaceholder();
    }

    public abstract string GetPath();
    public override string ToString()
    {
        return GetPath();
    }

    private static IBrush GenerateIconPlaceholder()
    {   
        var rnd = new Random();
        return new ImmutableSolidColorBrush(Color.FromRgb
            ((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)));
    }

    public bool Equals(FileSystemItem obj)
    {
        return obj.GetPath()==GetPath();
    }

    public bool Exists()
    {
        return System.IO.Path.Exists(GetPath());
    }
}