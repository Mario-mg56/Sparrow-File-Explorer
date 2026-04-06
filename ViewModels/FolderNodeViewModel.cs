using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using DynamicFileExplorer.Models;

namespace DynamicFileExplorer.ViewModels;

public class FolderNodeViewModel
{
    
    private readonly DirItem Node;

    public SolidColorBrush Icon => Node.icon;
    public string Name => Node.path.name;

    private bool _isExpanded;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value) return;

            _isExpanded = value;

            if (value)
                ExpandNode();
        }
    }

    public ObservableCollection<FolderNodeViewModel> Children { get; } = new();

    public FolderNodeViewModel(DirItem node)
    {
        Node = node;
        Children.Add(null!);
    }

    public void ExpandNode()
    {

        Children.Clear(); // quitar placeholder

        var dirs = App.Current.fileManager.ListAllDirectories(Node);

        foreach (var dir in dirs)
        {
            Children.Add(new FolderNodeViewModel(dir));
        }

    }

    public void AddChildren(FolderNodeViewModel item)
    {
        if (!Children.Any(c => c.Name == item.Name))
            Children.Add(item);
    }

    public void AddChildrens(List<FolderNodeViewModel> items)
    {
        foreach (var item in items)
        {
            if (!Children.Any(c => c.Name == item.Name))
                Children.Add(item);
        }
    }


    public DirItem GetNode()
    {
        return Node;
    }
}
