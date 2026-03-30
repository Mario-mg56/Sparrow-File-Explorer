using System;
using System.IO;
using System.Collections.Generic;
namespace DynamicFileExplorer.Infrastructures;

using System.IO.Enumeration;
using System.Linq;
using DynamicFileExplorer.Models;

class WorkingPath
{
    private FolderItem WorkingFolder;
    private List<FileSystemItem> SelectedItems;
    public event Action<FolderItem> WorkingFolderChanged;
    public event Action<List<FileSystemItem>> FocusChanged;

    public WorkingPath(FolderItem actualPath)
    {
        WorkingFolder = actualPath;
    } 

    public  List<FileSystemItem> ListAll() 
    {
        List<FileSystemItem> files = [];

        foreach (var item in  Directory.GetDirectories(WorkingFolder.Path))
        {
            files.Add(new FolderItem(new PathFile(item)));
        } 
        foreach (var item in  Directory.GetFiles(WorkingFolder.Path))
        {
            files.Add(new FileItem(new PathFile(item)));
        } 

        return files;
    }

    public bool Delete(FileItem item)
    {
        try
        {
            File.Delete(item.Path + item.Name);
        } catch (IOException )
        {
            return false;
        }
        return true;
    }
    public bool Delete(FolderItem item)
    {
        try
        {
            Directory.Delete(item.Path + item.Name,recursive:true);
        } catch (IOException )
        {
            return false;
        }
        return true;
    }

    public bool CreateFolder(FolderItem folder)
    {
        try
        {
            Directory.CreateDirectory(folder.GetFullPath());
            
        } catch (IOException)
        {
            return false;
        }
        return true;
    }
    public bool CreateFile(FileItem file)
    {
        try
        {
            Directory.CreateDirectory(file.GetFullPath());
            
        } catch (IOException)
        {
            return false;
        }
        return true;
    }

    public bool ChangeDirectory(FolderItem folder)
    {
        if(!Directory.Exists(folder.GetFullPath()))return false;
        WorkingFolder = folder;
        CastWorkingFolderChanged();
        return true;
    }
    public bool ChangeDirectoryByIndex(int index)
    {
        FolderItem folder = (FolderItem)ListAll()[index];
        if(!Directory.Exists(folder.GetFullPath()))return false;
        WorkingFolder = folder;
        CastWorkingFolderChanged();
        return true;
    }

    public bool Open()
    {
        if(SelectedItems.Count()!=1)return false;
        
        if(SelectedItems[0] is FolderItem folder)
        {
            ChangeDirectory(folder);
            return true;
        }
        return false;

    }

    public bool SelectItems(List<FileSystemItem> items)
    {
        foreach (var item in items)
        {
            if (item is FileItem file)
            {
                if(!File.Exists(file.GetFullPath()))return false;
            } if (item is FolderItem folder)
            {
                if(!Directory.Exists(folder.GetFullPath()))return false;
            }
        }
        SelectedItems = items;
        CastFocusChanged();
        return true;
    }

    public bool Select(FileItem file)
    {
        if(!File.Exists(file.GetFullPath()))return false;
        SelectedItems.Clear();
        SelectedItems.Add(file);
        CastFocusChanged();
        return true;
    }
    public bool Select(FolderItem folder)
    {
        if(!Directory.Exists(folder.GetFullPath()))return false;
        SelectedItems.Clear();
        SelectedItems.Add(folder);
        CastFocusChanged();
        return true;
    }

    public bool GoBack()
    {
       var parentPath = new DirectoryInfo(WorkingFolder.GetFullPath()).Parent?.FullName;

        if (parentPath == null)
            return false;

        WorkingFolder = new FolderItem(new PathFile(parentPath));
        CastWorkingFolderChanged();

        return true;
    }

    private void CastWorkingFolderChanged()
    {
        WorkingFolderChanged?.Invoke(WorkingFolder);
    }
    private void CastFocusChanged()
    {
        FocusChanged?.Invoke(SelectedItems);
    }
}