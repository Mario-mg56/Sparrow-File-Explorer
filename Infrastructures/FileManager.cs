namespace DynamicFileExplorer.Infrastructures;

using System;
using System.IO;
using static System.IO.Path;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicFileExplorer.Models;

class FileManager
{
    private static FileManager? instance;
    public DirItem WorkingDir;
    private List<FileSystemItem> searchWorkingDir = [];
    public ObservableCollection<FileSystemItem> files = [];
    private List<FileSystemItem> SelectedItems = [];
    public event Action<DirItem>? WorkingDirChanged;
    public event Action<List<FileSystemItem>>? FocusChanged;

    private FileManager(DirItem currentPath)
    {
        WorkingDirChanged += (_) => LoadFiles();
        if (!ChangeDirectory(currentPath)) throw new ArgumentException("Invalid directory");
        WorkingDir = currentPath;
        
    } 

    public static FileManager Init(DirItem currentPath)
    {
        instance ??= new FileManager(currentPath);
        return instance;
    }

    private void LoadFiles()
    {
        files.Clear();
        ListAll().ForEach(files.Add);
    }

    public DirItem? GetRoot()
    {
        string? path = GetPathRoot(Environment.CurrentDirectory);
        if (path==null) return null;
        return new DirItem(path);
    }

    public  List<FileSystemItem> ListAll() 
    {
        List<FileSystemItem> files = [];
        if(searchWorkingDir.Count==0){
            foreach (var path in Directory.GetDirectories(WorkingDir.GetPath()).OrderBy(d => GetFileName(d),
            StringComparer.CurrentCultureIgnoreCase))
                files.Add(new DirItem(new Models.Path(path)));

            foreach (var path in Directory.GetFiles(WorkingDir.GetPath()).OrderBy(d => GetFileName(d),
            StringComparer.CurrentCultureIgnoreCase))
            files.Add(new Models.File(new Models.Path(path)));
        } else
        {
            searchWorkingDir.ForEach(files.Add);
        }
        return files;
    }
    public List<DirItem> ListAllDirectories(DirItem dir)
    {
         List<DirItem> files = [];

        foreach (var path in Directory.GetDirectories(dir.GetPath()).OrderBy(d => GetFileName(d),
         StringComparer.CurrentCultureIgnoreCase))
            files.Add(new DirItem(new Models.Path(path)));

        return files;
    }

    public List<DirItem> GetAllFathers()
    {
        List<DirItem> fathersAndSon = [];
        DirItem dir = WorkingDir;
        fathersAndSon.Add(dir);

        while (dir != null)
        {
            // lógica

            var parent = Directory.GetParent(dir.GetPath());
            if (parent == null) break;

            dir = new DirItem(parent);
            fathersAndSon.Add(dir);
        }
        return fathersAndSon;
    }

    public bool Delete(Models.File item)
    {
        try
        {
            System.IO.File.Delete(item.path + item.path.name);
        } catch (IOException )
        {
            return false;
        }
        return true;
    }
    public bool Delete(DirItem item)
    {
        try
        {
            Directory.Delete(item.GetPath(),recursive:true);
        } catch (IOException )
        {
            return false;
        }
        return true;
    }

    public bool CreateDir(DirItem dir)
    {
        try
        {
            Directory.CreateDirectory(dir.GetPath());
            
        } catch (IOException)
        {
            return false;
        }
        return true;
    }
    public bool CreateFile(Models.File file)
    {
        try
        {
            System.IO.File.Create(file.GetPath());
            
        } catch (IOException)
        {
            return false;
        }
        return true;
    }

    public bool ChangeDirectory(DirItem dir)
    {
        Console.WriteLine(dir);
        if(!Directory.Exists(dir.GetPath())) return false;
        WorkingDir = dir;
        CastWorkingDirChanged();
        return true;
    }
    public bool ChangeDirectoryByIndex(int index)
    {
        return ChangeDirectory((DirItem)ListAll()[index]);
    }

    public FileManager? Open()
    {
        if(SelectedItems.Count()!=1) return null;   
        
        if(SelectedItems[0] is DirItem dir)
        {
            ChangeDirectory(dir);
            return instance;
        }
        return null;

    }

    public bool SelectItems(List<FileSystemItem> items)
    {
        foreach (var item in items)
        {
            if (item is Models.File file)
            {
                if(!System.IO.File.Exists(file.GetPath()))return false;
            } if (item is DirItem dir)
            {
                if(!Directory.Exists(dir.GetPath()))return false;
            }
        }
        SelectedItems = items;
        CastFocusChanged();
        return true;
    }

    public FileManager? Select(Models.File file)
    {
        if(SelectedItems.Any() && file == SelectedItems[0])
        {
            Open();
            return instance;
        }
        if(!System.IO.File.Exists(file.GetPath()))return null;
        SelectedItems.Clear();
        SelectedItems.Add(file);
        CastFocusChanged();
        return instance;
    }
    public FileManager? Select(DirItem dir)
    {
        Console.WriteLine("se ha selecionado" + dir.path.name);
        if(SelectedItems.Any() && dir == SelectedItems[0])
        {
            Open();
            return instance;
        }
        if(!Directory.Exists(dir.GetPath()))return null;
        SelectedItems.Clear();
        SelectedItems.Add(dir);
        CastFocusChanged();
        return instance;
    }

    public FileManager? GoBack() 
    {
        var parentPath = new DirectoryInfo(WorkingDir.GetPath()).Parent?.FullName;

        if (parentPath == null)return null;

        WorkingDir = new DirItem(new Models.Path(parentPath));
        CastWorkingDirChanged();

        return instance;
    }

    private void CastWorkingDirChanged()
    {
        Console.WriteLine("WD changed to " + WorkingDir.GetPath());
        LoadFiles();
        WorkingDirChanged?.Invoke(WorkingDir);
    }
    private void CastFocusChanged()
    {
        FocusChanged?.Invoke(SelectedItems);
    }

    public void SearchWorkingDir(string word)
    {
        

        searchWorkingDir.Clear();

        var stack = new Stack<string>();
        stack.Push(WorkingDir.path.path);

        while (stack.Count > 0)
        {
            var dir = stack.Pop();

            try
            {
            // Archivos
                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    Console.WriteLine(file);
                    if (file.Contains(word))
                    {
                        searchWorkingDir.Add(
                            new Models.File(new Models.Path(file))
                        );
                    }
                }

                // Subdirectorios
                foreach (var subDir in Directory.EnumerateDirectories(dir))
                {
                    if (subDir.Contains(word))
                    {
                        searchWorkingDir.Add(
                            new DirItem(new Models.Path(subDir))
                        );
                    }
                    stack.Push(subDir);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignorar carpetas sin permisos (/root, /proc…)
            }
            catch (IOException)
            {
                // Ignorar errores raros
            }
        }

        CastWorkingDirChanged();
    }


    public void PrintList()
    {
        ListAll().ForEach(Console.WriteLine);
    }
}