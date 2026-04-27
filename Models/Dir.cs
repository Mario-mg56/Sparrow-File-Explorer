using System;
using System.IO;
using DynamicFileExplorer.Infrastructures;

namespace DynamicFileExplorer.Models;

public class DirItem : FileSystemItem
{
    public DirItem(Path path) : base(path)
    {
    }

    public DirItem(string path) : base(path)
    {
    }

    public DirItem(DirectoryInfo info) : base (info.FullName)
    {
        
    }

    public DirItem? GetSubDir(string name)
    {
        FileManager fm = App.Current.fileManager 
        ?? throw new Exception("fileManager es null");
        
        return fm.ListAllDirectories(this).Find(s => s.path.name.Equals(name));
    }

    public DirectoryInfo GetInfo()
    {
        return new DirectoryInfo(GetPath());
    }

    public override string GetPath() => path.path;

}