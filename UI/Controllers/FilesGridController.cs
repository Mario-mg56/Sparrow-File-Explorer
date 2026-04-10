using System;
using Avalonia.Controls;
using DynamicFileExplorer.Infrastructures;
using System.Reactive.Linq;     
using DynamicFileExplorer.UI.Components;
using Avalonia.Interactivity;
using Avalonia;
using DynamicFileExplorer;

class FilesGridController(Grid grid)
{
    static readonly int COLS = 5;
    readonly Grid grid = grid;
    readonly FileManager fileManager = App.Current.fileManager;
    public void Mount()
    {
        int rows = (int) Math.Ceiling(fileManager.files.Count/(float)COLS);
        
        grid.GetObservable(Window.BoundsProperty).Subscribe(bounds => {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            int cellSize =  (int) Math.Round(bounds.Width/COLS);

            for (int i = 0; i < rows; i++)
                grid.RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
            for (int i = 0; i < COLS; i++) 
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        });

        fileManager.WorkingDirChanged += (_) => RebuildGrid();
        RebuildGrid();
    }
    
    private void RebuildGrid()
    {
        grid.Children.Clear();
        int rows = (int) Math.Ceiling(fileManager.files.Count/(float)COLS);
        int i = 0;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < COLS; c++)
            {
                if (i >= fileManager.files.Count)
                    return;

                var file = fileManager.files[i];

                var border = new Border {
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