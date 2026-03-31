using System;
using System.IO;
using Avalonia.Media;
using DynamicFileExplorer.Models;

namespace DynamicFileExplorer.ViewModels;

class FileViewModel{
    public string name;
    public SolidColorBrush icon;
    public FileItem File;
    public FolderItem Folder;

    public bool IsFolder;

    public FileViewModel(FileSystemItem file,bool isFolder)
    {
        IsFolder = isFolder;
        if (isFolder)
        {
            Folder = (FolderItem)file;
        } else
        {
            File = (FileItem)file;
            
        }
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