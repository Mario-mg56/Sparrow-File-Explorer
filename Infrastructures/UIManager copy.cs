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

namespace DynamicFileExplorer.Infrastructures;

class UM
{
    private static UM? instance;
    public MainViewModel? MainViewModel { get; private set; }
    public MainWindow? MainWindow { get; private set; }
    public FileLayoutController? FilesLayoutController { get; set; }
    public event Action<MainWindow>? OnMainWindowLoaded;
    public event Action<Border>? OnTabsResized;
    public readonly Dictionary<string, KeyEvent> KeyEvents = [];
    public UM()
    {
        Enum.GetNames(typeof(Key)).ToList().ForEach(k => KeyEvents.Add(k, new KeyEvent()));
        AddOnMainWindowLoadedListener(mw => {
            mw.KeyDown += (_, e) => KeyEvents[e.Key.ToString()].TriggerKeyPressed();
            mw.KeyUp += (_, e) => KeyEvents[e.Key.ToString()].TriggerKeyReleased();
            KeyEvents[Key.LeftCtrl + ""].KeyPressed += () => Console.WriteLine("LCtrl pressed");
        });
    }

    public static UM Init()
    {
        instance ??= new UM();
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

    // public KeyEvent GetKeyEvent(Key key)
    // {
    //     if (!KeyEvents.Keys.ToArray().Contains(key))
    //     {
    //         var ev = new KeyEvent();
    //         KeyEvents.Add(key, ev);
    //         ev.KeyPressed += () => ev.IsPressed = true;
    //         ev.KeyReleased += () => ev.IsPressed = false;
    //     }
    //     return KeyEvents[key];
    // }

    public class KeyEvent() { 
        public bool IsPressed {get; set;} = false;
        public event Action? KeyPressed, KeyReleased;
        public void TriggerKeyPressed() => KeyPressed?.Invoke();
        public void TriggerKeyReleased() => KeyReleased?.Invoke();
    }
}