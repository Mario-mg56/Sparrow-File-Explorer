namespace DynamicFileExplorer.Helpers;

using System;
using System.IO;
using static System.IO.Path;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicFileExplorer.Models;
using System.Threading.Tasks;
using File = Models.File;
using System.Diagnostics;
using DynamicFileExplorer.Infrastructures.Helpers;
using DynamicFileExplorer.Infrastructures;
using Avalonia.Threading;

public class FileManager
{
    public DirItem WorkingDir;
    public ObservableCollection<FileSystemItem> files = [];
    public List<FileSystemItem> SelectedItems {get; private set;} = [];
    private static bool ShowHiddenItems {get => App.Current.Cache.Config.DefaultIsHideItems;}

    private HistoryManager historyManager = new();
    public event Action<DirItem>? WorkingDirChanged;
    private readonly SearchManager searchManager;

    public FileManager(DirItem currentPath)
    {
        searchManager = new(this);
        if (!ChangeDirectory(currentPath)) throw new ArgumentException("Invalid directory");
        WorkingDir = currentPath;
        
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

    public async Task LoadFiles()
    {
        files.Clear();
        
        if (!searchManager.IsEmpty())
        {
            searchManager.GetResults().ToList().ForEach(files.Add);
            return;
        }

        var currentPath = WorkingDir.GetPath();
        bool showHidden = ShowHiddenItems;
        bool createContextMenuUse = AppArguments.IsUse(AppUse.CreateContextMenu);

        await Task.Run(async () =>
        {
            var batch = new List<FileSystemItem>();
            const int BATCH_SIZE = 50;

            try {
                foreach (var path in Directory.EnumerateDirectories(currentPath))
                {
                    if (!IsNotHidden(path)) continue;
                    batch.Add(new DirItem(new Models.Path(path)));

                    if (batch.Count >= BATCH_SIZE)
                    {
                        var copy = batch.ToList();
                        batch.Clear();
                        await Dispatcher.UIThread.InvokeAsync(() => copy.ForEach(files.Add));
                    }
                }
                
                foreach (var path in Directory.EnumerateFiles(currentPath))
                {
                    if (createContextMenuUse && !GetExtension(path).Equals(".sh", StringComparison.OrdinalIgnoreCase)) continue;
                    if (!IsNotHidden(path)) continue;

                    batch.Add(new File(new Models.Path(path)));

                    if (batch.Count >= BATCH_SIZE)
                    {
                        var copy = batch.ToList();
                        batch.Clear();
                        await Dispatcher.UIThread.InvokeAsync(() => copy.ForEach(files.Add));
                    }
                }

                if (batch.Count > 0)
                {
                    var copy = batch.ToList();
                    await Dispatcher.UIThread.InvokeAsync(() => copy.ForEach(files.Add));
                }
            } 
            catch (UnauthorizedAccessException) {}
        });
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
                .Where(f => 
                {
                    if (AppArguments.IsUse(AppUse.CreateContextMenu))
                    {
                        return GetExtension(f).Equals(".sh", StringComparison.OrdinalIgnoreCase);
                    }
                    
                    return IsNotHidden(f);
                })
                .OrderBy(GetFileName, StringComparer.CurrentCultureIgnoreCase))
            {
                files.Add(new File(new Models.Path(path)));
            }
        } else
        {
            searchManager.GetResults().ToList().ForEach(files.Add);
        }
        return files;
    }

    public static bool IsNotHidden(string path)
    {
        if (!ShowHiddenItems)return true;
        string? name = GetFileName(path);

        if (string.IsNullOrEmpty(name))
            return false;

        if (name.StartsWith('.'))
            return false;

        var attr = System.IO.File.GetAttributes(path);

        if ((attr & FileAttributes.Hidden) != 0) return false;
        return true;
    }
    public static List<DirItem> ListAllDirectories(DirItem dir)
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
        if(item is File) System.IO.File.Delete(item.path.path);
        else if (item is DirItem)  Directory.Delete(item.path.path,recursive:true);
            
        CastWorkingDirChanged();
    }

    public void Rename(FileSystemItem file, string newName)
    {
        System.IO.File.Move(file.path.path, Combine(file.path.PathWithoutName(),newName));
        CastWorkingDirChanged();
    }


    public void MoveItems(List<FileSystemItem> items, DirItem dest)
    {
        items.OfType<FileSystemItem>()
                .Where(f=>!f.Equals(dest))
                .ToList()
                .ForEach(f => Move(f, dest.path));
        CastWorkingDirChanged();
    }
    public void Move(FileSystemItem origen,Models.Path dest){
        Console.WriteLine(origen.path.path+" in "+Combine(dest.path,origen.path.name));
        try
        {
            System.IO.File.Move(origen.path.path, Combine(dest.path,origen.path.name));
            
        } catch(FileNotFoundException )
        {
            if(!Exists(Combine(dest.path,origen.path.name))) Console.WriteLine("No se pudo mover");
        }
        CastWorkingDirChanged();
    }
    

    public DirItem? CreateDir(DirItem dir, string name)
    {
        DirectoryInfo? result;
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
            System.IO.File.Create(Combine(dir.path.path,nameWithExtension)).Dispose();
            
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
        if (index < 0 || index >= files.Count) return false;
        return ChangeDirectory((DirItem)files[index]);
    }

    public FileManager? Open(FileSystemItem item)
    {
        
        if(item is DirItem dir)
        {
            ChangeDirectory(dir);
            return this;
        } else if (item is File file)
        {
            var value = OpenToolHelper.ResolveTool(file);
            if (value !=null)
            {   
                DesktopLauncher.OpenWithDesktop(value,file.GetPath());
                
            } else
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = file.GetPath(),
                    UseShellExecute = true
                });
            }
            
        }
        return null;

    }

    public FileManager? GoBack() 
    {
        var parentPath = new DirectoryInfo(WorkingDir.GetPath()).Parent?.FullName;

        if (parentPath == null)return null;

        ChangeDirectory(new DirItem(parentPath));

        return this;
    }
    
    public async void CastWorkingDirChanged()
    {
        WorkingDirChanged?.Invoke(WorkingDir); 
        await LoadFiles(); 
        App.Current.Cache.Cache.LastDir = WorkingDir.GetPath();
        PersistenceService.Save(App.Current.Cache);
    }
   
    public void ChangeHideItems(bool? state)
    {
        if (!state.HasValue)  return;
        App.Current.Cache.Config.DefaultIsHideItems = state.Value;
        CastWorkingDirChanged();
    }

    public void PrintList()
    {
        ListAll().ForEach(Console.WriteLine);
    }
}