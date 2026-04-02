using System;
using Avalonia.Media;
using DynamicFileExplorer.Models;

namespace DynamicFileExplorer.ViewModels;

class FileViewModel{
    public string name;
    public SolidColorBrush icon;
    public FileSystemItem file;

    public FileViewModel(FileSystemItem file)
    {
        this.file = file;
        name = file.Name;
        
        var rnd = new Random();
        icon = new SolidColorBrush(Color.FromRgb
            ((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)));
    }

    public override string ToString()
    {
        return name;
    }
}