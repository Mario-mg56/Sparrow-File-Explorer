using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;

namespace DynamicFileExplorer.UI.Components;
public abstract class ResizableContainer
{
    protected Border border;
    protected bool isDraggin;

    public ResizableContainer(Border border)
    {
        this.border = border;
        
    }
    public virtual void RebuildBorder(Border border)
    {
        
    }


     protected void HeaderBorder_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!isDraggin) return;
        var pos = e.GetPosition(border);
        SetSize(pos);
    }
    protected void OnPressed(object? sender, PointerPressedEventArgs e) 
    {
        isDraggin = true;
    }
    protected void OnReleased(object? sender, PointerReleasedEventArgs e)
    {
        isDraggin = false;
    }


    public virtual void SetSize(Avalonia.Point pixels)
    {
        
    }
}