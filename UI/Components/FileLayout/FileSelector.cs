
using System;
using System.Drawing;
using Avalonia.Controls;

namespace DynamicFileExplorer.UI.Components;

public class FileSelector : Border
{
    public int X {get; private set;} = 0;
    public int Y {get; private set;} = 0;
    public static readonly float ALPHA = 0.5f;
    public FileSelector()
    {
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.Overlay.Children.Add(this));

        Background = FileView.SELECTED_COLOR;
        Opacity = ALPHA;

    }

    public void SetPosition(Point start, Point end)
    {
        SetPosition(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y)); 
        Width = Math.Abs(start.X - end.X);
        Height = Math.Abs(start.Y - end.Y);     
    }

    public void Reset()
    {
        Width = 0;
        Height = 0;
    }

    private void SetPosition(int x, int y) {
        Canvas.SetLeft(this, x);
        Canvas.SetTop(this, y);
        X = x;
        Y = y;
    }
}