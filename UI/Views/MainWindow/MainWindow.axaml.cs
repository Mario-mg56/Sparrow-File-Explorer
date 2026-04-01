namespace DynamicFileExplorer.UI.Views.MainWindow;

using System;
using Avalonia;
using Avalonia.Controls;
using System.Reactive.Linq;     
using DynamicFileExplorer.UI.Components;
using DynamicFileExplorer.ViewModels;
using Avalonia.Interactivity;
using DynamicFileExplorer.Models;
using System.Linq;
using DynamicFileExplorer.Infrastructures;

public partial class MainWindow : Window
{
    static readonly int COLS = 5;

    readonly Grid grid;
    readonly Button backButton, forwardButton;
    FileManagerViewModel _FileManagerViewModel;
    public MainWindow() {
        InitializeComponent();

        grid = this.FindControl<Grid>("FilesGrid")!;
        backButton = this.FindControl<Button>("BackButton")!;
        forwardButton = this.FindControl<Button>("ForwardButton")!;

        _FileManagerViewModel = new(AppContext.BaseDirectory);
        int rows = (int) Math.Ceiling(_FileManagerViewModel.files.Count/(float)COLS);
        
        grid.GetObservable(BoundsProperty).Subscribe(bounds => {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            int cellSize =  (int) Math.Round(bounds.Width/COLS);

            for (int i = 0; i < rows; i++)
                grid.RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
            for (int i = 0; i < COLS; i++) 
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        });
        
        FileManager fm =FileManager.GetInstance()!;

        backButton.Click += (_, _) => fm.GoBack();

        _FileManagerViewModel._onFilesChanged += RebuildGrid;
        RebuildGrid();
    }
    
    private void RebuildGrid()
    {
        grid.Children.Clear();
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

                grid.Children.Add(border);

                i++; // incrementar DESPUÉS de usarlo
            }
        }
        Console.WriteLine(_FileManagerViewModel.files.Count());
        Console.WriteLine("termine de hacer rows");
    }

}