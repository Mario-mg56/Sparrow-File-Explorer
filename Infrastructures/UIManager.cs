using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Components;
using DynamicFileExplorer.UI.Components.Inspector;
using DynamicFileExplorer.UI.Views.MainWindow;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.Helpers;

class UIManager
{
    private static UIManager? instance;
    public MainViewModel? MainViewModel { get; private set; }
    public MainWindow? MainWindow { get; private set; }
    public FileLayoutController? FilesLayoutController { get; set; }
    public event Action<MainWindow>? OnMainWindowLoaded;
    public event Action<Border>? OnTabsResized;
    private readonly Dictionary<Key, KeyEvent> KeyEvents = [];
    public UIManager()
    {
        AddOnMainWindowLoadedListener(mw => {
            mw.KeyDown += (_, e) => GetKeyEvent(e.Key).TriggerKeyPressed();
            mw.KeyUp += (_, e) => GetKeyEvent(e.Key).TriggerKeyReleased();
        });
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


    public void LoadFileInfo(List<FileSystemItem> files)
    {
        MainViewModel?.inspectorVisibility = files != null;
        MainViewModel?.inspectorVisibility = (files?.Count ?? 0) != 0 ;
        InspectorViewModel.instance?.files = files;
    }

    public void DeselectFIleInfo()
    {
        LoadFileInfo(null!);
    }
    public void LoadMainWindow(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
        MainViewModel = (MainViewModel)MainWindow.DataContext!;
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

    public KeyEvent GetKeyEvent(Key key)
    {
        if (!KeyEvents.Keys.ToArray().Contains(key))
        {
            var ev = new KeyEvent();
            KeyEvents.Add(key, ev);
            ev.KeyPressed += () => ev.IsPressed = true;
            ev.KeyReleased += () => ev.IsPressed = false;
        }
        return KeyEvents[key];
    }

    public class KeyEvent() { 
        public bool IsPressed {get; set;} = false;
        public event Action? KeyPressed, KeyReleased;
        public void TriggerKeyPressed() => KeyPressed?.Invoke();
        public void TriggerKeyReleased() => KeyReleased?.Invoke();
    }
}