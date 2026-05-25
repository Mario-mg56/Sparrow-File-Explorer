using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using DynamicFileExplorer.Helpers;

namespace DynamicFileExplorer.UI.Components;

public abstract class ResizableContainer
{
    protected Border TopBorder;
    public Border ThisBorder;
    protected bool isDragging;

    public GridLength initialSize;
    public GridLength lastSize = new GridLength(2, GridUnitType.Star);

    public ResizableContainer(Border topBorder, Border thisBorder)
    {
        TopBorder = topBorder;
        ThisBorder = thisBorder;
        
        RebuildBorder();

    }

    public virtual void RebuildBorder()
    {
        ThisBorder.BorderBrush = null;
        ThisBorder.BorderThickness = new Thickness(0);
        ThisBorder.Padding = new Thickness(0);
        App.Current.UIManager.OnMainWindowLoaded += (_)=> ThisBorder.Background = App.Current.UIManager.MainViewModel?.resizableColor;

        ThisBorder.PointerPressed += OnPressed;
        ThisBorder.PointerReleased += OnReleased;
        ThisBorder.PointerMoved += HeaderBorder_PointerMoved;

    }

    protected void HeaderBorder_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!isDragging)
            return;

        var pos = e.GetPosition(ThisBorder);
        SetSize(pos);
        App.Current.UIManager.CallResizeTabs(ThisBorder);
    }

    protected void OnPressed(object? sender, PointerPressedEventArgs e)
    {
        isDragging = true;
    }

    protected void OnReleased(object? sender, PointerReleasedEventArgs e)
    {
        isDragging = false;
    }

    public abstract void SetSize(Point pixels);
    public abstract void OnafterThingChange(bool yes);

    public abstract void SetPrevThingSize(GridLength newSize);
}
