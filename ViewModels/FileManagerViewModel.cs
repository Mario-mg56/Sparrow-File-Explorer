using System;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicFileExplorer.Infrastructures;

namespace DynamicFileExplorer.ViewModels;

class FileManagerViewModel {
    public ObservableCollection<FileViewModel> files = [];
    public Action? _onFilesChanged;    
    public FileManagerViewModel() {
        FileManager fm = App.Current.fileManager;
        LoadFiles();
        fm.WorkingFolderChanged += (_) => LoadFiles();
    }
    
    private void LoadFiles()
    {
        files.Clear();
        App.Current.fileManager.ListAll().ForEach (f =>files.Add(new FileViewModel(f)));
        files.ToList().ForEach(Console.WriteLine);
        Console.WriteLine("termine de cargar folders");
        _onFilesChanged?.Invoke();
    }
    
}