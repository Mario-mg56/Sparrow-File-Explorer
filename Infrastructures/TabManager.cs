

using System;
using System.Collections.Generic;
using System.Linq;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Components;

namespace DynamicFileExplorer.Helpers;

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
        var tab = new Tab() {
            Type = TabType.FILES,
            fileManager = new(dir)
        };
        tabs.Add(tab);
        TabAdded?.Invoke(tab);
        if (tabs.Count == 1) FocusTab(tab);
        return tab;
    }

    public Tab CreateSettingsTab()
    {
        Tab? tab = tabs.Find(t => t.Type == TabType.SETTINGS);
        if (tab == null){
            tab = new Tab() {Type = TabType.SETTINGS};
            tabs.Add(tab);
        }
        TabAdded?.Invoke(tab);
        FocusTab(tab);
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
        public TabType? Type {get; init;}
        public FileManager? fileManager {get; set;}
        public TabView? View {get; set;}
    }
    public enum TabType {FILES, SETTINGS}
}