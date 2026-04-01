using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using DynamicFileExplorer.Infrastructures;
using DynamicFileExplorer.Models;

namespace DynamicFileExplorer.UI.Components;

class FileView
{
    public string name;
    public readonly Grid layout;
    public readonly Border iconImg;
    public readonly TextBlock label;
    static readonly int PADDING = 10;
    public event Action? Clicked;
    private readonly FileSystemItem file;
    public FileView(FileSystemItem file, SolidColorBrush icon)
    {
        this.file = file;
        name = file.Name;

        layout = new Grid {
            Margin = new Thickness(PADDING)
        };

        layout.RowDefinitions.Add(new RowDefinition(new GridLength(3, GridUnitType.Star)));
        layout.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        layout.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        iconImg = new Border {
            Background = icon
        };

        label = new TextBlock {
            Text = name,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        Grid.SetRow(iconImg, 0);
        Grid.SetColumn(iconImg, 0);
        Grid.SetRow(label, 1);
        Grid.SetColumn(label, 0);

        layout.Children.Add(iconImg);
        layout.Children.Add(label);

        layout.PointerPressed += (_, _) =>
        {
            Clicked?.Invoke();
        };

        Clicked += Click;

        FileManager.GetInstance()!.FocusChanged += (focusedItems) => {
            foreach (var f in focusedItems) {
                if (f == file) {
                    layout.Background = new SolidColorBrush(Colors.LightBlue);
                    return;
                }
            }
            layout.Background = new SolidColorBrush(Colors.Transparent);
        };
    }
    public void Click()
    {
        FileManager fm = FileManager.GetInstance()!;

        if (file is FileItem fi) fm.Select(fi);
        else if(file is FolderItem fo) fm.Select(fo);
    }
}