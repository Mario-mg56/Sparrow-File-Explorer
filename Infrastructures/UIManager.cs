using System;
using Avalonia.Controls;
using DynamicFileExplorer.UI.Components;
using DynamicFileExplorer.UI.Views.MainWindow;

namespace DynamicFileExplorer.Infrastructures;

class UIManager
{
    private static UIManager? instance;
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

    public void LoadMainWindow(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
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