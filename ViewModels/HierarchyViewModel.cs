using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicFileExplorer;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.ViewModels;
public class HierarchyViewModel
{
    private FolderNodeViewModel _SelectedItem ;

    public FolderNodeViewModel SelectedItem
    {
        get => _SelectedItem;
        set
        {
            _SelectedItem = value;
            Console.WriteLine(value);
            App.Current.fileManager.ChangeDirectory(value.GetNode());
        }
    }
    public ObservableCollection<FolderNodeViewModel> Roots { get; } = [];

    public HierarchyViewModel()
    {
        var rootDir = (App.Current?.fileManager?.GetRoot()) ?? throw new Exception("GetRoot() devolvió null");
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
