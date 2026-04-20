using System;
using System.IO;
using Avalonia.Controls;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Components;
using DynamicFileExplorer.UI.Components.Inspector;
using DynamicFileExplorer.UI.Views.MainWindow;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.Infrastructures;

class UIManager
{
    private static UIManager? instance;
    public MainViewModel? MainViewModel { get; private set; }
    public MainWindow? MainWindow { get; private set; }
    public FileLayoutController? FilesLayoutController { get; set; }
    public event Action<MainWindow>? OnMainWindowLoaded;
    public event Action<Border>? OnTabsResized;

    public UIManager()
    {
        
    }

    public static UIManager Init()
    {
        instance ??= new UIManager();
        return instance;
    }

    public void AddOnMainWindowLoadedListener(Action<MainWindow> listener)
    {
        if (MainWindow != null) listener(MainWindow);
        else OnMainWindowLoaded += listener;
    }


    public void LoadFileInfo(FileSystemItem? file)
    {
        MainViewModel?.inspectorVisibility = file != null;
        InspectorViewModel.instance.file = file;
    }

    public void DeselectFIleInfo()
    {
        LoadFileInfo(null);
    }
    public void LoadMainWindow(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
        MainViewModel = (MainViewModel)MainWindow.DataContext;
        OnMainWindowLoaded?.Invoke(mainWindow);
    }

    public void CallResizeTabs(Border border)
    {
        OnTabsResized?.Invoke(border);
    }

    public double? GetRelativeWidth(Border border)
    {
        
        if (MainWindow is null)return null;
        return border.Bounds.Width/MainWindow.Bounds.Width;
    }
}