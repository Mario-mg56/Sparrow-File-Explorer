using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicFileExplorer;
using DynamicFileExplorer.Models;
using DynamicFileExplorer.ViewModels;

namespace DynamicFileExplorer.ViewModels;
public class HierarchyViewModel
{
    public ObservableCollection<FolderNodeViewModel> Roots { get; } = new();

    public HierarchyViewModel()
    {
        Roots.Add(new FolderNodeViewModel(App.Current.fileManager.GetRoot()));
        Roots[0].IsExpanded = true;
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
