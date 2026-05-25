using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using DynamicFileExplorer.Helpers;

namespace DynamicFileExplorer.ViewModels;
public class HierarchyViewModel
{
    private FolderNodeViewModel? _selectedItem;

public FolderNodeViewModel? SelectedItem
{
    get => _selectedItem;
    set
    {
        if (_selectedItem == value)
            return;

        _selectedItem = value;

        Console.WriteLine(value);

        if (value != null)
        {
            _ = App.Current.FocusedTab?.fileManager?.ChangeDirectory(value.GetNode());
        }
    }
}
    public ObservableCollection<FolderNodeViewModel> Roots { get; } = [];

    public HierarchyViewModel()
    {
        var rootDir = FileManager.GetRoot() ?? throw new Exception("GetRoot() devolvió null");
        var rootVm = new FolderNodeViewModel(rootDir);
        rootVm.IsExpanded = true;

        Roots.Add(rootVm);
    }

   

    public static void PrintTree(FolderNodeViewModel node, string indent = "", bool isLast = true)
    {
        // ├── o └── según si es el último hijo
        Console.Write(indent);

        if (isLast)
        {
            Console.Write("└── ");
            indent += "    ";
        }
        else
        {
            Console.Write("├── ");
            indent += "│   ";
        }

        Console.WriteLine(node.Name);

        var children = node.Children.ToList();

        for (int i = 0; i < children.Count; i++)
        {
            bool last = i == children.Count - 1;
            PrintTree(children[i], indent, last);
        }
    }

}
