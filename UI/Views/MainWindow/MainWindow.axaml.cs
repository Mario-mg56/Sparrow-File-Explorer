namespace DynamicFileExplorer.UI.Views.MainWindow;

using System;
using Avalonia;
using Avalonia.Controls;
using System.Reactive.Linq;     
using Avalonia.Media;
using DynamicFileExplorer.UI.Components;
using DynamicFileExplorer.ViewModels;
using DynamicFileExplorer.Infrastructures;
using Avalonia.Interactivity;
using System.IO;
using DynamicFileExplorer.Models;
using System.Linq;

public partial class MainWindow : Window
{
    static readonly int COLS = 5;

    Grid _Grid;
    FileManagerViewModel _FileManagerViewModel;
    public MainWindow() {
        InitializeComponent();

        _Grid = this.FindControl<Grid>("FilesGrid");
        if (_Grid == null) return;

        _FileManagerViewModel = new("/home");
        int rows = (int) Math.Ceiling(_FileManagerViewModel.files.Count/(float)COLS);
        
        _Grid.GetObservable(BoundsProperty).Subscribe(bounds => {
            _Grid.RowDefinitions.Clear();
            _Grid.ColumnDefinitions.Clear();

            int cellSize =  (int) Math.Round(bounds.Width/COLS);

            for (int i = 0; i < rows; i++)
                _Grid.RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
            for (int i = 0; i < COLS; i++) 
                _Grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        });

        _FileManagerViewModel._onFilesChanged += RebuildGrid;
        RebuildGrid();
    }
    
    private void RebuildGrid()
    {
        _Grid.Children.Clear();
        int rows = (int) Math.Ceiling(_FileManagerViewModel.files.Count/(float)COLS);
        int i = 0;
        _FileManagerViewModel.files.ToList().ForEach(Console.WriteLine);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                if (i >= _FileManagerViewModel.files.Count)
                    return; // salir completamente

                var item = _FileManagerViewModel.files[i];

                FileSystemItem file = item.IsFolder ? item.Folder : item.File;

                var border = new Border
                {
                    Child = new FileView(file, item.icon).layout
                };

                Grid.SetRow(border, r);
                Grid.SetColumn(border, c);

                _Grid.Children.Add(border);

                i++; // incrementar DESPUÉS de usarlo
            }
        }
        Console.WriteLine(_FileManagerViewModel.files.Count());
        Console.WriteLine("termine de hacer rows");
    }

}