using System;
using Avalonia.Media;

namespace DynamicFileExplorer.Placeholders;

class FilePlaceholder{
    public string name;
    public SolidColorBrush icon;

    public FilePlaceholder(string name)
    {
        this.name = name;
        
        var rnd = new Random();
        icon = new SolidColorBrush(Color.FromRgb
            ((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)));
    }
}