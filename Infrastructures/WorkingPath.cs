using System;
using System.IO;
using System.Collections.Generic;
using DynamicFileExplorer.Models;
using System.Linq;
using System.IO.Enumeration;

namespace DynamicFileExplorer.Infrastructures;

class WorkingPath
{
    private string ActualPath;

    public WorkingPath(string actualPath)
    {
        ActualPath = actualPath;
    } 

    public  List<FileSystemEntry> List() 
    {
        List<FileSystemEntry> files;
        


        return Directory.GetFiles(ActualPath).ToList().Select(i => new FileItem(i)).ToList();
    }

    public bool Delete(FileSystemItem item)
    {
        return true;
    }

    public bool CreateFolder(string name)
    {
        return true;
    }
    public bool CreateFile(string name)
    {
        return true;
    }

    public bool ChangeLocalDirectory(FolderItem folder)
    {
        return true;
    }

    public bool ChangeAbsoluteDirectory(FolderItem folder)
    {
        return true;
    }

    public bool GoBack()
    {
        return true;
    }
}