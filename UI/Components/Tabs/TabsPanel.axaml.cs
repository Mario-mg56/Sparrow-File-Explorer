using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.UI.Persistence;
using static DynamicFileExplorer.Helpers.TabManager;
using static DynamicFileExplorer.Models.Path;

namespace DynamicFileExplorer.UI.Components;

public class TabsPanel : ContentControl
{
    readonly StackPanel tabsContainer;
    private Tab? _lastFocusedBuffer;
    public TabsPanel()
    {
        var mainGrid = new Grid();
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star)); 
        mainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));    

        var scrollViewer = new ThinScrollBarViewer {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
        };
        tabsContainer = new StackPanel { Orientation = Orientation.Horizontal };
        scrollViewer.Content = tabsContainer;

        Grid.SetColumn(scrollViewer, 0);

        var addTabButton = new SVGIcon
        {
            IconPath = Icons.ADD_SVG_PATH, 
            IconSize = 16,
            Padding = new Thickness(12),
            Width = 32,
            Height = 32,
            CornerRadius = new CornerRadius(50),
            Margin = new Thickness(4, 0, 0, 0)
        };

        addTabButton.PointerReleased += (sender, e) => {
            if (e.InitialPressMouseButton == MouseButton.Left)
                App.Current.tabManager.CreateTab(App.Current.FocusedTab?.fileManager?.WorkingDir ?? new DirItem(BasePath)); 
        };

        Grid.SetColumn(addTabButton, 1);

        mainGrid.Children.Add(scrollViewer);
        mainGrid.Children.Add(addTabButton);

        App.Current.Tabs.ForEach(AddTab);

        FocusTab(App.Current.FocusedTab);
        App.Current.tabManager.TabAdded += AddTab;
        App.Current.tabManager.TabFocused += FocusTab;

        Content = mainGrid;
        MinHeight = 36;
    }

    private void AddTab(Tab tab) {
        string title = "";
        switch(tab.Type)
        {
            case TabType.FILES:
                title = tab.fileManager?.WorkingDir.path.name ?? "";
                tab.fileManager?.WorkingDirChanged += wd => tab.View?.Title = wd.path.name;
                break;
            case TabType.SETTINGS:
                title = "Settings";
                break;
        }

        tab.View = new TabView {Title = title};
        tabsContainer.Children.Add(tab.View);

        if (App.Current.FocusedTab == tab) FocusTab(tab);

        tab.View.ClickedTab += _ => App.Current.tabManager.FocusTab(tab);
        tab.View.ClosedTab += _ => RemoveTab(tab);
    }

    private void RemoveTab(Tab tab) {
        if (tab.View != null){
            tabsContainer.Children.Remove(tab.View);
            tab.View.ClickedTab -= _ => App.Current.tabManager.FocusTab(tab);
            tab.fileManager?.WorkingDirChanged -= wd => tab.View.Title = wd.path.name;
        }
        App.Current.tabManager.RemoveTab(tab); 
    }

    private void FocusTab(Tab? tab)
    {
        _lastFocusedBuffer?.View?.FocusTab(false);
        _lastFocusedBuffer = tab;
        tab?.View?.FocusTab(true);
    }
}