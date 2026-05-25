namespace DynamicFileExplorer.UI.Components;

using System;
using Avalonia.Controls;
using System.Reactive.Linq;     
using Avalonia.Interactivity;
using Avalonia;
using System.Collections.Generic;
using Avalonia.Media;


public class FilesGrid : Grid, IFileLayout
{
    public List<FileView> Items {get; private set;} = [];
    public readonly FileLayoutController fileLayoutController;
    public int IconSize {get => FileView.ICON_SIZE;}
    public int Cols {get; private set;}
    public int Rows {get; private set;}

    public FilesGrid()
    {
        Background = new SolidColorBrush(Colors.Transparent); //Para que reciba eventos de mouse aunque no tenga fondo
        fileLayoutController = new FileLayoutController(this);

        this.GetObservable(BoundsProperty).Subscribe(OnResize);
    }

    public void SetFileViews(List<FileView> fileViews)
    {
        Items = fileViews;
        OnResize(Bounds);
    }

    private void OnResize(Rect bounds)
    {
        if (bounds.Width == 0 || bounds.Height == 0) return;

        RowDefinitions.Clear();
        ColumnDefinitions.Clear();

        Cols = (int) Math.Floor((float) (bounds.Width/IconSize));
        Cols = Cols == 0 ? 1 : Cols;
        Rows = (int) Math.Ceiling(Items.Count/(float)Cols);

        for (int i = 0; i < Rows; i++)
            RowDefinitions.Add(new RowDefinition(new GridLength(IconSize)));
        for (int i = 0; i < Cols; i++) 
            ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        RebuildGrid();
    }
    
    private void RebuildGrid()
    {
        Children.Clear();

        for (int i = 0, r = 0; r < Rows; r++) {
            for (int c = 0; c < Cols && i < Items.Count; c++, i++) {
                var fv = Items[i];
                SetRow(fv, r);
                SetColumn(fv, c);
                Children.Add(fv);
            }
        }
    }
}