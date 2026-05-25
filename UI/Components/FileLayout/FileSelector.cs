
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using DynamicFileExplorer.Util;

namespace DynamicFileExplorer.UI.Components;

public class FileSelector : Border
{
    public int X {get; private set;} = 0;
    public int Y {get; private set;} = 0;
    public Point Start {get; private set;} = new (0, 0);
    public Point End {get; private set;} = new (0, 0);
    public readonly Control attachedControl;
    public event Action<Point, Point>? Selecting;
    public Func<object?, PointerPressedEventArgs, bool>? StartSelectingCondition {
        get => dragController.StartDraggingCondition; 
        set => dragController.StartDraggingCondition = value;
    }
    private readonly DragController dragController;
    private Point startPosFSBuffer;
    public FileSelector(Control attachedControl)
    {
        this.attachedControl = attachedControl;
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => mw.Overlay.Children.Add(this));
        Background = new SolidColorBrush(Color.Parse(Persistence.Theme.Current.FileSelector));
        Persistence.Theme.ThemeChanged += theme => Background = new SolidColorBrush(Color.Parse(theme.FileSelector));

        dragController = new DragController(attachedControl);
            
        dragController.StartDragging += (d, _, e) => {
            var pos = e.GetPosition(d);
            startPosFSBuffer = new Point((int) Math.Round(pos.X), (int) Math.Round(pos.Y));
            IsVisible = true;
        };
        dragController.Drag += (d, _, e) => {
            var pos = e.GetPosition(d);
            SetPositionRelativeTo
            (startPosFSBuffer, new Point((int) Math.Round(pos.X), (int) Math.Round(pos.Y)), attachedControl);
            Selecting?.Invoke(Start, End);
        };
        dragController.StopDragging += (d, _, e) => {
            IsVisible = false;
            Reset();
        };
    }

    public void SetPosition(Point start, Point end)
    {
        Start = start;
        End = end;
        SetPosition((int) Math.Min(start.X, end.X), (int) Math.Min(start.Y, end.Y));
        Width = Math.Abs(start.X - end.X);
        Height = Math.Abs(start.Y - end.Y);
    }

    public void SetPositionRelativeTo(Point start, Point end, Control control)
    {
        App.Current.UIManager.AddOnMainWindowLoadedListener(mw => {
            SetPosition(control.TranslatePoint(start, mw) ?? new(0, 0),
                        control.TranslatePoint(end, mw) ?? new(0, 0));
        });
    }
    public void Reset()
    {
        Width = 0;
        Height = 0;
    }

    private void SetPosition(int x, int y) {
        X = x;
        Y = y;
        Canvas.SetLeft(this, X);
        Canvas.SetTop(this, Y);
    }
}