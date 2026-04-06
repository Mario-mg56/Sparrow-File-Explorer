namespace DynamicFileExplorer.UI.Views.MainWindow;

using System;
using Avalonia;
using Avalonia.Controls;
using System.Reactive.Linq;     
using DynamicFileExplorer.UI.Components;
using Avalonia.Interactivity;
using System.Linq;
using DynamicFileExplorer.Infrastructures;

public partial class MainWindow : Window
{
    static readonly int COLS = 5;

    readonly Grid grid;
    readonly Button backButton, forwardButton;
    readonly FileManager fileManager;
    public MainWindow() {
        InitializeComponent();

        grid = this.FindControl<Grid>("FilesGrid")!;
        backButton = this.FindControl<Button>("BackButton")!;
        forwardButton = this.FindControl<Button>("ForwardButton")!;

        fileManager = App.Current.fileManager;
        int rows = (int) Math.Ceiling(fileManager.files.Count/(float)COLS);
        
        grid.GetObservable(BoundsProperty).Subscribe(bounds => {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            int cellSize =  (int) Math.Round(bounds.Width/COLS);

            for (int i = 0; i < rows; i++)
                grid.RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
            for (int i = 0; i < COLS; i++) 
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        });
        
        FileManager fm = App.Current.fileManager;

        backButton.Click += (_, _) => fm.GoBack();

        fileManager.WorkingDirChanged += (_) => RebuildGrid();
        RebuildGrid();
    }
    
    private void RebuildGrid()
    {
        grid.Children.Clear();
        int rows = (int) Math.Ceiling(fileManager.files.Count/(float)COLS);
        int i = 0;
        fileManager.files.ToList().ForEach(Console.WriteLine);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                if (i >= fileManager.files.Count)
                    return;

                var file = fileManager.files[i];

                var border = new Border
                {
                    Child = new FileView(file).layout
                };

                Grid.SetRow(border, r);
                Grid.SetColumn(border, c);

                grid.Children.Add(border);

                i++;
            }
        }
    }

}