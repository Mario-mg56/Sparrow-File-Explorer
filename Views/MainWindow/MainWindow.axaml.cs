namespace DynamicFileExplorer.Views.MainWindow;

using System;
using Avalonia;
using Avalonia.Controls;
using System.Reactive.Linq;     
using Avalonia.Media;


public partial class MainWindow : Window
{
    static readonly int ROWS = 3, COLS = 5;
    public MainWindow() {
        InitializeComponent();

        var grid = this.FindControl<Grid>("FilesGrid");
        if (grid == null) return;
        
        grid.GetObservable(BoundsProperty).Subscribe(bounds => {
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            int cellSize =  (int) Math.Round(bounds.Width/COLS);

            Console.WriteLine(bounds.Width);

            for (int i = 0; i < ROWS; i++)
                grid.RowDefinitions.Add(new RowDefinition(new GridLength(cellSize)));
            for (int i = 0; i < COLS; i++) 
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        });
        
        var rnd = new Random();

        for (int r = 0; r < ROWS; r++) {
            for (int c = 0; c < COLS; c++) {
                var border = new Border {
                    Child = new TextBlock{Text = $"{r}, {c}"},
                    Background = new SolidColorBrush(Color.FromRgb
                        ((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)))
                };
                Grid.SetRow(border, r);
                Grid.SetColumn(border, c);
                grid.Children.Add(border);
            }
        }
    }
}