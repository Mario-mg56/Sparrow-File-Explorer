using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;

namespace DynamicFileExplorer.ViewModels;

class FileManagerViewModel {
    public ObservableCollection<FileViewModel> files = [];
    public  Action _onFilesChanged;    
    public FileManagerViewModel(String fullPath) {
        FileManager fm = FileManager.GetInstance(new FolderItem(new PathFile(fullPath)));
        LoadFiles();
        fm.WorkingFolderChanged += OnWorkingFolderChanged;
    }
    private void OnWorkingFolderChanged(FolderItem f)
    {
        LoadFiles();
    }

    private void LoadFiles()
    {
        files.Clear();
        FileManager.GetInstance().ListAll()
                .ForEach (f =>{
                        if(f is FolderItem )
                        {
                            files.Add(new FileViewModel(f,true));
                        } else if (f is FileItem)
                        {
                            files.Add(new FileViewModel(f,false));
                        }
                }) ;
        
        files.ToList().ForEach(Console.WriteLine);
        Console.WriteLine("termine de cargar folders");
        _onFilesChanged?.Invoke();
    }
    
}