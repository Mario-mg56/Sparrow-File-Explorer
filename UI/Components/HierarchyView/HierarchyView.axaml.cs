using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.UI.Components;

public partial class HierarchyView : UserControl
{

    public HierarchyView()
    {
        DataContext = new HierarchyViewModel();
        InitializeComponent();
        App.Current.UIManager.OnTabsResized+=ResizeTexts;

        this.AttachedToVisualTree += (_, __) =>
        {
            this.AddHandler(TreeViewItem.ExpandedEvent, OnItemExpanded, RoutingStrategies.Bubble);
        };
           }



     public void ResizeTexts(Border border)
        {
            if(Parent?.Parent != border)return;
            double? widthPercent = App.Current.UIManager.GetRelativeWidth(border);
            if (widthPercent is null) return;
            FolderNodeViewModel.ChangeVisibility(widthPercent < 0.25? false: true);
        }
     private void OnItemExpanded(object? sender, RoutedEventArgs e)
    {
        if (e.Source is TreeViewItem item)
        {
            if (item.DataContext is FolderNodeViewModel vm)
            {
                vm.IsExpanded = true;
                Console.WriteLine("Nodo expandido: " + vm.Name);
            }
        }
    }

}
