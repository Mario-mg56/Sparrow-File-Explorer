using System;
using System.IO;
using System.Collections.Generic;
namespace DynamicFileExplorer.Infrastructures;
using System.Linq;
using DynamicFileExplorer.Models;

class FileManager
{
    public FolderItem WorkingFolder;
    private List<FileSystemItem> SelectedItems = [];
    public event Action<FolderItem> ?WorkingFolderChanged;
    public event Action<List<FileSystemItem>> ?FocusChanged;
    private static FileManager ?_fileManager;


    private FileManager(FolderItem actualPath)
    {
        WorkingFolder = actualPath;
    } 

    public static FileManager GetInstance(FolderItem actualPath)
    {
        _fileManager ??= new FileManager(actualPath);
        return _fileManager;
    }
    public static FileManager? GetInstance()
    {
        if(_fileManager==null){
            Console.WriteLine("El file Manager debe estar inicializado para usar el sc");
            return null;
        }
        return _fileManager;
    }

    public  List<FileSystemItem> ListAll() 
    {
        List<FileSystemItem> files = [];

        foreach (var item in  Directory.GetDirectories(WorkingFolder.GetFullPath()).OrderBy(d => Path.GetFileName(d),
                             StringComparer.CurrentCultureIgnoreCase))
        {
            files.Add(new FolderItem(new PathFile(item)));
        } 
        foreach (var item in  Directory.GetFiles(WorkingFolder.GetFullPath()).OrderBy(d => Path.GetFileName(d),
                             StringComparer.CurrentCultureIgnoreCase))
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
            Directory.Delete(item.GetFullPath(),recursive:true);
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
            File.Create(file.GetFullPath());
            
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

    public FileManager? Open()
    {
        if(SelectedItems.Count()!=1)return null;
        
        if(SelectedItems[0] is FolderItem folder)
        {
            ChangeDirectory(folder);
            return _fileManager;
        }
        return null;

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

    public FileManager? Select(FileItem file)
    {
        if(SelectedItems.Any() && file == SelectedItems[0])
        {
            
            Open();
            return _fileManager;
        }
        if(!File.Exists(file.GetFullPath()))return null;
        SelectedItems.Clear();
        SelectedItems.Add(file);
        CastFocusChanged();
        return _fileManager;
    }
    public FileManager? Select(FolderItem folder)
    {
        Console.WriteLine("se ha selecionado" + folder.Name);
        if(SelectedItems.Any() && folder == SelectedItems[0])
        {
            Open();
            return _fileManager;
        }
        if(!Directory.Exists(folder.GetFullPath()))return null;
        SelectedItems.Clear();
        SelectedItems.Add(folder);
        CastFocusChanged();
        return _fileManager;
    }

    public FileManager? GoBack() 
    {
        var parentPath = new DirectoryInfo(WorkingFolder.GetFullPath()).Parent?.FullName;

        if (parentPath == null)return null;

        WorkingFolder = new FolderItem(new PathFile(parentPath));
        CastWorkingFolderChanged();

        return _fileManager;
    }

    private void CastWorkingFolderChanged()
    {
        WorkingFolderChanged?.Invoke(WorkingFolder);
    }
    private void CastFocusChanged()
    {
        FocusChanged?.Invoke(SelectedItems);
    }

    public void PrintList()
    {
        ListAll().ForEach(Console.WriteLine);
    }
    // public static void Init()
    // {   FileManager fm = FileManager.GetInstance(new FolderItem(new PathFile("/home/diego/proyectos/interfaces/DynamicFileExplorer")));
    //     fm.Select(new FolderItem("bin")).Open().Select(new FolderItem("Debug")).Open();
    //     fm.PrintList();
    // }
}