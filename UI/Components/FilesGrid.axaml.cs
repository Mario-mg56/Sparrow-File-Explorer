namespace DynamicFileExplorer.UI.Components;

using System;
using Avalonia.Controls;
using DynamicFileExplorer.Infrastructures;
using System.Reactive.Linq;     
using Avalonia.Interactivity;
using Avalonia;
using Avalonia.Media;
using DynamicFileExplorer.Models;

class FilesGrid : Grid
{
    public static readonly int COLS = 7;
    public readonly ContextMenu<DirItem> contextMenu;
    private readonly FileManager fileManager = App.Current.fileManager;

    public FilesGrid()
    {
        int rows = (int) Math.Ceiling(fileManager.files.Count/(float)COLS);
        
        Background = new SolidColorBrush(Colors.Transparent); //Para que reciba eventos de mouse aunque no tenga fondo
        

        this.GetObservable(BoundsProperty).Subscribe(bounds => {
            RowDefinitions.Clear();
            ColumnDefinitions.Clear();

            int cellSize =  (int) Math.Round(bounds.Width/COLS);

            for (int i = 0; i < rows; i++)
                RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
            for (int i = 0; i < COLS; i++) 
                ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        });

        contextMenu = new (this, fileManager.WorkingDir, [
            new (name: "Item 1", itemAction: (i, wd, _) => Console.WriteLine("wd " + wd)),
            new (name: "Item 2", itemAction: (i, _, _) => Console.WriteLine(i.name + " selected")),
            new (name: "Item 3", itemAction: (i, _, _) => Console.WriteLine(i.name + " selected"))
        ]) {Background = Brushes.White};

        fileManager.WorkingDirChanged += (dir) => {
            contextMenu.SetAttachedItem(dir);
            RebuildGrid();
        };
        RebuildGrid();
    }
    
    private void RebuildGrid()
    {
        Children.Clear();
        int rows = (int) Math.Ceiling(fileManager.files.Count/(float)COLS);

        for (int i = 0, r = 0; r < rows; r++) {
            for (int c = 0; c < COLS && i < fileManager.files.Count; c++, i++) {
                var file = fileManager.files[i];
                var border = new Border {Child = new FileView(file)};

                SetRow(border, r);
                SetColumn(border, c);
                Children.Add(border);
            }
        }
    }

}