

using System;
using System.Collections.Generic;
using System.Linq;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Components;

namespace DynamicFileExplorer.Infrastructures;

class TabManager
{
    private static TabManager? instance;
    public readonly List<Tab> tabs = [];
    public Tab? FocusedTab {get; private set;}
    public event Action<Tab>? TabAdded, TabClosed;
    public event Action<Tab?>? TabFocused;
    public static TabManager Init()
    {
        instance ??= new TabManager();
        return instance;
    }

    public Tab CreateTab(DirItem dir)
    {
        var tab = new Tab() {fileManager = new(dir)};
        tabs.Add(tab);
        if (tabs.Count == 1) FocusTab(tab);
        TabAdded?.Invoke(tab);
        return tab;
    }

    public void FocusTab(Tab tab) {
        if(FocusedTab != tab) {
            FocusedTab = tab;
            TabFocused?.Invoke(tab);
        }
    }

    public void RemoveTab(Tab tab) {
        tabs.Remove(tab);
        if(FocusedTab == tab) {
            FocusedTab = tabs.LastOrDefault();
            TabFocused?.Invoke(FocusedTab);
        }
        TabClosed?.Invoke(tab);
    }

    public class Tab
    {
        public FileManager? fileManager {get; set;}
        public TabView? View {get; set;}
    }
}