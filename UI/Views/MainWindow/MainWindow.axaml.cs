namespace DynamicFileExplorer.UI.Views.MainWindow;

using System;
using Avalonia;
using Avalonia.Controls;
using System.Reactive.Linq;     
using Avalonia.Media;
using DynamicFileExplorer.Placeholders;
using DynamicFileExplorer.UI.Components;

public partial class MainWindow : Window
{
    static readonly int COLS = 5;
    public MainWindow() {
        InitializeComponent();

        var grid = this.FindControl<Grid>("FilesGrid");
        if (grid == null) return;

        FileManager fm = new(17);
        int rows = (int) Math.Ceiling(fm.files.Count/(float)COLS);
        
        grid.GetObservable(BoundsProperty).Subscribe(bounds => {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            int cellSize =  (int) Math.Round(bounds.Width/COLS);

            for (int i = 0; i < rows; i++)
                grid.RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
            for (int i = 0; i < COLS; i++) 
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        });
        
        var rnd = new Random();
        int i = 0;
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < COLS; c++) {
                // var border = new Border {
                //     Child = new TextBlock{Text = $"{r}, {c}"},
                //     Background = new SolidColorBrush(Color.FromRgb
                //         ((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)))
                // };
                i++;
                if (i>=fm.files.Count) break;
                var file = fm.files[i];
                var border = new Border {
                    Child = new FileView(file.name, file.icon).layout
                };
                Grid.SetRow(border, r);
                Grid.SetColumn(border, c);
                grid.Children.Add(border);
            }
        }
    }
}