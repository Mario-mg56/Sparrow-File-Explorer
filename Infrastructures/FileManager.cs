namespace DynamicFileExplorer.Infrastructures;

using System;
using System.IO;
using static System.IO.Path;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicFileExplorer.Models;
using System.Threading.Tasks;
using File = Models.File;

public class FileManager
{
    private static FileManager? instance;
    public DirItem WorkingDir;
    public ObservableCollection<FileSystemItem> files = [];
    public List<FileSystemItem> SelectedItems {get; private set;} = [];
    private bool isHideItemsHide = App.Config.DefaultIsHideItems;

    private HistoryManager historyManager = new();
    public event Action<DirItem>? WorkingDirChanged;
    public event Action<List<FileSystemItem>>? FocusChanged;
    private readonly SearchManager searchManager;

    private FileManager(DirItem currentPath)
    {
        searchManager = new(this);
        WorkingDirChanged += (_) => LoadFiles();
        if (!ChangeDirectory(currentPath)) throw new ArgumentException("Invalid directory");
        WorkingDir = currentPath;
        
    } 

    public static FileManager Init(DirItem currentPath)
    {
        instance ??= new FileManager(currentPath);
        return instance;
    }

    public void GoBackward() => historyManager.GoBackward();
    public void GoForward() => historyManager.GoForward();
    
    public void Execute(Action action, string message)
    {
        historyManager.stackHaciaAlante(action, message);
        action(); // ejecutar UNA sola vez
    }

    public void CleanSearch() => searchManager.CleanSearch();
    public Task SearchWorkingDir(string word) {
        return searchManager.SearchWorkingDir(word);
    }

    private void LoadFiles()
    {
        files.Clear();
        ListAll().ForEach(files.Add);
    }

    public static DirItem? GetRoot()
    {
        string? path = GetPathRoot(Environment.CurrentDirectory);
        if (path==null) return null;
        return new DirItem(path);
    }

    public  List<FileSystemItem> ListAll() 
    {
        List<FileSystemItem> files = [];
        if(searchManager.IsEmpty()){
            foreach (var path in Directory.GetDirectories(WorkingDir.GetPath())
            .Where(IsNotHidden)
            .OrderBy(d => GetFileName(d),
            StringComparer.CurrentCultureIgnoreCase))
                files.Add(new DirItem(new Models.Path(path)));

            foreach (var path in Directory.GetFiles(WorkingDir.GetPath())
            .Where(IsNotHidden)
            .OrderBy(GetFileName,
            StringComparer.CurrentCultureIgnoreCase))
            files.Add(new Models.File(new Models.Path(path)));
        } else
        {
            searchManager.GetResults().ToList().ForEach(files.Add);
        }
        return files;
    }

    public bool IsNotHidden(string path)
    {
        if (!isHideItemsHide)return true;
        string? name = GetFileName(path);

        if (string.IsNullOrEmpty(name))
            return false;

        if (name.StartsWith('.'))
            return false;

        var attr = System.IO.File.GetAttributes(path);
        if ((attr & FileAttributes.Hidden) != 0)
        return false;

        return true;
    }
    public List<DirItem> ListAllDirectories(DirItem dir)
    {
         List<DirItem> files = [];

        foreach (var path in Directory.GetDirectories(dir.GetPath()).Where(IsNotHidden).OrderBy(d => GetFileName(d),
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
    public void Delete(FileSystemItem item)
    {
        if(item is Models.File f)
        {
             Delete(f);
        } else if (item is DirItem d)
        {
             Delete(d);
            
        }
        CastWorkingDirChanged();
    }
    public bool Delete(File item)
    {
        try
        {
            System.IO.File.Delete(item.path.path);
        } catch (IOException )
        {
            return false;
        }
        CastWorkingDirChanged();
        return true;
    }
    public bool Delete(DirItem item)
    {
        try
        {
            Directory.Delete(item.path.path,recursive:true);
        } catch (IOException )
        {
            return false;
        }
        return true;
    }

    public void Rename(FileSystemItem file,string newName)
    {
        System.IO.File.Move(file.path.path, Combine(file.path.PathWithoutName(),newName));
        CastWorkingDirChanged();
    }
    public void Move(File origen,Models.Path dest){
        Console.WriteLine(origen.path.path+" in "+Combine(dest.path,origen.path.name));
        System.IO.File.Move(origen.path.path, Combine(dest.path,origen.path.name));
        CastWorkingDirChanged();
    }
    public void Move(DirItem origen,Models.Path dest){
        Console.WriteLine(origen.path.path + " in " + dest.path);
        Directory.Move(origen.path.path, Combine(dest.path,origen.path.name));
        CastWorkingDirChanged();
    }


    public DirItem? CreateDir(DirItem dir,string name)
    {
        DirectoryInfo? result = null;
        try
        {
            result = Directory.CreateDirectory(Combine(dir.path.path,name));
            
        } catch (IOException)
        {
            return null;
        }
        CastWorkingDirChanged();
        return new DirItem(result.FullName);
    }
    public bool CreateFile(DirItem dir,string nameWithExtension)
    {
        try
        {
            System.IO.File.Create(Combine(dir.path.path,nameWithExtension));
            
        } catch (IOException)
        {
            return false;
        }
        CastWorkingDirChanged();
        return true;
    }

    public bool ChangeDirectory(DirItem dir)
    {
        Console.WriteLine(dir);
        if(!Directory.Exists(dir.GetPath())) return false;

        historyManager.stackHaciaAlante(()=>ChangeDirectoryEffect(dir),"Cambiando directorio a " + dir.GetPath());

        ChangeDirectoryEffect(dir);
        return true;
    }

    private void ChangeDirectoryEffect(DirItem dir)
    {
        WorkingDir = dir;
        searchManager.Clear();
        CastWorkingDirChanged();
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

        ChangeDirectory(new DirItem(parentPath));

        return instance;
    }
    
    public void CastWorkingDirChanged()
    {
        // Console.WriteLine("WD changed to " + WorkingDir.GetPath());
        LoadFiles();
        WorkingDirChanged?.Invoke(WorkingDir);
    }
    private void CastFocusChanged()
    {
        FocusChanged?.Invoke(SelectedItems);
    }
   
    public void ChangeHideItems(bool? state)
    {
        if (!state.HasValue)
            return;
        isHideItemsHide = state.Value;
        CastWorkingDirChanged();
    }

    public void PrintList()
    {
        ListAll().ForEach(Console.WriteLine);
    }
}