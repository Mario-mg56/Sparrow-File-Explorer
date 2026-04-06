using System;
using Avalonia.Media;

namespace DynamicFileExplorer.Models;

public abstract class FileSystemItem
{
    public SolidColorBrush icon;

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

    private static SolidColorBrush GenerateIconPlaceholder()
    {   
        var rnd = new Random();
        return new SolidColorBrush(Color.FromRgb
            ((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)));
    }
}
