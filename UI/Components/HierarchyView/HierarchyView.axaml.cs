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

        this.AttachedToVisualTree += (_, __) =>
        {
            this.AddHandler(TreeViewItem.ExpandedEvent, OnItemExpanded, RoutingStrategies.Bubble);
        };
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
