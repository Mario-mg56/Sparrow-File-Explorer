using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicFileExplorer.Helpers;
using DynamicFileExplorer.Models;

namespace DynamicFileExplorer.ViewModels;

public class FolderNodeViewModel : INotifyPropertyChanged
{
    
    private readonly DirItem Node;

    public SolidColorBrush Icon => Node.icon;
    public string Name => Node.GetPath().Equals(FileManager.GetRoot()?.GetPath())? "root":Node.path.name;

    private bool _isExpanded;
    private bool _isVisible;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            OnPropertyChanged();
            _isVisible = value;
        }
    }

    public static event Action<bool>? VisibilityChanged;

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
        VisibilityChanged += (v)=>IsVisible=v;
        Node = node;
        Children.Add(null!);
    }

    public static void ChangeVisibility(bool v)
    {
        VisibilityChanged?.Invoke(v);
    }

    
    public void ExpandNode()
    {

        Children.Clear(); // quitar placeholder

        var dirs = FileManager.ListAllDirectories(Node);

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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
